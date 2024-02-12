using System;
using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class Chest : MonoBehaviour
    {
        public bool isOpen { get; protected set; }
        protected Animator anim;
    
        public virtual void Awake()
        {
            anim = GetComponent<Animator>();
        }

        public virtual void Start()
        {
        }

        // Ativa animação de abrir o baú
        public virtual void Open()
        {
            if (isOpen)
                return;

            anim.SetBool("isOpen", true);
            isOpen = true;
        }

        public virtual void Close()
        {
            anim.SetBool("isOpen", false);
            isOpen = false;
        }

        // Reseta todas as informações do tesouro
        public virtual void Reset()
        {
            Close();
        }

        [Obsolete]
        public virtual void OnAnimationEnd()
        {
        }

        public virtual void OnAnimationOpenEnd()
        {
        }

        public virtual void OnAnimationCloseEnd()
        {
        }
    }
}
