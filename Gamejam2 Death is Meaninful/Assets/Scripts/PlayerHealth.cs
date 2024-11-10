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
    private Coroutine damageOverTimeCoroutine;   // Reference to continuous damage coroutine

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

        // Update the health bar fill amount based on the current health
        healthBar.fillAmount = currentHealth / maxHealth;
    }
    
    public void SetHealth(float health)
    {
        currentHealth = health;
    }

    public IEnumerator DamageOverTime()
    {
        Debug.Log("DamageOverTime coroutine started.");
        while (currentHealth > 0)
        {
            currentHealth -= damagePerSecond * Time.deltaTime;

            if (currentHealth <= 0 && gameObject.activeSelf) // Only call Kill if player is still active
            {
                Kill();
                break;
            }
            yield return null;  // Wait until the next frame
        }
        Debug.Log("DamageOverTime coroutine ended.");
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

    // Function to start or stop the DamageOverTime coroutine
    public void ToggleDamageOverTime(bool enable)
    {
        if (enable)
        {
            
            // Start the DamageOverTime coroutine if it's not already running
            if (damageOverTimeCoroutine == null)
            {
                Debug.Log("Starting DamageOverTime coroutine.");
                damageOverTimeCoroutine = StartCoroutine(DamageOverTime());
            }
            else
            {
                Debug.Log("DamageOverTime coroutine is already running.");
            }
        }
        else
        {
            // Stop the DamageOverTime coroutine if it's currently running
            if (damageOverTimeCoroutine != null)
            {
                Debug.Log("Stopping DamageOverTime coroutine.");
                StopCoroutine(damageOverTimeCoroutine);
                damageOverTimeCoroutine = null;
            }
            else
            {
                Debug.Log("DamageOverTime coroutine is not running.");
            }
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

        // Stop continuous damage when the player dies
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
