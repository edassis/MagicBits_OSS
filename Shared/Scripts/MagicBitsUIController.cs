using System;
using MagicBits.Minigame_2_x.Scripts;
using MagicBits.Shared.Debug;
using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class MagicBitsUIController : MonoBehaviour
    {
        const string name = "MagicBitsUIController";

        [SerializeField] private GameObject m_debugButtonContainer;
    
        private void Awake()
        {
            DebugController.OnStateChange += OnDebugStateChange;
            
            if (!Application.isEditor)
            {
                m_debugButtonContainer.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            DebugController.OnStateChange -= OnDebugStateChange;
        }

        private void OnDebugStateChange(bool state)
        {
            m_debugButtonContainer.SetActive(state);
        }
    }
}
