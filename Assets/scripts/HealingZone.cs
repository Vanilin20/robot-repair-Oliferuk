using UnityEngine;

public class HealingZone : MonoBehaviour
{
    [Header("Healing Settings")]
    [Tooltip("Кількість здоров'я, яка відновлюється за один такт")]
    public int healAmount = 1;

    [Tooltip("Інтервал між відновленнями (у секундах)")]
    public float healInterval = 2.0f;

    private float timer;

    void OnTriggerStay2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null)
        {
            // Перевіряємо, чи гравець потребує лікування
            if (controller.health < controller.maxHealth)
            {
                timer -= Time.deltaTime;

                if (timer <= 0f)
                {
                    controller.ChangeHealth(healAmount);
                    timer = healInterval; // Скидаємо таймер на заданий інтервал
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Коли гравець виходить із зони, скидаємо таймер для наступного заходу
        if (other.GetComponent<PlayerController>() != null)
        {
            timer = 0f;
        }
    }
}