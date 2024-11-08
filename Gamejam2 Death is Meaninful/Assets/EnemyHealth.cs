using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;       // Maximum health of the enemy
    private float currentHealth;         // Current health of the enemy
    public float damageAmount = 20f;     // Damage amount to take per hit

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

    // Kill method that destroys the enemy
    void Kill()
    {
        // Optional: Add any death animations or effects here

        // Destroy the enemy GameObject
        Destroy(gameObject);
    }

    // Check for collisions based on the name of the GameObject
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the other object's name is "Spell"

        //TODO Change this to be handles from the spell
        if (other.gameObject.name == "Spell(Clone)")
        {
            // Take damage
            TakeDamage(damageAmount);
            
            // Optional: Destroy the "Spell" GameObject after it hits
            Destroy(other.gameObject);
        }
    }
}
