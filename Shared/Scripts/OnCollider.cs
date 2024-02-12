using System.Collections.Generic;
using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class OnCollider : MonoBehaviour
    {
        public List<string> tags;
        public UnityEngine.Events.UnityEvent onColliderEnter;
        public UnityEngine.Events.UnityEvent onColliderExit;

        [System.Serializable]
        public class OnColliderPressButtonDown{
            public string button;
            public UnityEngine.Events.UnityEvent onPressButtonDown;
        };

        public List<OnColliderPressButtonDown> onColliderPressButtonDown;

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
                foreach(OnColliderPressButtonDown collider in onColliderPressButtonDown)
                {
                    if(Input.GetButtonDown(collider.button))
                    {
                        collider.onPressButtonDown.Invoke();
                    }
                }
            }
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            foreach(string i in tags)
            {
                if(col.collider.tag == i)
                {
                    onColliderEnter.Invoke();

                    isTouching = true;
                }
            }
        }

        void OnCollisionExit2D(Collision2D col)
        {
            foreach(string i in tags)
            {
                if(col.collider.tag == i)
                {
                    onColliderExit.Invoke();

                    isTouching = false;
                }
            }
        }
    }
}
