using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    /// <summary>
    /// Script especializado em dar assistência com métodos de animação para objetos no jogo
    /// útil também para adicionar métodos de animação em eventos no editor.
    /// </summary>
    public class AnimatorUtility : MonoBehaviour
    {
        private Animator m_anim;
        
        void Awake()
        {
            m_anim = GetComponent<Animator>();
        }

        public void EnableBool(string name)
        {
            m_anim.SetBool(name, true);
        }
        
        public void DisableBool(string name)
        {
            m_anim.SetBool(name, false);
        }
    }
}
