using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;               // Reference to the player's Transform
    public float speed = 5f;               // Speed at which the enemy follows
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

    void Update()
    {
        if (player != null)  // Ensure the player is assigned
        {
            // Calculate the distance between the enemy and the player
            float distance = Vector3.Distance(transform.position, player.position);

            // Move toward the player if beyond the stopping distance
            if (distance > stoppingDistance)
            {
                // Calculate direction toward the player
                Vector3 direction = (player.position - transform.position).normalized;

                // Move the enemy toward the player
                transform.position += direction * speed * Time.deltaTime;
            }

            // Calculate the angle between the enemy's position and the player's position
            Vector3 difference = player.position - transform.position;
            float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg - 90f; // Add 45 degrees

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
