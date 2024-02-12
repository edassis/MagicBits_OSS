using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace MagicBits_OSS.Shared.Scripts
{
    public class OperationGround : MonoBehaviour
    {
        [SerializeField] private TextMeshPro m_plateText;

        [SerializeField] private GameObject m_walls;

        [SerializeField] private GameObject m_spears;

        private Collider2D []m_colliderWalls;
        private Collider2D m_collider2D;

        public UnityEvent<string> onEnter;

        public string plateText{
            get{
                return m_plateText.text;
            }
            set{
                m_plateText.text = value;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            m_collider2D = GetComponent<Collider2D>();
            m_colliderWalls = m_walls.GetComponents<Collider2D>();
        }

        // Update is called once per frame
        // void Update()
        // {
        
        // }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag == "Player")
            {
                m_collider2D.enabled = false;            
                onEnter.Invoke(plateText);
            }
        }

        public void EnableSpears()
        {                
            m_spears.SetActive(true);
        }

        public void SetWallsCollider(bool enabled)
        {
            foreach(Collider2D collider2D in m_colliderWalls)
                collider2D.isTrigger = !enabled;
        }

        public void SetNumber(int number)
        {
            plateText = number.ToString();
        }

        public void Reset()
        {
            plateText = "0";

            SetWallsCollider(false);
        
            m_collider2D.enabled = true;        
            m_spears.SetActive(false);
            onEnter.RemoveAllListeners();
        }

        public bool IsPlateTextOverflowing()
        {
            return m_plateText.isTextOverflowing;
        }
    }
}
