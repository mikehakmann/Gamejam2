using UnityEngine;
using System.Collections;  // Needed for IEnumerator

[RequireComponent(typeof(Collider2D))]
public class DamageArea : MonoBehaviour
{
    public float damagePerSecond = 10f;  // Damage per second
    private bool playerInRange = false;  // Flag to check if the player is within range
    private PlayerHealth playerHealth;   // Reference to the player's health script

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering the trigger has a PlayerHealth component
        playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerInRange = true;  // Set the flag to true if the player enters
            StartCoroutine(DamagePlayerOverTime());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Check if the object exiting the trigger has the same PlayerHealth component
        if (other.GetComponent<PlayerHealth>() == playerHealth)
        {
            playerInRange = false;  // Set the flag to false when the player leaves
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
    }
}