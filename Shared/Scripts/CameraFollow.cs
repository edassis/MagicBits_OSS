using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public float radius = 5f;
        [Range(0,10)]
        public float smooth;

        public bool isFollowing {get; set;}

        private bool isGoing = false;
        private Vector3 goingTo;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            // DrawnRadius();
            // CheckRadius();
            // Move();
        }

        private void DrawnRadius()
        {
            Color red = Color.red;

            Vector3 start = transform.position;
            Vector3 end = transform.position;
            Vector3 end2 = transform.position;

            start.y += 0.25f;
            end.y += 0.25f;
            end2.y += 0.25f;
            end.x -= radius;
            end2.x += radius;
            UnityEngine.Debug.DrawLine(start, end, red);
            UnityEngine.Debug.DrawLine(start, end2, red);
        }

        private void CheckRadius()
        {
            bool outRadius = (target.position.x < transform.position.x - radius) || (target.position.x > transform.position.x + radius);
            if(outRadius)
            {
                isFollowing = true;
            }
        }

        private void Move()
        {
            if(isFollowing)
            {
                Vector3 targetPosition = target.position;
                Vector3 cameraPosition = transform.position;
                Vector3 smoothPosition = Vector3.Lerp(cameraPosition, targetPosition, smooth*Time.deltaTime);

                transform.position = new Vector3(smoothPosition.x, cameraPosition.y, cameraPosition.z);

                isGoing = false;
            }
            else if(isGoing)
            {
                Vector3 cameraPosition = transform.position;
                Vector3 smoothPosition = Vector3.Lerp(cameraPosition, goingTo, smooth*Time.deltaTime);

                transform.position = new Vector3(smoothPosition.x, cameraPosition.y, cameraPosition.z);
            }
        }

        public void GoToPosition(Vector3 position)
        {
            isGoing = true;
            goingTo = position;
        }
    }
}
