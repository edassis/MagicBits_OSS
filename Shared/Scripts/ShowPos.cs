using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class ShowPos : MonoBehaviour
    {
        private void Awake()
        {
            UnityEngine.Debug.Log($"{name}: Pos at awake: {transform.position}");
        }

        private void Update()
        {
            UnityEngine.Debug.DrawRay(transform.position, Vector2.down, Color.red);
        }
    }
}
