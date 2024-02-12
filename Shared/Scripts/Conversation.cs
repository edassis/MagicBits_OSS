using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class Conversation : MonoBehaviour
    {
        public TextMeshPro textObject;
        [SerializeField] public GameObject message;
        public GameObject nextButton;
        [Multiline(6)]
        [SerializeField]
        public List<string> text;
        public float outLineAlpha = 1f;

        public int mode = 0;
        private int indexConversation = -1;
        private Renderer render;
        // private GameController gc;
        private bool existOutline = false;
        // private float timeEffect = 0f;
        // Start is called before the first frame update
        void Start()
        {
            if (!transform.parent) return;

            render = transform.parent.gameObject.GetComponent<Renderer>();
            // gc = transform.parent.gameObject.GetComponent<GameController>();

            foreach (string keyWord in render.material.shaderKeywords)
            {
                if (keyWord == "OUTBASE_ON")
                {
                    render.material.SetFloat("_OutlineAlpha", 0f);
                    existOutline = true;
                }
            }

            NextConversation();
        }

        void Update()
        {
            ClickNextButton();
            WritingEffect();
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag == "Player")
            {
                indexConversation = -1;
                NextConversation();
                if(message) message.SetActive(true);

                if (existOutline)
                    render.material.SetFloat("_OutlineAlpha", outLineAlpha);
            }
        }

        void OnTriggerExit2D(Collider2D col)
        {
            if (col.tag == "Player")
            {
                if(message) message.SetActive(false);

                if (existOutline)
                    render.material.SetFloat("_OutlineAlpha", 0f);
            }
        }

        private void WritingEffect()
        {
        }

        private void ClickNextButton()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                if (hit.collider != null && hit.collider.gameObject == nextButton)
                {
                    NextConversation();
                }
            }
        }

        private void NextConversation()
        {
            if (indexConversation + 1 < text.Count)
            {
                indexConversation++;
                textObject.text = text[indexConversation];
            }
        }
    }
}
