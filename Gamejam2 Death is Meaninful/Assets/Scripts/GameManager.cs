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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameState = GameState.Midgard;
    }

    // Update is called once per frame
    void Update()
    {
        
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


    /*<summary>
     * Start fly up anim
     * Zoom out 
     * rotate world
     * Zoom in
     * Start Fly down anim
     * execute ChangeGameState
     * excute spawn upgrades and stairway
     * </summary>
     */
    private IEnumerator ChangeWorld(GameState gameState)
    {
        Debug.Log("Changing world to " + gameState);
        // Start fly-up animation
        yield return new WaitForSeconds(2);

        // Zoom out
        float originalZoom = OrignalCameraZoom;
        float targetZoom = 9.85f;
        float zoomSpeed = 4f;
        bool rotationStarted = false;

        Debug.Log("Zooming out");
        while (Camera.main.orthographicSize <= targetZoom - 0.20f)
        {
            // Lerp the camera's orthographic size toward the target zoom level
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);

            // Check if we've reached 50% of the zoom transition
            if (!rotationStarted && Camera.main.orthographicSize >= (originalZoom + targetZoom) / 2)
            {
                rotationStarted = true;
                StartCoroutine(RotateWorld());
            }

            yield return null;
        }


        Debug.Log("Zooming in");
        while (Camera.main.orthographicSize >= 2.83f)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 2.8f, Time.deltaTime * zoomSpeed);
            yield return null;
        }


        yield return new WaitForSeconds(1);
    }

    private IEnumerator RotateWorld()
    {
        // Rotate world with the specified speed until reaching 180 degrees on the Y-axis
        float rotationSpeed = 300f;
        Debug.Log("Rotating world");

        while (world.transform.rotation.eulerAngles.y < 180)
        {
            world.transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed);
            yield return null;
        }
    }
}
