using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class CircleDraw : MonoBehaviour
    {
        [SerializeField] private float m_radius;
        private float m_thetaScale = 0.01f; //Set lower to add more points
        private LineRenderer m_lineRenderer;
        private int m_size; //Total number of points in circle

        private void Awake()
        {
            m_size = (int)(2f*Mathf.PI/m_thetaScale) + 1;
            m_lineRenderer = gameObject.AddComponent<LineRenderer>();
            // m_lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
            m_lineRenderer.startColor = m_lineRenderer.endColor = Color.blue;
            m_lineRenderer.startWidth = m_lineRenderer.endWidth = 0.1f;
            m_lineRenderer.positionCount = m_size;
        }

        private void Update()
        {
            Vector3 pos;
            float theta = 0f;
            for (int i = 0; i < m_size; i++)
            {
                theta += (2.0f * Mathf.PI * m_thetaScale);
                float x = m_radius * Mathf.Cos(theta);
                float y = m_radius * Mathf.Sin(theta);
                x += gameObject.transform.position.x;
                y += gameObject.transform.position.y;
                pos = new Vector3(x, y, 0);
                m_lineRenderer.SetPosition(i, pos);
            }
        }
    }
}
