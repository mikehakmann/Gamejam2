using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;                  // Maximum health of the player
    private float currentHealth;                    // Current health of the player
    public GameObject teleporterPrefab;             // Prefab of the teleporter to spawn on death

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Check if the "K" key is pressed to kill the player
        if (Input.GetKeyDown(KeyCode.K))
        {
            Kill();
        }
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
        
        // Spawn the teleporter at the player's current position
        if (teleporterPrefab != null)
        {
            Instantiate(teleporterPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Teleporter prefab is not assigned!");
        }

        // Disable the player GameObject
        gameObject.SetActive(false);

        // Find all PowerUp objects in the scene and change their state
        EnemyDrop[] allPowerUps = FindObjectsOfType<EnemyDrop>();
        foreach (EnemyDrop powerUp in allPowerUps)
        {
            powerUp.ChangeState();
        }
    }
}