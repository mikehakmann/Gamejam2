using UnityEngine;
using System.Collections;

public class DamageAreaCircleCast : MonoBehaviour
{
    public float damagePerSecond = 10f;      // Damage per second
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

            // Start damaging if the player enters the range
            if (detectedPlayerHealth != null && detectedPlayerHealth != playerHealth)
            {
                playerHealth = detectedPlayerHealth;
                playerInRange = true;

                // Start the damage coroutine if it isn't already running
                if (damageCoroutine == null)
                {
                    damageCoroutine = StartCoroutine(DamagePlayerOverTime());
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

    // Coroutine to damage the player over time
    private IEnumerator DamagePlayerOverTime()
    {
        while (playerInRange && playerHealth != null)
        {
            playerHealth.TakeDamage(damagePerSecond * Time.deltaTime);  // Apply damage per second
            yield return null;  // Wait until the next frame
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
