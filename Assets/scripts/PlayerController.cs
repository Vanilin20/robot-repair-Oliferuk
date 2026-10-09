using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    Rigidbody2D rigidbody2d;
    Vector2 move;

    [Header("Movement")]
    public float speed = 3.0f;

    [Header("Health")]
    public int maxHealth = 5;
    public int health { get { return currentHealth; } }
    int currentHealth;

    [Header("Invincibility")]
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float damageCooldown;

    void Start()
    {
        MoveAction.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
        
        // Встановлюємо повне здоров'я на старті
        currentHealth = maxHealth;
    }

    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();

        // Відлік таймера невразливості
        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        // Якщо отримуємо шкоду (amount < 0)
        if (amount < 0)
        {
            if (isInvincible)
            {
                return; // Якщо вже невразливі — шкоду не зараховуємо
            }
            isInvincible = true;
            damageCooldown = timeInvincible;
        }

        // Обмежуємо здоров'я в межах [0, maxHealth]
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
    }
}