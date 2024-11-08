using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;       // Maximum health of the player
    private float currentHealth;         // Current health of the player

    void Start()
    {
        // Initialize the player's health to the maximum at the start
        currentHealth = maxHealth;
    }

    // Method to take damage, which can be called from external scripts
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;  // Reduce health by the specified amount

        // Check if the health has dropped to zero or below
        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    // Method to handle player death
    void Kill()
    {
        // Optional: Trigger game-over logic here or disable the player
        Debug.Log("Player has died!");

        // Optionally, deactivate the GameObject instead of destroying it
        gameObject.SetActive(false);

        // Additional logic like showing Game Over screen can be handled here
    }

    // Optional collision handling if you want automatic damage from certain objects
    void OnTriggerEnter2D(Collider2D other)
    {
        // Example: Check if the other GameObject is named "EnemyProjectile"
        if (other.gameObject.name == "EnemyProjectile")
        {
            TakeDamage(20f);  // Example damage amount

            // Optionally destroy the projectile on impact
            Destroy(other.gameObject);
        }
    }
}
