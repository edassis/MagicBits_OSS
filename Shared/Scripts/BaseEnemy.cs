using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public abstract class BaseEnemy : MonoBehaviour
    {
        public float moveSpeed = 2f;
        public float attackSpeed = 2f;
        public bool isMoving = true;
        protected Animator anim;
        protected Collider2D col2D;
        protected Rigidbody2D rb;
        protected bool canMove = true;
        protected bool isAttacking;
        protected int health = 100;
        protected bool isDead = false;
        // Start is called before the first frame update
        public virtual void Start()
        {
            anim = GetComponent<Animator>();
            col2D = GetComponent<Collider2D>();
            rb = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        public virtual void Update()
        {

        }

        public virtual void FixedUpdate()
        {

        }

        public virtual void OnTriggerEnter2D(Collider2D target)
        {

        }

        // Define se o inimigo pode andar ou não
        public virtual void SetCanMove(bool value)
        {
            canMove = value;
        }

        public virtual void Hit(int damage)
        {
            health -= damage;
            Debug.Log(health);
            Debug.Log(health); // Duplicado.
        }

        public virtual int GetHealth()
        {
            return health;
        }

        public virtual void SetHealth(int newHealth)
        {
            health = newHealth;
        }
        // Necessita implementar o movimento do inimigo
        protected abstract void Move();
        // Necessita implementar a morte do inimigo
        public abstract void Kill();
        // Necessita implementar o ataque do inimigo
        public abstract void Attack(GameObject target);
    }
}
