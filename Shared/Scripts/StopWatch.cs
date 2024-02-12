using System.Collections;
using TMPro;
using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class StopWatch : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_tmp;
        [SerializeField] private bool m_isActive = true;
        [SerializeField] private bool m_resetOnEnable = true;

        private float m_time = 0f;

        private void OnEnable()
        {
            if (m_resetOnEnable) Reset();
            if (m_tmp) StartCoroutine(UpdateText());
        }

        private void Update()
        {
            if (!m_isActive) return;

            UpdateTimer();
        }

        public void Play()
        {
            m_isActive = true;
        }

        public void Pause()
        {
            m_isActive = false;
        }

        public void Stop()
        {
            m_isActive = false;
            Reset();
        }

        public void Reset()
        {
            m_time = 0f;
        }

        private void UpdateTimer()
        {
            m_time += Time.deltaTime;
        }

        private IEnumerator UpdateText()
        {
            while (m_isActive)
            {
                int minutes = Mathf.FloorToInt(m_time / 60);
                int seconds = Mathf.FloorToInt(m_time % 60);
                m_tmp.text = $"{minutes:00}:{seconds:00}";

                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
