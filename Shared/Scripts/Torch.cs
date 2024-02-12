using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace MagicBits_OSS.Shared.Scripts
{
    public class Torch : MonoBehaviour
    {
        [SerializeField] private TextMeshPro m_letter;
        [SerializeField] private SpriteRenderer m_imageAnswer;

        public UnityEvent onFireEnabled;
        public UnityEvent onFireDisabled;

        private Animator m_anim;

        private bool m_onFire;
        public bool onFire
        {
            get
            {
                return m_onFire;
            }
            set
            {
                m_onFire = value;
                m_anim.SetBool("on", value);

                if (value)
                    onFireEnabled.Invoke();
                else
                    onFireDisabled.Invoke();
            }
        }

        public string letter
        {
            get
            {
                return m_letter.text;
            }
            private set
            {
                m_letter.text = value;
            }
        }

        private string _nameImage;
        public string nameImage { 
            get
            {
                return _nameImage;
            }
            set
            {
                _nameImage = value;
                StartCoroutine(LoadImage(value));
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            m_anim = GetComponent<Animator>();
        }

        public void TurnMode(bool onFire)
        {
            this.onFire = onFire;
        }

        public void TurnMode()
        {
            this.onFire = !this.onFire;
        }

        private IEnumerator LoadImage(string image)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture($"{Application.streamingAssetsPath}/Resources/Minigame_1/{image}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                UnityEngine.Debug.Log($"Ocorreu um erro ao carregar a imagem do Altar: {request.error}");
            }
            else
            {
                Texture2D texture2D = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));

                m_imageAnswer.sprite = sprite;
            }
        }
    }
}
