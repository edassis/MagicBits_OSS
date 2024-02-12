using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MagicBits.Minigame_2_x.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace MagicBits_OSS.Shared.Scripts
{
    [RequireComponent(typeof(IdGenerator), typeof(Speaker))]
    public abstract class MagicPuzzle : MonoBehaviour, IPuzzle, ISaveable
    {
        public GameObject focusCamera;
        public MagicPuzzleUI ui;
        public GameObject invisibleWall;
        public GameObject trigger;
        public Wizard player;
        public int failureWeight;
        public UnityEvent onPuzzleCompleted;

        protected bool isActive = false;
        protected bool isComplete = false;
        // key: conversation, value: pool of questions
        protected static Dictionary<string, List<DialogueUtility.Question>> s_questionsPool = new();
        protected DialogueUtility.Question currentQuestion;

        private IdGenerator m_idGenerator;
        private Speaker m_speaker;
    
        protected virtual void Awake()
        {
            GameController_2_2_1.OnGameStarted += OnGameStarted;

            m_idGenerator = GetComponent<IdGenerator>();
            m_speaker = GetComponent<Speaker>();
        }

        protected virtual void OnDestroy()
        {
            GameController_2_2_1.OnGameStarted -= OnGameStarted;
        }

        protected virtual void Update()
        {
            if (!isActive) return;

            if (Input.GetButtonDown("Cancel"))
            {
                CloseInteraction();
            }
        }

        public virtual void StartInteraction()
        {
            if (isActive || isComplete) return;

            player.PrepareMagic();

            SetUpUI();

            if (focusCamera)
                focusCamera.SetActive(true);
            if (ui)
                ui.gameObject.SetActive(true);

            this.ExecuteAfter(UpdateCurrentQuestion(), () =>
            {
                ui.DisplayQuestion(currentQuestion.text);
                ui.ActivatePanel(isActive);
            });
            ui.onAnswerConfirmed.AddListener(OnAnswerConfirmed);

            isActive = true;
        }

        public virtual void CloseInteraction()
        {
            ui.onAnswerConfirmed.RemoveListener(OnAnswerConfirmed);
            isActive = false;

            ui.ActivatePanel(false);

            if (focusCamera)
                focusCamera.SetActive(false);

            player.CancelMagic();
        }

        public virtual void FinishInteraction()
        {
            ui.onAnswerConfirmed.RemoveListener(OnAnswerConfirmed);
            isActive = false;
            isComplete = true;

            ui.ActivatePanel(false);

            if (invisibleWall)
                invisibleWall.SetActive(false);
            if (focusCamera)
                focusCamera.SetActive(false);

            player.FinishMagic();
            trigger.SetActive(false);

            onPuzzleCompleted.Invoke();
        }
    
        public abstract bool ValidateAnswer(string playerInput);

        // Carrega lista com as questões referentes a conversa no dicionário (s_questionsPool).
        // Seleciona 1 questão aleatoriamente da lista toda vez que é chamada.
        // Se não houver questões na lista, carrega do DB novamente.
        public IEnumerator UpdateCurrentQuestion()
        {
            string conversation = m_speaker.conversation;
        
            if (!s_questionsPool.ContainsKey(conversation))
                s_questionsPool.Add(conversation, new List<DialogueUtility.Question>());

            if (s_questionsPool[conversation].Count == 0)
                yield return DialogueUtility.LoadQuestionsFromConversationRoutine(conversation,
                    (questions) => { s_questionsPool[conversation].AddRange(questions); });

            int idx = UnityEngine.Random.Range(0, s_questionsPool[conversation].Count);
            currentQuestion = s_questionsPool[conversation][idx];
            s_questionsPool[conversation].RemoveAt(idx);
            
            Utilities.Log($"{name}: Question: {currentQuestion.text}");
            Utilities.Log($"{name}: Answer: {currentQuestion.answers.First()}");
        }

        protected abstract void SetUpUI();

        public virtual void OnAnswerConfirmed(string playerInput)
        {
            if (ValidateAnswer(playerInput))
            {
                ui.AnswerAccepted();
                this.SetTimeout(() => FinishInteraction(), 0.6f);
            }
            else
            {
                GameController_2_2_1.IncrementFails(failureWeight);
                ui.AnswerDenied();
            }
        }

        public void OnGameStarted()
        {
            if (isComplete)
            {
                if (invisibleWall)
                    invisibleWall.SetActive(false);
                if (focusCamera)
                    focusCamera.SetActive(false);
                trigger.SetActive(false);
                onPuzzleCompleted.Invoke();
            }
        }

        // ISaveable
        public string GetId()
        {
            return m_idGenerator.GetId();
        }

        public Dictionary<string, string> GetSaveData()
        {
            Dictionary<string, string> dict = new()
            {
                { "isComplete", $"{isComplete.ToString()}" }
            };
            // Debug.Log($"{this} {GetId()} dados a serem salvos {dict["isComplete"]}");
            return dict;
        }

        public void SetSaveData(Dictionary<string, string> dict)
        {
            isComplete = bool.Parse(dict["isComplete"]);
            // Debug.Log($"{this} {GetId()}: isComplete {isComplete}");
        }
    }
}
