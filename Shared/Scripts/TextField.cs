using TMPro;
using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class TextField : MonoBehaviour
    {
        public GameObject panel;
        public TextMeshProUGUI tmp;

        public void Show()
        {
            panel.SetActive(true);
            tmp.gameObject.SetActive(true);
        }

        public void Hide()
        {
            panel.SetActive(false);
            tmp.gameObject.SetActive(false);
        }
    }
}
