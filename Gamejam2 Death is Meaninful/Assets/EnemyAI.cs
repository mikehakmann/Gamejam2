using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float idleSpeed = 2f;              // Speed when idle wandering
    public float runAwaySpeed = 5f;           // Speed when running away from the player
    public float runDistance = 5f;            // Distance to start running away from the player
    public float runDuration = 1f;            // Duration to keep running after player leaves range
    public float idleDuration = 1f;           // Duration to walk in a random direction
    public float moveToCenterDuration = 1f;   // Duration to move toward the center when inside Border
    
    [HideInInspector] public float distanceToPlayer; // Distance to the player (visible in Inspector)
    
    private Transform player;                 // Reference to the player's Transform
    private Vector2 lastRunDirection;         // Last direction the enemy ran
    private Coroutine currentCoroutine;       // Reference to the current active coroutine

    // Define states
    private enum EnemyState { Idle, RunningAway, ContinueRunning, MovingToCenter }
    private EnemyState currentState = EnemyState.Idle;

    void Start()
    {
        // Try to find the player in the scene
        GameObject playerObject = GameObject.Find("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player GameObject not found in the scene!");
        }

        // Start in the idle state
        ChangeState(EnemyState.Idle);
    }

    void Update()
    {
        if (player != null)  // Ensure the player is assigned
        {
            // Calculate and display distance to player
            distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= runDistance && currentState != EnemyState.RunningAway)
            {
                ChangeState(EnemyState.RunningAway); // Start running away if close enough
            }
            else if (distanceToPlayer > runDistance && currentState == EnemyState.RunningAway)
            {
                ChangeState(EnemyState.ContinueRunning); // Continue running for a duration when player leaves range
            }
        }
    }

    void ChangeState(EnemyState newState)
    {
        // If the current state is the same as the new state, do nothing
        if (currentState == newState) return;

        // Log the state change
        Debug.Log($"Changing state from {currentState} to {newState}");

        // Stop the current coroutine if one is active
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        // Update the state
        currentState = newState;

        // Start the corresponding coroutine for the new state
        switch (currentState)
        {
            case EnemyState.Idle:
                currentCoroutine = StartCoroutine(IdleWander());
                break;
            case EnemyState.RunningAway:
                StartRunningAway();
                break;
            case EnemyState.ContinueRunning:
                currentCoroutine = StartCoroutine(ContinueRunningAfterExit());
                break;
            case EnemyState.MovingToCenter:
                currentCoroutine = StartCoroutine(MoveToCenter());
                break;
        }
    }

    void StartRunningAway()
    {
        // Set the direction away from the player
        Vector2 directionToPlayer = (transform.position - player.position).normalized;
        lastRunDirection = directionToPlayer;

        // Start or continue running away by starting a coroutine if it's not already running
        if (currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(RunAway());
        }
    }

    IEnumerator RunAway()
    {
        while (currentState == EnemyState.RunningAway)
        {
            // Keep moving in the last direction away from the player
            MoveInDirection(lastRunDirection, runAwaySpeed);
            LookInDirection(lastRunDirection); // Look in the direction of running

            // Update direction if the player has moved
            Vector2 directionToPlayer = (transform.position - player.position).normalized;
            lastRunDirection = directionToPlayer;

            yield return null; // Wait for the next frame to continue running away
        }

        currentCoroutine = null; // Clear the reference when the coroutine ends
    }

    void MoveInDirection(Vector2 direction, float moveSpeed)
    {
        // Move the enemy in the specified direction at the specified speed
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    IEnumerator ContinueRunningAfterExit()
    {
        // Keep running in the last direction for the specified duration
        float elapsed = 0f;
        while (elapsed < runDuration)
        {
            MoveInDirection(lastRunDirection, runAwaySpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // After running, switch to idle state
        ChangeState(EnemyState.Idle);
    }

    IEnumerator IdleWander()
    {
        while (currentState == EnemyState.Idle)
        {
            // Pick a random direction for idle movement
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float elapsed = 0f;

            while (elapsed < idleDuration)
            {
                MoveInDirection(randomDirection, idleSpeed); // Use idleSpeed for wandering
                LookInDirection(randomDirection);             // Look in the direction of wandering
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        currentCoroutine = null; // Clear the reference when the coroutine ends
    }

    IEnumerator MoveToCenter()
    {
        // Calculate direction to the center (0,0)
        Vector2 directionToCenter = ((Vector2)Vector2.zero - (Vector2)transform.position).normalized;

        float elapsed = 0f;
        while (elapsed < moveToCenterDuration)  // Move toward center for moveToCenterDuration seconds
        {
            MoveInDirection(directionToCenter, idleSpeed); // Move toward the center at idle speed
            LookInDirection(directionToCenter);            // Look in the direction of the movement
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Return to idle state after moving toward the center
        ChangeState(EnemyState.Idle);
    }

    void LookInDirection(Vector2 direction)
    {
        // Calculate the angle based on the given direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply the rotation only on the z-axis
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // If the enemy enters a "Border" trigger, move toward the center
        if (other.CompareTag("Border") && currentState != EnemyState.MovingToCenter)
        {
            ChangeState(EnemyState.MovingToCenter);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Keep the enemy moving to the center as long as it's in the "Border" trigger
        if (other.CompareTag("Border") && currentState != EnemyState.MovingToCenter)
        {
            ChangeState(EnemyState.MovingToCenter);
        }
    }

    void OnDrawGizmos()
    {
        // Draw a sphere to indicate the run distance in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, runDistance);

        // Draw a line between the enemy and the player if the player exists
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
