using UnityEngine;
using System.Collections;

public class TeleportToMidgaard : MonoBehaviour
{
    public enum State { Closed, Opened }
    public State currentState = State.Closed;

    public GameObject ClosedSprite;              // Reference to the closed state visual
    public GameObject OpenSprite;                // Reference to the open state visual
    public Collider2D CircleTriggerCollider2D;   // Reference to the trigger collider
    public float initialClosedDuration = 5f;     // Duration for which the closed state is active

    private void Start()
    {
        // Initialize to the closed state
        SetState(State.Closed);

        // Start coroutine to transition to open state after the specified duration
        StartCoroutine(SwitchToOpenedStateAfterDelay());
    }

    private IEnumerator SwitchToOpenedStateAfterDelay()
    {
        yield return new WaitForSeconds(initialClosedDuration);

        // Switch to the opened state after the delay
        SetState(State.Opened);
    }

    private void SetState(State newState)
    {
        currentState = newState;

        if (currentState == State.Closed)
        {
            OnClosedState();
        }
        else if (currentState == State.Opened)
        {
            OnOpenedState();
        }
    }

    private void OnClosedState()
    {
        // Set ClosedSprite active and disable OpenSprite and trigger collider
        ClosedSprite.SetActive(true);
        OpenSprite.SetActive(false);
        CircleTriggerCollider2D.enabled = false;
    }

    private void OnOpenedState()
    {
        // Set OpenSprite active and enable the trigger collider
        ClosedSprite.SetActive(false);
        OpenSprite.SetActive(true);
        CircleTriggerCollider2D.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && GameManager.Instance != null && currentState == State.Opened)
        {
            // When the player enters the trigger in the opened state, change the GameState to Midgard
            GameManager.Instance.ChangeGameState();
            Debug.Log("Player entered trigger - GameState changed to Midgard");
        }
    }
}
