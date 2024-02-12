using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class TCCDialogue : MonoBehaviour
    {
        public TextMeshProUGUI textObject;
        public GameObject nextButton;
        private List<string> text;
        private int indexText = -1;
        public static Action onDialogueStart;
        public static Action onDialogueEnd;
        // public static Action<bool> onPauseRequest;
        // mesmo que (por�m Action sempre retorna void):
        // public delegate void OnPauseRequest();
        // public static event OnPauseRequest onPauseRequest
        void Start()
        { }

        void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                OnClose();
            }
        }

        private void InitializeDialogue()
        {
            nextButton.gameObject.SetActive(true);
            indexText = -1;
            TextNext();
        }

        private void TextNext()
        {
            indexText++;

            if (indexText < text.Count)
            {
                textObject.text = text[indexText];
            }
        }

        public void OnStart(List<string> text)
        {
            this.text = text;
            gameObject.SetActive(true);
            InitializeDialogue();
            onDialogueStart.Invoke();
        }

        public void OnClose()
        {
            gameObject.SetActive(false);
            onDialogueEnd.Invoke();
        }

        public void OnClickNextButton()
        {
            TextNext();
            if (indexText >= text.Count)
            {
                nextButton.gameObject.SetActive(false);
            }
        }
    }
}
