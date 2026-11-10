using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    public InputAction LaunchAction;
    public InputAction TalkAction = new InputAction("Talk", InputActionType.Button);

    public float speed = 3.0f;
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;
    public GameObject projectilePrefab;

    // Кулдаун для ДЗ ("Спробувати більше")
    public float timeBetweenShots = 0.5f;
    private float shootCooldown;

    public int health { get { return currentHealth; } }
    int currentHealth;

    bool isInvincible;
    float damageCooldown;

    Rigidbody2D rigidbody2d;
    Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);
    Vector2 move;

    private NonPlayerCharacter lastNonPlayerCharacter;

    void Start()
    {
        // Якщо в Inspector для Talk Action не задано клавішу — ставимо X
        if (TalkAction.bindings.Count == 0)
        {
            TalkAction.AddBinding("<Keyboard>/x");
        }

        MoveAction.Enable();
        LaunchAction.Enable();
        TalkAction.Enable();

        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }

        animator.SetFloat("Look X", moveDirection.x);
        animator.SetFloat("Look Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }

        shootCooldown -= Time.deltaTime;

        if (LaunchAction.WasPressedThisFrame() && shootCooldown <= 0)
        {
            Launch();
            shootCooldown = timeBetweenShots;
        }

        RaycastHit2D hit = Physics2D.Raycast(
            rigidbody2d.position + Vector2.up * 0.2f,
            moveDirection, 1.5f, LayerMask.GetMask("NPC"));

        if (hit.collider != null)
        {
            NonPlayerCharacter npc = hit.collider.GetComponent<NonPlayerCharacter>();
            if (lastNonPlayerCharacter != null && lastNonPlayerCharacter != npc)
            {
                lastNonPlayerCharacter.dialogueBubble.SetActive(false);
            }
            npc.dialogueBubble.SetActive(true);
            lastNonPlayerCharacter = npc;
            FindFriend();
        }
        else
        {
            if (lastNonPlayerCharacter != null)
            {
                lastNonPlayerCharacter.dialogueBubble.SetActive(false);
                lastNonPlayerCharacter = null;
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
        if (amount < 0)
        {
            if (isInvincible) return;
            isInvincible = true;
            damageCooldown = timeInvincible;
            animator.SetTrigger("Hit");
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection, 300);

        animator.SetTrigger("Launch");
    }

    void FindFriend()
    {
        if (TalkAction.WasPressedThisFrame())
        {
            UIHandler.instance.DisplayDialogue(lastNonPlayerCharacter.dialogueText);
        }
    }
}
