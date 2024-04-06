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
        public static event Action OnImported;
        public static event Action OnDataBaseReady;

        public static bool isDataBaseReady { get; private set; } = false;

        [Tooltip("Prioriza DB externo no editor (no export, o DB externo é sempre priorizado).")]
        public bool prioritizeExternalDB;

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
                    if (!_instance)
                    {
                        throw new ApplicationException("DialogueUtility instance not found.");
                    }
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
#else // Jogo exportado.
            prioritizeExternalDB = true;
#endif
            m_useExternalDB = prioritizeExternalDB;
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

            var masterDatabase = DialogueManager.instance.masterDatabase;
            PixelCrushers.DialogueSystem.Conversation externalConversation = null;
            PixelCrushers.DialogueSystem.Conversation internalConversation = null;
            List<Question> questions = new();

            if (s_instance.m_useExternalDB)
            {
                // Search for imported conversation's variant.
                externalConversation = masterDatabase.GetConversation(conversationName + "_RT");
                if (externalConversation != null)
                {
                    questions.AddRange(RetrieveQuestionsFromConversation(externalConversation));
                }
                else
                {
                    UnityEngine.Debug.LogWarning(
                        $"{s_instance.gameObject.name}: '{conversationName}' not found in external DB.");
                }
            }

            // Search in internal DB.
            // NOTE: Only if external DB failed or is not prioritized.
            if (externalConversation == null)
            {
                internalConversation = masterDatabase.GetConversation(conversationName);
                if (internalConversation != null)
                {
                    questions.AddRange(RetrieveQuestionsFromConversation(internalConversation));
                }
                else
                {
                    UnityEngine.Debug.LogWarning(
                        $"{s_instance.gameObject.name}: '{conversationName}' not found in internal DB.");
                }
            }

            if (questions.Count == 0)
            {
                UnityEngine.Debug.LogError(
                    $"{s_instance.gameObject.name}: Conversation '{conversationName}' not found!");
                yield break;
            }

            callback(questions);
        }

        private static List<Question> RetrieveQuestionsFromConversation(
            PixelCrushers.DialogueSystem.Conversation conversation)
        {
            // For each node in conversation:
            //      - Retrieve the nodes' sequence and dialogue text
            //      - Confirm the node calls the correct audio file (check sequence)
            var dialogues = new Dictionary<int, string>();
            var QtoALinks = new Dictionary<int, int>();
            foreach (var dialogueEntry in conversation.dialogueEntries.Where(dialogueEntry => dialogueEntry.id != 0))
            {
                //Debug.Log($"[{dialogueEntry.id}]: {dialogueEntry.DialogueText}");

                dialogues.Add(dialogueEntry.id, dialogueEntry.DialogueText);

                foreach (var link in dialogueEntry.outgoingLinks)
                {
                    // We are interested in the links that go from a question to an answer.
                    // Q -> A -x
                    // NOTE: Without some tagging mechanism to make clear which node is a question and which is an answer,
                    // we cannot have multiple nodes as answers deriving from the same question node as we wouldn't be able
                    // to differentiate them.

                    //Debug.Log($"Link para: {link.destinationDialogueID}");
                    QtoALinks.Add(link.originDialogueID, link.destinationDialogueID);
                }
            }

            // For each link:
            // - Key: question
            // - Value: answers
            var questions = new List<Question>();
            foreach (var link in QtoALinks)
            {
                var question = new Question();
                question.text = dialogues[link.Key];

                var answersSplitted = new List<string>(dialogues[link.Value].Split(';'));
                answersSplitted.ForEach(delegate(string s) { s = s.Trim(); });

                question.answers = answersSplitted;
                questions.Add(question);
            }

            //foreach (var each in ret)
            //{
            //    Debug.Log(each.text);
            //    foreach (var ans in each.answers)
            //    {
            //        Debug.Log(ans);
            //    }
            //}

            return questions;
        }

        private static IEnumerator ImportChatMapperXML()
        {
            // Como a aplicação roda no WebGl, classes ao sistema de arquivos não funcionam (System.IO),
            // como File, Path, Directory. Isso foi exaustivamente testado. Provavelmente, devido
            // aos arquivos estarem hospedados num servidor http.
            string path = $"Resources/{s_minigameName}-{m_levelName}-DB.xml";
#if UNITY_WEBGL && !UNITY_EDITOR
            Debug.Log($"Carregando DB de '{path}'...");
            yield return s_instance.StartCoroutine(GetTextFromWeb(path, ImportChatMapperRoutine));
#elif UNITY_EDITOR
            yield return null;
            path = Path.Combine(Application.dataPath, "../Build", $"{s_minigameName}/{m_levelName}", path);
            path = Path.GetFullPath(path);
            // Debug.Log($"Path '{path}'");
            try
            {
                Debug.Log($"Loading DB from '{path}'...");
                string text = File.ReadAllText(path);
                TextAsset textAsset = new TextAsset(text);
                ImportChatMapperRoutine(textAsset);
            }
            catch (IOException ex)
            {
                UnityEngine.Debug.LogError(
                    $"{s_instance.gameObject.name}: Wasn't possible to load DB from : '{path}' ({ex.GetType().ToString()})");
                yield break;
            }
#else
            Utilities.Log($"{s_instance.gameObject.name}: Import method not specified for this platform.");
            yield break;
#endif
            Utilities.Log($"{s_instance.gameObject.name}: Successfully imported Dialogue Databases.");
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
            Debug.Log($"Loading asset from '{path}' via Web.");
            
            var www = UnityWebRequest.Get(path);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                UnityEngine.Debug.LogError(www.error);
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
