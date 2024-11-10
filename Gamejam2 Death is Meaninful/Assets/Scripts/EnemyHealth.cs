using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;             // Maximum health of the enemy
    private float currentHealth;               // Current health of the enemy
    public float damageAmount = 20f;           // Damage amount to take per hit
    public GameObject onDeathSpawnPrefab;      // Reference to the prefab to spawn on death

    void Start()
    {
        // Set the enemy's health to the maximum at the start
        currentHealth = maxHealth;
    }

    // Method to take damage
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;  // Reduce health by the damage amount

        // Check if the health has dropped to zero or below
        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    // Kill method that destroys the enemy and spawns the prefab
    void Kill()
    {
        // Optional: Add any death animations or effects here

        // Spawn the prefab at the enemy's position if it's assigned
        if (onDeathSpawnPrefab != null)
        {
            Instantiate(onDeathSpawnPrefab, transform.position, Quaternion.identity);
            
        }
        else
        {
            Debug.LogWarning("onDeathSpawnPrefab is not assigned!");
        }

        // Destroy the enemy GameObject
        Destroy(gameObject);
    }


}