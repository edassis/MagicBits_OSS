using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class TriggerCamera : MonoBehaviour
    {
        public bool stopCamera;
        private CameraFollow cam;
        // Start is called before the first frame update
        void Start()
        {
            cam = Camera.main.GetComponent<CameraFollow>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if(col.tag == cam.target.gameObject.tag)
            {
                if(!stopCamera)
                {
                    cam.isFollowing = true;
                }
                else
                {
                    cam.GoToPosition(transform.position);
                    cam.isFollowing = false;
                }
            }
        }
    }
}
