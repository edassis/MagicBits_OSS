using System;
using System.Collections;
using System.Collections.Generic;
using MagicBits.Minigame_2_x.Scripts;
using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class BasePlayer : MonoBehaviour
    {
        /*
         It is very important to notice that in both cases the event keyword is used to declare the event member.
         An event member declared this way is not simply a field of the class, despite it looks as if it was.
         Instead, the compiler creates it as an event property1. The event properties are similar to regular
         properties, except that they do not have get or set accessors. The compiler allows them to be used only on the
         left side of a += and -= assignments (adding or removing an event handler). There is no way to overwrite the
         already assigned event handlers, or to invoke the event outside the class that declares it.
         https://stackoverflow.com/a/22878784/8903027
         */
        public static event Action OnKill;

        // Fields
        public float speedMovement = 5f;
        public float jumpForce = 15f;
        public int timeDeadNormal = 3;
        public int timeDeadLimbo = 1;
        public float coyoteDuration = 0.25f;
        public float gravity = 5f;
        public AudioClip deathSound;
        public AudioClip jumpSound;
        public List<Transform> groundDetectors;

        protected Rigidbody2D rb;
        protected SpriteRenderer sr;
        protected Animator anim;
        protected AudioSource audioSource;
        protected bool isJumping = false;
        protected bool isDoubleJumping = false;
        protected bool isFalling = false;
        protected bool canMove = true;

        // Properties (getters and setters)
        public bool isDead { get; private set; }
        public bool isActive { get; private set; } = true;


        // Methods
        public virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
            anim = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();

            Spawn();
        }

        public virtual void Update()
        {
            if (!isActive || Time.timeScale == 0f)
                return;

            if (canMove)
            {
                Move();
                Jump();
            }
            else
                ResetAnimation();

            GroundDetection();
        }

        public virtual void OnCollisionEnter2D(Collision2D col)
        {
            if (!isActive) return;

            if (col.gameObject.layer == 7 || col.gameObject.tag == "Enemy")
            {
                Kill(timeDeadNormal);
                // GameController_2_2_1.IncrementFails();
            }
            else if (col.gameObject.tag == "MovingPlatform")
            {
                transform.parent = col.transform;
            }
        }

        public virtual void OnCollisionExit2D(Collision2D col)
        {
            if (!isActive) return;

            if (col.gameObject.layer == 7 || col.gameObject.tag == "Enemy")
            {
                Kill(timeDeadNormal);
                // GameController_2_2_1.IncrementFails();
            }
            else if (col.gameObject.tag == "MovingPlatform")
            {
                transform.parent = null;
            }
        }

        // Ativa o Movimento da Camera quando toca um Trigger de Camera
        public virtual void OnTriggerEnter2D(Collider2D col)
        {
            if (!isActive) return;

            if (col.gameObject.tag == "Respawn")
            {
                SaveCheckPoint(col.gameObject);
            }
            else if (col.gameObject.tag == "Limbo")
            {
                Kill(timeDeadLimbo);
            }
            else if (col.gameObject.tag == "Enemy")
            {
                Kill(timeDeadNormal);
                // GameController_2_2_1.IncrementFails();
            }
        }

        // Movimentação do Personagem e Acompanhamento da Cãmera
        protected virtual void Move()
        {
            if (isDead) return;

            float horizontalInput = Input.GetAxis("Horizontal");
            if (horizontalInput != 0f)
            {
                transform.eulerAngles =
                    new Vector3(transform.rotation.x, horizontalInput < 0f ? 180f : 0f, transform.rotation.z);
                // sr.flipX = (horizontalInput < 0f);
                anim.SetBool("isRunning", true);
                rb.velocity = new Vector2(horizontalInput * speedMovement, rb.velocity.y);
            }
            else
            {
                anim.SetBool("isRunning", false);
            }
        }

        protected virtual void Jump()
        {
            if (isDead) return;

            Action jumpAction = () =>
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                if (jumpSound)
                    PlaySound(jumpSound);
            };

            bool jumpInput = Input.GetButtonDown("Jump");
            if (jumpInput)
            {
                if (!isJumping)
                {
                    isJumping = true;
                    anim.SetBool("isJumping", isJumping);
                    jumpAction();
                }
                else if (!isDoubleJumping)
                {
                    isDoubleJumping = true;
                    jumpAction();
                }
            }
        }

        // Salva o ponto de checkpoint para respawn
        private void SaveCheckPoint(GameObject checkPoint)
        {
            GameObject listSpawns = GameController_2_2_1.GetListSpawns();
            int currentSpawn = GameController_2_2_1.s_spawn;
            int spawnCount = listSpawns.transform.childCount;

            for (int i = 0; i < spawnCount; i++)
            {
                if (i > currentSpawn && checkPoint.gameObject == listSpawns.transform.GetChild(i).gameObject)
                {
                    GameController_2_2_1.s_spawn = i;
                }
            }
        }

        private void ResetAnimation()
        {
            if (anim.GetBool("isRunning"))
                anim.SetBool("isRunning", false);
            if (anim.GetBool("isJumping"))
                anim.SetBool("isJumping", false);
        }

        // Spawna o jogador no ultimo checkpoint.
        public void Spawn()
        {
            GameObject listSpawns = GameController_2_2_1.GetListSpawns();
            if (!listSpawns)
                return;

            int spawn = GameController_2_2_1.s_spawn;
            int spawnCount = listSpawns.transform.childCount;

            if (spawn >= 0 && spawn < spawnCount)
            {
                Transform child = listSpawns.transform.GetChild(spawn);
                SpriteRenderer sr;
                float offSetY = 0.5f;

                if (child.TryGetComponent<SpriteRenderer>(out sr))
                {
                    offSetY = sr.size.y;
                }

                Vector3 positionSpawn = new Vector3(child.transform.position.x, child.transform.position.y + offSetY,
                    child.transform.position.z);
                transform.position = positionSpawn;
                Camera.main.transform.position = new Vector3(positionSpawn.x, Camera.main.transform.position.y,
                    Camera.main.transform.position.z);
            }
        }

        // Inicia processos de morte do jogador e impede ele de movimentar
        public virtual void Kill(int timeToDead = 3)
        {
            if (isDead)
                return;
            // Temporizador para reiniciar a cena.
            StartCoroutine(CoroutineKill(timeToDead));
        }

        // Coroutine de temporizador do Kill
        IEnumerator CoroutineKill(int timeToDead)
        {
            anim.SetBool("isDead", true);
            isDead = true;
            SetCanMove(false);
            GameController_2_2_1.deaths++;

            if (deathSound)
                GameController_2_2_1.PlaySound(deathSound);

            yield return new WaitForSeconds(timeToDead);
            OnKill?.Invoke();
        }

        // Define se o jogador pode se mover
        public virtual void SetCanMove(bool value)
        {
            rb.constraints = value
                ? rb.constraints & ~RigidbodyConstraints2D.FreezePositionX
                : rb.constraints | RigidbodyConstraints2D.FreezePositionX;

            canMove = value;
        }

        // Altera a animação atual do jogador
        public virtual void SetAnimatorState(string stateName)
        {
            anim.Play(stateName);
        }

        // Verifica o estado da animação atual do jogador
        public virtual bool IsCurrentAnimatorState(string stateName)
        {
            // Debug.Log(anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) + " : " + stateName);
            return anim.GetCurrentAnimatorStateInfo(0).IsName(stateName);
        }

        // Toca algum som
        public void PlaySound(AudioClip clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }

        public void FreezeBy(float time)
        {
            SetCanMove(false);
            isActive = false;
            this.SetTimeout(() =>
            {
                SetCanMove(true);
                isActive = true;
            }, time);
        }

        private void GroundDetection()
        {
            // Produto escalar, não detecta chão caso o jogador esteja subindo.
            if (Vector2.Dot(rb.velocity.normalized, Vector2.up) > 0.1) return;

            bool groundDetected = false;

            // Linhas de detecção que desativa variáveis do pulo
            groundDetectors.ForEach(groundDetector =>
            {
                float distance = 0.2f;

                RaycastHit2D hit = Physics2D.Raycast(groundDetector.position, Vector2.down, distance,
                    LayerMask.GetMask("Ground"));

                Color hitColor = Color.red;

                if (hit && hit.collider)
                {
                    // Debug.Log("colidiu " + hit.collider.gameObject.name);
                    hitColor = Color.green;

                    anim.SetBool("isJumping", false);
                    isJumping = false;
                    isDoubleJumping = false;
                    isFalling = false;
                    groundDetected = true;
                }

                UnityEngine.Debug.DrawRay(groundDetector.position, Vector2.down * distance, hitColor);
            });

            if (!groundDetected)
            {
                isFalling = true;
                StartCoroutine(CoyoteEffect());
            }
        }

        private IEnumerator CoyoteEffect()
        {
            yield return new WaitForSeconds(coyoteDuration);
            // Após a duração do "efeito Coyote", a queda é considerada como 1 pulo.
            if (isFalling) isJumping = true;
        }
    }
}
