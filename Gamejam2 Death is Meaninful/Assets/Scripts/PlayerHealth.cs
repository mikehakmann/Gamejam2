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
    private float nextDamageTime = 0f;           // Time when next damage is allowed
    private bool isDamageOverTimeActive = false; // Flag to control damage over time

    private GameObject Endscreen;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar = GameObject.Find("HpMiddle").GetComponent<Image>();
        hpAnim = GameObject.Find("Hp").GetComponent<Animator>();
        Endscreen = GameObject.Find("Deathscreen");
        Endscreen.SetActive(false);
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

        // Apply damage over time if active
        if (isDamageOverTimeActive && Time.time >= nextDamageTime && currentHealth > 0)
        {
            TakeDamage(5);  // Apply 5 damage
            nextDamageTime = Time.time + 1f;  // Schedule the next damage after 1 second
        }

        // Update the health bar fill amount based on the current health
        healthBar.fillAmount = currentHealth / maxHealth;
    }
    
    public void SetHealth(float health)
    {
        currentHealth = health;
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

    // Enable or disable the damage over time effect
    public void ToggleDamageOverTime(bool enable)
    {
        isDamageOverTimeActive = enable;
        if (enable)
        {
            Debug.Log("Damage over time activated.");
            nextDamageTime = Time.time + 1f; // Start the timer for the next damage in 1 second
        }
        else
        {
            Debug.Log("Damage over time deactivated.");
        }
    }

    void Kill()
    {

        if(GameManager.Instance.gameState == GameState.Helheims)
        {
            Endscreen.SetActive(true);
            foreach(Transform child in Endscreen.transform)
            {
                child.gameObject.SetActive(true);
            }

        }
        else
        {
        GameManager.Instance.ChangeGameState();  // Change the game state when the player dies
        }

        Debug.Log("Player has died!");

        // Stop all continuous damage when the player dies
        ToggleDamageOverTime(false);

        // Spawn the teleporter at the player's current position
        if (teleporterPrefab != null)
        {
            Instantiate(teleporterPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Teleporter prefab is not assigned!");
        }
    }
}
