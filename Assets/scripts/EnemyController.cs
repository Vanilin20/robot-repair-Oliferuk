using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 2.0f;
    public bool vertical;
    public float changeTime = 3.0f;

    Rigidbody2D rigidbody2d;
    Animator animator;
    float timer;
    int direction = 1;
    bool broken = true;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = changeTime;
    }

    void Update()
    {
        if (!broken) return;

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;

            // Випадкова зміна осі руху (Hard Challenge)
            if (Random.value > 0.5f)
            {
                vertical = !vertical;
            }
        }
    }

    void FixedUpdate()
    {
        if (!broken)
        {
            rigidbody2d.linearVelocity = Vector2.zero; // Зменшуємо швидкість до 0, якщо полагоджений
            return;
        }

        // Рух через фізичну швидкість (velocity) замість прямої зміни позиції
        if (vertical)
        {
            rigidbody2d.linearVelocity = new Vector2(0, speed * direction);
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            rigidbody2d.linearVelocity = new Vector2(speed * direction, 0);
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Перевіряємо, чи зіштовхнулися з гравцем
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ChangeHealth(-1);
            return;
        }

        // Якщо зіштовхнулися зі стіною/декорацією (не снаряд) — розвертаємося
        if (collision.gameObject.GetComponent<Projectile>() == null)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    public void Fix()
    {
        broken = false;
        rigidbody2d.simulated = false; // Вимикаємо фізику після ремонту
        animator.SetTrigger("Fixed");
    }
}