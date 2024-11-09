using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;       // Maximum health of the player
    private float currentHealth;         // Current health of the player

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    void Kill()
    {
        Debug.Log("Player has died!");
        gameObject.SetActive(false);
        
        // Find all PowerUp objects in the scene and change their state
        PowerUp[] allPowerUps = FindObjectsOfType<PowerUp>();
        foreach (PowerUp powerUp in allPowerUps)
        {
            powerUp.ChangeState();
        }
    }
}