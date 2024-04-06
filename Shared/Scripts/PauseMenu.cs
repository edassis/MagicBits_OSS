using System;
using System.Collections;
using System.Collections.Generic;
using MagicBits.Minigame_2_x.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MagicBits_OSS.Shared.Scripts
{
    public class PauseMenu : MonoBehaviour
    {
        // Externo
        public Image muteIconElement;
        public Sprite unMuteImage;
        public Sprite muteImage;
        public GameObject debugButton;
    
        // Interno
        [SerializeField] private GameObject m_pausedPanel;
        [SerializeField] private CheatCodeUI m_cheatCodePanel;
        [SerializeField] private TextMeshProUGUI m_textVersion;

        public static event Action OnToggleDebugUI;
        public static event Action<bool> OnCheatCodeEntered;

        private HashSet<UnityEngine.KeyCode> m_keyPool = new();

        private void Awake()
        {  
            GameController_2_2_1.OnPrivilegedAccessChange += OnPrivilegedAccessChange;
        }

        private void OnDestroy()
        {
            GameController_2_2_1.OnPrivilegedAccessChange -= OnPrivilegedAccessChange;
        }
        
        private void Start()
        {
            m_textVersion.text = $"v{Application.version}";
        }

        private void Update()
        {
            if (!m_pausedPanel.activeSelf) return;
        
            // Handler passada para o CheatCodeUI
            Action<bool> cheatCodeEnteredHandler = (bool state) =>
            {
                m_cheatCodePanel.gameObject.SetActive(false);
                OnCheatCodeEntered?.Invoke(state);
            };

            if (m_keyPool.Contains(KeyCode.C) && m_keyPool.Contains(KeyCode.I))
            {
                if (!GameController_2_2_1.hasPrivilegedAccess)
                {
                    if (!m_cheatCodePanel.gameObject.activeSelf)
                    {
                        // Ativar painel para pegar senha.
                        m_cheatCodePanel.Init(cheatCodeEnteredHandler);
                        m_cheatCodePanel.gameObject.SetActive(true);
                        m_keyPool.Clear();
                    }
                    // Para isso funcionar tem q pegar as teclas no OnGUI,
                    // mas ao fazer isso, o comportamento não fica bom
                    // ao digitar no inputField.
                    // else
                    // {
                    //     m_CheatCodePanel.gameObject.SetActive(false);
                    //     m_keyPool.Clear();
                    // }
                }
                else
                {
                    cheatCodeEnteredHandler(false);
                    m_keyPool.Clear();
                }
            }
        }

        private void OnGUI()
        {
            if (!m_pausedPanel.activeSelf) return;

            Event e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                if (!m_cheatCodePanel.gameObject.activeSelf)
                {
                    switch (e.keyCode)
                    {
                        case KeyCode.C:
                        case KeyCode.I:
                            RegisterKeyPress(e.keyCode);
                            break;
                    }
                }
            }
        }

        public void Pause(bool value)
        {
            GameController_2_2_1.PauseGame(value);
        }

        public void Mute()
        {
            if (GameController_2_2_1.IsMutedSound())
            {
                GameController_2_2_1.MuteSound(false);
                muteIconElement.sprite = unMuteImage;
            }
            else
            {
                GameController_2_2_1.MuteSound(true);
                muteIconElement.sprite = muteImage;
            }
        }

        public void GiveUp()
        {
            UnityEngine.Debug.Log(SceneManager.GetSceneByBuildIndex(0));
            GameController_2_2_1.LoadSceneReseted(0);
        }

        public void RestartGame()
        {
            GameController_2_2_1.ResetSceneData();
            GameController_2_2_1.ReloadScene();
        }

        public void DebugToggle()
        {
            OnToggleDebugUI?.Invoke();
        }

        private void RegisterKeyPress(UnityEngine.KeyCode keyCode)
        {
            m_keyPool.Add(keyCode);
            if (m_keyPool.Count == 1) StartCoroutine(ClearInputKeys());
        }

        private IEnumerator ClearInputKeys()
        {
            yield return new WaitForSeconds(0.5f);
            m_keyPool.Clear();
        }
        
        private void OnPrivilegedAccessChange(bool state)
        {
            debugButton.SetActive(state);
        }
    }
}
