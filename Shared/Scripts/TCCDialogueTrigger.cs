using System.Collections.Generic;
using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class DialogueTrigger : MonoBehaviour
    {   
        public TCCDialogueEvent onDialogueActive;
        [Multiline(6)]
        [SerializeField]
        public List<string> text;

        void OnTriggerEnter2D(Collider2D col) {
            if (col.tag == "Player")
            {
                onDialogueActive.Invoke(new List<string>());
            }
        }
    }
}
