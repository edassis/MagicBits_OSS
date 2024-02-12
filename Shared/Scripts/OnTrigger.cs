using System.Collections.Generic;
using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class OnTrigger : MonoBehaviour
    {
        public List<string> tags;
        public UnityEngine.Events.UnityEvent onTriggerEnter;
        public UnityEngine.Events.UnityEvent onTriggerExit;

        [System.Serializable]
        public class OnTriggerPressButtonDown{
            public string button;
            public UnityEngine.Events.UnityEvent onPressButtonDown;
        };

        public List<OnTriggerPressButtonDown> onTriggerPressButtonDown;

        public bool isTouching {get; protected set;} = false;
        // Start is called before the first frame update
        // void Start()
        // {
        
        // }

        // Update is called once per frame
        void Update()
        {
            if(isTouching)
            {
                foreach(OnTriggerPressButtonDown trigger in onTriggerPressButtonDown)
                {
                    if(Input.GetButtonDown(trigger.button))
                    {
                        trigger.onPressButtonDown.Invoke();
                    }
                }
            }
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            foreach(string i in tags)
            {
                if(col.tag == i)
                {
                    onTriggerEnter.Invoke();

                    isTouching = true;
                }
            }
        }

        void OnTriggerExit2D(Collider2D col)
        {
            foreach(string i in tags)
            {
                if(col.tag == i)
                {
                    onTriggerExit.Invoke();

                    isTouching = false;
                }
            }
        }
    }
}
