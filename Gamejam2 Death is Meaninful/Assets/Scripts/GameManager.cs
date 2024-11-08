using System.Collections;
using UnityEngine;

public enum GameState
{
    Midgard,
    Helheims
}

public class GameManager : MonoBehaviour
{
    public GameState gameState;
    public GameObject world;

    private float OrignalCameraZoom = 2.83f;

    void Start()
    {
        gameState = GameState.Midgard;
    }

    [ContextMenu("ChangeGameState")]
    public void ChangeGameState()
    {
        if (gameState == GameState.Midgard)
        {
            gameState = GameState.Helheims;
            Debug.Log("Changing gamestate to " + gameState);
            StartCoroutine(ChangeWorld(GameState.Helheims));
        }
        else
        {
            gameState = GameState.Midgard;
            Debug.Log("Changing gamestate to " + gameState);
            StartCoroutine(ChangeWorld(GameState.Midgard));
        }
    }

    private IEnumerator ChangeWorld(GameState targetGameState)
    {
        Debug.Log("Changing world to " + targetGameState);
        // Start fly-up animation'

        yield return new WaitForSeconds(2);

        // Zoom out fully before rotating
        float originalZoom = OrignalCameraZoom;
        float targetZoom = 9.85f;
        float zoomSpeed = 4f;

        Debug.Log("Zooming out");
        while (Mathf.Abs(Camera.main.orthographicSize - targetZoom) > 0.05f)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
            yield return null;
        }
        Camera.main.orthographicSize = targetZoom; // Snap to the exact target zoom
        Debug.LogWarning("Zooming out complete");

        // Start rotation after fully zooming out
        yield return RotateWorld(targetGameState);  // Wait for rotation to complete

        // Zoom back in after rotation is complete
        Debug.Log("Zooming in");
        while (Mathf.Abs(Camera.main.orthographicSize - originalZoom) > 0.05f)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, originalZoom, Time.deltaTime * zoomSpeed);
            yield return null;
        }
        Camera.main.orthographicSize = originalZoom; // Snap to the exact original zoom

        yield return new WaitForSeconds(1);
    }



    private IEnumerator RotateWorld(GameState targetGameState)
    {
        // Determine the target rotation angle based on the target game state
        float rotationSpeed = 300f;
        float targetRotation = (targetGameState == GameState.Helheims) ? 180f : 0f;

        Debug.Log("Rotating world");

        while (true)
        {
            // Calculate the current Y angle and the shortest angle difference to the target
            float currentYRotation = world.transform.rotation.eulerAngles.y;
            float angleDifference = Mathf.DeltaAngle(currentYRotation, targetRotation);

            // If the angle difference is within a small range, stop rotating
            if (Mathf.Abs(angleDifference) < 0.5f)
            {
                world.transform.rotation = Quaternion.Euler(0, targetRotation, 0);
                break;
            }

            // Rotate towards the target at the specified speed
            float rotationStep = rotationSpeed * Time.deltaTime * Mathf.Sign(angleDifference);
            world.transform.Rotate(Vector3.up, rotationStep);

            yield return null;
        }

        Debug.Log("Rotation complete");
    }

}
