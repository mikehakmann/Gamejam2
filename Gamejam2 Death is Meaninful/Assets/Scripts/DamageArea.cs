using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class DamageArea : MonoBehaviour
{
    public float damageAmount = 20f;           // Damage amount per interval
    public float damageInterval = 1f;          // Interval (in seconds) between each damage tick
    private float nextDamageTime = 0f;         // Timestamp for the next allowed damage
    private PlayerHealth playerHealth;         // Reference to the player's health script

    void OnTriggerStay2D(Collider2D other)
    {
        // Check if the object in the trigger area has a PlayerHealth component
        if (playerHealth == null)
        {
            playerHealth = other.GetComponent<PlayerHealth>();
        }

        // If a player is detected and the damage interval has passed
        if (playerHealth != null && Time.time >= nextDamageTime)
        {
            playerHealth.TakeDamage(damageAmount);   // Apply damage
            nextDamageTime = Time.time + damageInterval;   // Set next damage time
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Clear player reference if they leave the area
        if (other.GetComponent<PlayerHealth>() == playerHealth)
        {
            playerHealth = null;
        }
    }
}