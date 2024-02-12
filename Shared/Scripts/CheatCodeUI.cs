using System;
using TMPro;
using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class CheatCodeUI : MonoBehaviour
    {
        public const string CheatCode = "ISCFTW";

        [SerializeField] private TMP_InputField m_inputTMP;
        private Action<bool> m_cheatCodeEnteredHandler;

        public void Init(Action<bool> cheatCodeEnteredHandler)
        {
            m_cheatCodeEnteredHandler = cheatCodeEnteredHandler;
            // EventSystem.current.SetSelectedGameObject(gameObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown("return"))
            {
                ValidateInput();
            }
        }

        private void ValidateInput()
        {
            if (m_inputTMP.text.Trim() == CheatCode)
            {
                m_cheatCodeEnteredHandler(true);
            }
            else
            {
                m_cheatCodeEnteredHandler(false);
            }

            m_inputTMP.text = "";
        }
    }
}
