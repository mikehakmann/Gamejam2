using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum GameState
{
    Midgard,
    Helheims
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }  // Singleton instance

    public GameState gameState;
    public GameObject world;
    public GameObject player;

    public CinemachineCamera cinemachineCamera; // Change Camera to CinemachineVirtualCamera
    public Light2D sceneLight;
    private Color midgardLightColor = Color.white;
    private Color helheimsLightColor = Color.gray * 0.3f;

    private float originalCameraZoom = 2.83f;

    private List<EnemyHealth> enemiesInScene = new List<EnemyHealth>();  // List to keep track of enemies



    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gameState = GameState.Midgard;
        CacheEnemiesInScene();  // Store references to all enemies in the scene at the start
    }

    private void CacheEnemiesInScene()
    {
        // Find all objects with EnemyHealth in the current scene and add to list
        enemiesInScene.Clear();
        enemiesInScene.AddRange(FindObjectsOfType<EnemyHealth>());
    }

    [ContextMenu("ChangeGameState")]
    public void ChangeGameState()
    {
        if (gameState == GameState.Midgard)
        {
            gameState = GameState.Helheims;
            Debug.Log("Changing gamestate to " + gameState);
            StartCoroutine(ChangeWorld(GameState.Helheims));
            ToggleEnemies(false);  // Disable all enemies
        }
        else
        {
            gameState = GameState.Midgard;
            Debug.Log("Changing gamestate to " + gameState);
            StartCoroutine(ChangeWorld(GameState.Midgard));
            ToggleEnemies(true);  // Enable all enemies
        }
    }

    private void ToggleEnemies(bool enable)
    {
        foreach (var enemy in enemiesInScene)
        {
            if (enemy != null)
            {
                enemy.gameObject.SetActive(enable);  // Enable or disable enemy GameObjects
            }
        }
    }

    private IEnumerator ChangeWorld(GameState targetGameState)
    {
        Debug.Log("Changing world to " + targetGameState);
        // Start fly-up animation
        player.GetComponent<Animator>().Play("FlyUp");
        LockMovement();
        yield return new WaitForSeconds(1);

        // Zoom out fully before rotating
        float targetZoom = 30f;
        float zoomSpeed = 4f;
        //ZOOM ------------------------------------------------------------
        Debug.Log("Zooming out");
        while (Mathf.Abs(cinemachineCamera.Lens.OrthographicSize - targetZoom) > 0.05f)
        {
            cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(cinemachineCamera.Lens.OrthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
            yield return null;
        }
        cinemachineCamera.Lens.OrthographicSize = targetZoom; // Snap to the exact target zoom
        Debug.LogWarning("Zooming out complete");
        //------------------------------------------------------------------
        // Start rotation after fully zooming out
        yield return RotateWorld(targetGameState);  // Wait for rotation to complete


        // lIGHT ----------------------------------------------------------
        Debug.Log("Changing light color");
        Color targetColor = targetGameState == GameState.Helheims ? helheimsLightColor : midgardLightColor;
        float colorTransitionSpeed = 4f;
        float colorTolerance = 0.01f; // Margin for color difference

        while (Vector3.Distance(new Vector3(sceneLight.color.r, sceneLight.color.g, sceneLight.color.b),
                                new Vector3(targetColor.r, targetColor.g, targetColor.b)) > colorTolerance)
        {
            sceneLight.color = Color.Lerp(sceneLight.color, targetColor, Time.deltaTime * colorTransitionSpeed);
            yield return null;
        }
        sceneLight.color = targetColor; // Snap to the exact target color

        //------------------------------------------------------------------
        // Zoom in after rotating and changing light color

        Debug.Log("Zooming in");
        while (Mathf.Abs(cinemachineCamera.Lens.OrthographicSize - originalCameraZoom) > 0.05f)
        {
            cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(cinemachineCamera.Lens.OrthographicSize, originalCameraZoom, Time.deltaTime * zoomSpeed);
            yield return null;
        }
        cinemachineCamera.Lens.OrthographicSize = originalCameraZoom; // Snap to the exact original zoom

        player.GetComponent<Animator>().Play("FlyDown");
        yield return new WaitForSeconds(0.45f);
        CameraManager.Instance.Shake();
        ResumeMovement();
        //------------------------------------------------------------------



        if (gameState == GameState.Midgard)
        {
            gameState = GameState.Helheims;
            Debug.Log("Changing gamestate to " + gameState);
        }
        else
        {
            gameState = GameState.Midgard;
            Debug.Log("Changing gamestate to " + gameState);
        }
    }


    private IEnumerator RotateWorld(GameState targetGameState)
    {
        float rotationSpeed = 300f;
        float targetRotation = (targetGameState == GameState.Helheims) ? 180f : 0f;

        Debug.Log("Rotating world");

        while (true)
        {
            float currentYRotation = world.transform.rotation.eulerAngles.y;
            float angleDifference = Mathf.DeltaAngle(currentYRotation, targetRotation);

            if (Mathf.Abs(angleDifference) < 0.5f)
            {
                world.transform.rotation = Quaternion.Euler(0, targetRotation, 0);
                break;
            }

            float rotationStep = rotationSpeed * Time.deltaTime * Mathf.Sign(angleDifference);
            world.transform.Rotate(Vector3.up, rotationStep);

            yield return null;
        }

        Debug.Log("Rotation complete");
    }

    public void LockMovement()
    {
        player.GetComponent<Player>().enabled = false;
        player.GetComponent<PlayerActions>().enabled = false;
    }

    public void ResumeMovement()
    {
        player.GetComponent<Player>().enabled = true;
        player.GetComponent<PlayerActions>().enabled = true;
    }
}
