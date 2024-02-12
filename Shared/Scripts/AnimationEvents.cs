using UnityEngine;
using UnityEngine.Events;

namespace MagicBits_OSS.Shared.Scripts
{
    public class AnimationEvents : MonoBehaviour
    {
        public UnityEvent onAnimationEnd;
    
        public void OnAnimationEnd()
        {
            onAnimationEnd?.Invoke();
        }
    }
}
