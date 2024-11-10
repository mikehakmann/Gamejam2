using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;               // Reference to the player's Transform
    public float speed;                    // Constant speed at which the enemy follows
    public float stoppingDistance = 1.5f;  // Distance to stop following to avoid collision
    public Transform spriteTransform;      // Reference to the sprite's Transform (only this will rotate)

    void Awake()
    {
        // Find the player GameObject by name and assign its Transform
        GameObject playerObject = GameObject.Find("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player GameObject not found in the scene!");
        }
    }

    void FixedUpdate()
    {
        if (player != null)  // Ensure the player is assigned
        {
            // Calculate the direction toward the player
            Vector3 direction = (player.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, player.position);

            // Move the enemy toward the player at constant speed if beyond stopping distance
            if (distance > stoppingDistance)
            {
                // Move at a constant speed toward the player
                transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            }

            // Calculate the angle between the enemy's position and the player's position
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // Offset by 90 degrees

            // Apply rotation only to the spriteTransform's z-axis
            if (spriteTransform != null)
            {
                spriteTransform.rotation = Quaternion.Euler(0, 0, angle);
            }
            else
            {
                Debug.LogWarning("Sprite Transform not assigned in EnemyFollow.");
            }
        }
    }
}
