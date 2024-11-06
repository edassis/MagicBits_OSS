using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace MagicBits_OSS.Shared.Scripts
{
    public static class MonoBehaviourExtensions
    {
        [Obsolete("Esperar 1 frame não garante que o estado atualizou. Pode ser que exista interpolação na" +
                  "transição ou ExitTime.")]
        private static IEnumerator WaitAnimationEnd(Animator animator, Action callback)
        {
            yield return null; // Wait 1 frame for animation clip update itself.
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
            callback();
        }
        
        [Obsolete]
        public static Coroutine WaitAnimationEndCoroutine(this MonoBehaviour component, Animator animator,
            Action callback)
        {
            return component.StartCoroutine(WaitAnimationEnd(animator, callback));
        }

        [Obsolete]
        public static Task WaitAnimationEnd(this MonoBehaviour component, Animator animator, Action callback)
        {
            return new Task(WaitAnimationEnd(animator, callback));
        }


        public static Task SetTimeout(this MonoBehaviour component, Action callback, float waitTime,
            bool isRealTime = false)
        {
            return new Task(Utilities.SetTimeoutRoutine(callback, waitTime, isRealTime));
        }

        private static IEnumerator CoroutineWrapper(this MonoBehaviour component, IEnumerator coroutine,
            Action callback)
        {
            yield return component.StartCoroutine(coroutine);
            callback();
        }
        
        /// <summary>
        /// <b>TESTE!</b>
        /// </summary>
        public static Task ExecuteAfter(this MonoBehaviour component, IEnumerator coroutine, Action callback)
        {
            return new Task(CoroutineWrapper(component, coroutine, callback));
        }

        /// <summary>
        /// <b>TESTE!</b><br/>
        /// Repare que esse recebe component e não faz nada com ele.
        /// </summary>
        public static Task WaitEventTask(this MonoBehaviour component, UnityEvent unityEvent, Action callback)
        {
            return new Task(Utilities.WaitEvent(unityEvent, callback));
        }

        /// <summary>
        /// <b>TESTE!</b>
        /// </summary>
        public static void WaitEvent(this MonoBehaviour component, UnityEvent unityEvent, Action callback)
        {
            component.StartCoroutine(Utilities.WaitEvent(unityEvent, callback));
        }
        
        public static void WaitCond(this MonoBehaviour component, Func<bool> predicate, Action callback)
        {
            component.StartCoroutine(Utilities.WaitCond(predicate, callback));
        }
    }
}
