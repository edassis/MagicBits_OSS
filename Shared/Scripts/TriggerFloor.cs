using UnityEngine;
using UnityEngine.Events;

namespace MagicBits_OSS.Shared.Scripts
{
    public class TriggerFloor : MonoBehaviour
    {
        [SerializeField] private Animator m_animator;
    
        private bool isTouching = false;

        [SerializeField] private UnityEvent onTriggered;
    
        // Não está chamando essa funcao.
        public void OnTriggerEntered(GameObject other)
        {
            if (other.CompareTag("Player"))
            {
                Enable();
                isTouching = true;
                onTriggered?.Invoke();
            }
        }

        public void OnTriggerExited(GameObject other)
        {
            isTouching = false;
        
            this.SetTimeout(() =>
            {
                if (!isTouching) Disable();
            }, 1f);
        }
    
        private void Enable()
        {
            m_animator.SetBool("active", true);
        }


        private void Disable()
        {
            // m_animator.SetTrigger("out");
            m_animator.SetBool("active", false);
        }
    }
}
