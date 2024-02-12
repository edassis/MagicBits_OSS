using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class PersistentData : MonoBehaviour
    {
    
        public static PersistentData instance
        {
            get
            {
                if (!m_instance)
                {
                    m_instance = new GameObject().AddComponent<PersistentData>();
                    m_instance.name = m_instance.GetType().ToString();
                    DontDestroyOnLoad(m_instance.gameObject);
                }

                return m_instance;
            }
            private set => m_instance = value;
        }
        private static PersistentData m_instance;

        public class Debug
        {
            public bool isActive = false;
        }

        public readonly Debug debug = new Debug();

        // void Awake()
        // {
        //     if (!m_instance)
        //     {
        //         m_instance = this;
        //         DontDestroyOnLoad(this.gameObject);
        //     }
        //     else if (m_instance != this)
        //     {
        //         Destroy(this.gameObject);
        //         m_instance = null;
        //     }
        // }
    }
}
