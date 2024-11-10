using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;               // Maximum health of the player
    private float currentHealth;                 // Current health of the player
    public GameObject teleporterPrefab;          // Prefab of the teleporter to spawn on death
    public float damagePerSecond = 5f;           // Amount of damage taken per second
    private Image healthBar;                     // Reference to the health bar image
    private Animator hpAnim;                     // Reference to the health bar animator
    private Coroutine damageOverTimeCoroutine;   // Coroutine for continuous damage

    void Start()
    {
        currentHealth = maxHealth;
        healthBar = GameObject.Find("HpMiddle").GetComponent<Image>();
        hpAnim = GameObject.Find("Hp").GetComponent<Animator>();

        // Start the continuous damage coroutine
        damageOverTimeCoroutine = StartCoroutine(DamageOverTime());
    }

    void Update()
    {
        // Check if the "K" key is pressed to kill the player
        if (Input.GetKeyDown(KeyCode.K))
        {
            Kill();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            TakeDamage(20);
        }

        // Update the health bar fill amount based on the current health
        healthBar.fillAmount = currentHealth / maxHealth;
    }

    private IEnumerator DamageOverTime()
    {
        while (currentHealth > 0)
        {
            currentHealth -= damagePerSecond * Time.deltaTime;

            if (currentHealth <= 0 && gameObject.activeSelf) // Only call Kill if player is still active
            {
                Kill();
            }
            yield return null;  // Wait until the next frame
        }
    }

    public void TakeDamage(float amount)
    {
        hpAnim.SetTrigger("HpBounce"); // Trigger health bar animation
        currentHealth -= amount;

        if (currentHealth <= 0 && gameObject.activeSelf) // Only call Kill if player is still active
        {
            Kill();
        }
    }

    void Kill()
    {
        Debug.Log("Player has died!");
        
        // Stop continuous damage when the player dies
        if (damageOverTimeCoroutine != null)
        {
            StopCoroutine(damageOverTimeCoroutine);
        }

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
