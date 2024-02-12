using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class ParallaxEffect : MonoBehaviour
    {
        public float parallaxRatio;
        private GameObject m_cam;
        private float m_length;
        private Vector3 m_startPos;

        private Vector3 pos
        {
            get => transform.position;
            set => transform.position = value;
        }
        private Vector3 camPos => m_cam.transform.position;
    
        private void Awake()
        {
            if (Camera.main != null) m_cam = Camera.main.gameObject;
            else UnityEngine.Debug.LogWarning($"{gameObject}: camera not found");
            m_startPos = m_cam.transform.position;
            m_length = GetComponent<SpriteRenderer>().bounds.size.x;
        }

        private void Update()
        {
            // https://www.youtube.com/watch?v=zit45k6CUMk
            // Distância em relação ao ponto inicial.
            float dist = m_cam.transform.position.x - m_startPos.x;
            // Atualiza posicao aplicando parallax.
            pos = new Vector3(m_startPos.x + dist * parallaxRatio, transform.position.y, transform.position.z);
            // Debug.DrawRay(new Vector3(transform.position.x, transform.position.y, -10), Vector3.right*m_length, Color.green);
            // Debug.DrawRay(new Vector3(m_startPos.x, m_startPos.y, -10), Vector3.right*m_length, Color.red);
            // Atualiza ponto inicial.
            if ((camPos - pos).x > m_length) m_startPos = camPos;
            else if ((camPos - pos).x < -m_length) m_startPos = camPos;
        }
    }
}
