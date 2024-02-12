using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.ChatMapper;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace MagicBits_OSS.Shared.Scripts
{
    [AddComponentMenu("MagicBits/Dialogue/Dialogue Utility")]
    public class DialogueUtility : MonoBehaviour
    {
        public static Action OnImported;
        public static Action OnDataBaseReady;

        public static bool hasInstance => s_instance != null;

        public static bool isDataBaseReady { get; private set; } = false;

        public class Question
        {
            public string text = "";
            public List<string> answers = new();
        }


        private static DialogueUtility _instance;

        private static DialogueUtility s_instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<DialogueUtility>();
                }

                return _instance;
            }
        }

        private static string s_minigameName = "";
        private static bool s_isUnityEditor = false;

        /// <summary>
        /// Active scene's name.
        /// </summary>
        private static string m_levelName = "";

        [Tooltip("Prioriza DB externo no editor (no export, o DB externo é sempre priorizado).")]
        public bool prioritizeExternalDB;

        private bool m_useExternalDB = false;

        private void Awake()
        {
            //     if (!hasInstance)
            //     {
            //         s_instance = this;
            //         //DontDestroyOnLoad(this.gameobject);
            //         // \-> Ao trocar de cena deve recarregar esse objeto.
            //     }
            //     else if (s_instance != this)
            //     {
            //         // Destroy(gameObject);
            //         s_instance = null;
            //     }

            s_isUnityEditor = (Application.platform == RuntimePlatform.WindowsEditor ||
                               Application.platform == RuntimePlatform.LinuxEditor ||
                               Application.platform == RuntimePlatform.OSXEditor);
        }

        private IEnumerator Start()
        {
            // Assets/Project/<minigame>/...
            s_minigameName = SceneManager.GetActiveScene().path.Split('/')[2];
            m_levelName = $"{SceneManager.GetActiveScene().name}";

            // Seleção do DB.
#if UNITY_EDITOR
            if (prioritizeExternalDB) m_useExternalDB = true;
#else // Jogo exportado.
        prioritizeExternalDB = true;
#endif
            if (m_useExternalDB) yield return StartCoroutine(ImportChatMapperXML());
            isDataBaseReady = true;
            OnDataBaseReady?.Invoke();
        }

        public static Coroutine LoadQuestionsFromConversationCoroutine(string conversationName,
            Action<List<Question>> callback)
        {
            return s_instance.StartCoroutine(LoadQuestionsFromConversationRoutine(conversationName, callback));
        }

        public static Task LoadQuestionsFromConversation(string conversationName, Action<List<Question>> callback)
        {
            return new Task(LoadQuestionsFromConversationRoutine(conversationName, callback));
        }

        public static IEnumerator LoadQuestionsFromConversationRoutine(string conversationName,
            Action<List<Question>> callback)
        {
            yield return new WaitUntil(() => isDataBaseReady);

            var ans = new List<Question>();

            var masterDatabase = DialogueManager.instance.masterDatabase;
            PixelCrushers.DialogueSystem.Conversation conversation = null;

            if (s_instance.m_useExternalDB)
            {
                // Search for imported conversation's variant.
                conversation = masterDatabase.GetConversation(conversationName + "_RT");
                if (conversation == null && s_isUnityEditor)
                {
                    UnityEngine.Debug.LogWarning(
                        $"{s_instance.gameObject.name}: \"{conversationName}\" não foi encontrado no DB externo.");
                }
            }

            // Search in intern DB.
            if (conversation == null) conversation = masterDatabase.GetConversation(conversationName);

            if (conversation == null)
            {
                UnityEngine.Debug.LogError($"{s_instance.gameObject.name}: Conversa ({conversationName}) não foi encontrada no DB!");
                yield break;
            }

            // For each node in conversation:
            //      - Retrieve the nodes' sequence and dialogue text
            //      - Confirm the node calls the correct audio file (check sequence)
            var dialogues = new Dictionary<int, string>();
            var linksTo = new Dictionary<int, int>();
            foreach (var dialogueEntry in conversation.dialogueEntries.Where(dialogueEntry => dialogueEntry.id != 0))
            {
                //Debug.Log($"[{dialogueEntry.id}]: {dialogueEntry.DialogueText}");

                dialogues.Add(dialogueEntry.id, dialogueEntry.DialogueText);

                foreach (var link in dialogueEntry.outgoingLinks)
                {
                    //Debug.Log($"Link para: {link.destinationDialogueID}");
                    linksTo.Add(link.originDialogueID, link.destinationDialogueID);
                }
            }

            // Para cada link:
            // - Key: uma pergunta
            // - Value: string com respostas
            var answersRaw = new Dictionary<int, string>(); // Valores ainda não foram tratados (num;num;...)
            foreach (var link in linksTo)
            {
                var question = new Question();
                question.text = dialogues[link.Key];

                var answersSplitted = new List<string>(dialogues[link.Value].Split(';'));
                answersSplitted.ForEach(delegate(string s) { s = s.Trim(); });

                question.answers = answersSplitted;
                ans.Add(question);
            }

            //foreach (var each in ret)
            //{
            //    Debug.Log(each.text);
            //    foreach (var ans in each.answers)
            //    {
            //        Debug.Log(ans);
            //    }
            //}
            callback(ans);
        }

        private static IEnumerator ImportChatMapperXML()
        {
            // Como a aplicação roda no WebGl, classes ao sistema de arquivos não funcionam (System.IO),
            // como File, Path, Directory. Isso foi exaustivamente testado. Provavelmente, devido
            // aos arquivos estarem hospedados num servidor http.
            string path = $"./{s_minigameName}/{m_levelName}/Resources/{s_minigameName}_{m_levelName}_DB.xml";
#if UNITY_WEBGL && !UNITY_EDITOR
        yield return s_instance.StartCoroutine(GetTextFromWeb(path, ImportChatMapperRoutine));
#elif UNITY_EDITOR
            yield return null;
            path = Path.Combine(Application.dataPath, "../Build", path);
            path = Path.GetFullPath(path);
            try
            {
                string text = File.ReadAllText(path);
                TextAsset textAsset = new TextAsset(text);
                ImportChatMapperRoutine(textAsset);
            }
            catch (IOException ex)
            {
                UnityEngine.Debug.LogWarning($"{s_instance.gameObject.name}: Não foi possível carregar o DB no caminho: {path} ({ex.GetType().ToString()})");
                yield break;
            }
#else
        Utilities.Log($"{s_instance.gameObject.name}: Import method not specified for this platform.");
        yield break;
#endif
            UnityEngine.Debug.Log($"{s_instance.gameObject.name}: Successfully imported Dialogue Databases.");
            OnImported?.Invoke();
        }

        private static void ImportChatMapperRoutine(TextAsset textAsset)
        {
            var chatMapperProject = ChatMapperUtility.Load(textAsset);
            AddExternalConversationToDb(chatMapperProject);
        }

        private static void AddExternalConversationToDb(ChatMapperProject chatMapperProject)
        {
            var importedDB = ChatMapperToDialogueDatabase.ConvertToDialogueDatabase(chatMapperProject);
            importedDB.conversations.ForEach((e) =>
            {
                if (!e.Title.Contains("_RT"))
                {
                    e.Title += "_RT";
                }
            });
            // Como o Dialogue Manager não é destruído, é importante resetar os diálogos carregados ao trocar de cena.
            DialogueManager.ResetDatabase(DatabaseResetOptions.RevertToDefault);
            DialogueManager.AddDatabase(importedDB);
            DialogueManager.SendUpdateTracker();
            //DialogueManager.instance.masterDatabase.conversations.ForEach((e) =>
            //{
            //    Debug.Log($"[{e.id}]: {e.Title}");
            //});
        }

        private static bool URLExists(string url)
        {
            var result = false;

            var webRequest = WebRequest.Create(url);
            webRequest.Timeout = 1200; // milliseconds
            webRequest.Method = "HEAD";

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)webRequest.GetResponse();
                result = true;
            }
            catch (WebException webException)
            {
                UnityEngine.Debug.Log(url + " doesn't exist: " + webException.Message);
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            return result;
        }

        private static IEnumerator GetTextFromWeb(string path, Action<TextAsset> callback)
        {
            var www = UnityWebRequest.Get(path);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                UnityEngine.Debug.LogWarning(www.error);
            }
            else
            {
                // Show results as text
                //Debug.Log(www.downloadHandler.text);

                // Or retrieve results as binary data
                //byte[] results = www.downloadHandler.data;

                callback(new TextAsset(www.downloadHandler.text));
            }
        }
    }
}
