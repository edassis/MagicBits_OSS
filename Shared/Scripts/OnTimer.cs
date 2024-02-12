using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MagicBits_OSS.Shared.Scripts
{
    public class OnTimer : MonoBehaviour
    {
        [SerializeField] private bool m_unscaled;
        [SerializeField] private bool m_repeat;
        [SerializeField] private List<OnTimerGroup> m_onTimer;

        [System.Serializable]
        private class OnTimerGroup
        {
            public bool isRunning
            {
                get; set;
            }
            public bool removeOnConclude;
            public float timer;
            private float m_saveTimer;
            public UnityEvent onConcluded;

            public void InitalizeTimer()
            {
                m_saveTimer = timer;
            }

            public void ResetTimer()
            {
                timer = m_saveTimer;
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            foreach (OnTimerGroup group in m_onTimer)
            {
                group.InitalizeTimer();
                group.ResetTimer();
            }

            if (m_onTimer.Count > 0)
                m_onTimer[0].isRunning = true;
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < m_onTimer.Count; i++)
            {
                if (m_onTimer[i].isRunning)
                {
                    m_onTimer[i].timer -= m_unscaled ? Time.unscaledDeltaTime : Time.deltaTime;

                    if (m_onTimer[i].timer <= 0f)
                    {
                        ConcludeTimer(i);
                    }
                    break;
                }
            }
        }

        public void ForceConclude()
        {
            for (int i = 0; i < m_onTimer.Count; i++)
            {
                if (m_onTimer[i].isRunning)
                {
                    ConcludeTimer(i);
                    break;
                }
            }
        }

        private void ConcludeTimer(int index)
        {
            m_onTimer[index].onConcluded.Invoke();
            m_onTimer[index].isRunning = false;
            m_onTimer[index].ResetTimer();

            if (index != m_onTimer.Count - 1)
                m_onTimer[index + 1].isRunning = true;
            else if (m_repeat)
                m_onTimer[0].isRunning = true;

            if (m_onTimer[index].removeOnConclude)
            {
                m_onTimer.RemoveAt(index);
            }
        }
    }
}
