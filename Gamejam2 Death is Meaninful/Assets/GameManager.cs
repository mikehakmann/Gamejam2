using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private float OrignalCameraZoom = 2.83f;

    private List<EnemyHealth> enemiesInScene = new List<EnemyHealth>();  // List to keep track of enemies

    private void Awake()
    {
        // Singleton pattern enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
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
        player.GetComponent<Animator>().Play("FlyUp");
        LockMovement();
        yield return new WaitForSeconds(2);

        float originalZoom = OrignalCameraZoom;
        float targetZoom = 9.85f;
        float zoomSpeed = 4f;

        Debug.Log("Zooming out");
        while (Mathf.Abs(Camera.main.orthographicSize - targetZoom) > 0.05f)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
            yield return null;
        }
        Camera.main.orthographicSize = targetZoom;
        Debug.LogWarning("Zooming out complete");

        yield return RotateWorld(targetGameState);

        Debug.Log("Zooming in");
        while (Mathf.Abs(Camera.main.orthographicSize - originalZoom) > 0.05f)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, originalZoom, Time.deltaTime * zoomSpeed);
            yield return null;
        }
        Camera.main.orthographicSize = originalZoom;

        player.GetComponent<Animator>().Play("FlyDown");
        yield return new WaitForSeconds(1);
        ResumeMovement();
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
