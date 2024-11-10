using UnityEngine;
using System.Collections;

public class DamageAreaCircleCast : MonoBehaviour
{
    public float damageAmount = 20f;         // Fixed amount of damage dealt each time the player is hit
    public float damageRadius = 5f;          // Radius for the circle cast
    public LayerMask playerLayer;            // Layer mask to identify the player

    private bool playerInRange = false;      // Flag to check if the player is within range
    private PlayerHealth playerHealth;       // Reference to the player's health script
    private Coroutine damageCoroutine;       // Reference to the damage coroutine

    void Update()
    {
        // Perform a circle cast to detect the player within the specified radius
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, damageRadius, Vector2.zero, 0f, playerLayer);

        // Check if the circle cast hit a player
        if (hit.collider != null)
        {
            PlayerHealth detectedPlayerHealth = hit.collider.GetComponent<PlayerHealth>();

            // Start damaging if the player enters the range and isn't already being damaged
            if (detectedPlayerHealth != null && detectedPlayerHealth != playerHealth)
            {
                playerHealth = detectedPlayerHealth;
                playerInRange = true;

                // Start the damage coroutine if it isn't already running
                if (damageCoroutine == null)
                {
                    damageCoroutine = StartCoroutine(DamagePlayerOnHit());
                }
            }
        }
        else
        {
            // Stop damaging if no player is detected within the radius
            playerInRange = false;
            playerHealth = null;

            // Stop the coroutine if the player has left the range
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    // Coroutine to apply a fixed amount of damage each time the player is within range
    private IEnumerator DamagePlayerOnHit()
    {
        while (playerInRange && playerHealth != null)
        {
            // Deal a fixed damage amount to the player
            playerHealth.TakeDamage(damageAmount);

            // Wait for a cooldown period (e.g., 1 second) before potentially hitting again
            yield return new WaitForSeconds(1f); // Adjust this delay as needed
        }

        // Reset the coroutine reference when it finishes
        damageCoroutine = null;
    }

    // Visualize the damage area in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
