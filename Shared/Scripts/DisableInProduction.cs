using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class DisableInProduction : MonoBehaviour
    {
        void Start()
        {
#if !UNITY_EDITOR
        // Ativo somente no editor.
        gameObject.SetActive(false);
#endif
        }
    }
}
