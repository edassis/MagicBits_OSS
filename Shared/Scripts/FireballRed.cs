using System.Collections.Generic;
using MagicBits.Minigame_2_x.Scripts;
using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class FireballRed : MonoBehaviour
    {
        public Transform target;
        // public Transform target { get; set; }
        public float moveSpeed = 5f;
        public float timeToAttack = 30f;
        public List<GameObject> effectExplosionReference;
        public UnityEngine.Events.UnityEvent onHiden;
        public UnityEngine.Events.UnityEvent onFall;
        public UnityEngine.Events.UnityEvent onKill;
        private Rigidbody2D rb;
        private Collider2D col;
        private SpriteRenderer sprite;
        private float timer;
        private bool isWaiting = false;
        private bool isFalling = false;
        private bool isDead = false;
        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            sprite = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!GameController_2_2_1.IsGamePaused())
            {
                if (isWaiting)
                {
                    if (timer < timeToAttack)
                    {
                        timer += Time.deltaTime;
                    }
                    else
                    {
                        Fall();
                    }

                    rb.velocity = Vector2.zero;
                }
                else
                {
                    Move();
                }
            }
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            // Debug.Log("entrou");
            if (col.tag == "AttackTrigger")
            {
                if (!isFalling)
                {
                    isWaiting = true;
                    sprite.enabled = false;
                    onHiden.Invoke();
                }
            }
            else if(col.tag != "Enemy" && col.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
            {
                Kill(col.gameObject.transform);

                if(col.tag == "Player")
                {
                    BasePlayer player = col.gameObject.GetComponent<BasePlayer>();
                    player.Kill();
                }
            }
        }

        // Função responsável para sinalizar ao Fireball que ele deve cair
        private void Fall()
        {
            transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 270f);
            isFalling = true;
            isWaiting = false;
            sprite.enabled = true;
            onFall.Invoke();
        }

        private void Move()
        {
            rb.velocity = (isFalling ? Vector2.down : Vector2.up) * moveSpeed;
        }

        public void Kill(Transform origin)
        {
            if (!isDead)
            {
                onKill.Invoke();
                Destroy(gameObject);

                foreach(GameObject effect in effectExplosionReference)
                {
                    Instantiate(effect, origin.position, effect.transform.rotation);
                }

                isDead = true;
            }
        }
    }
}
