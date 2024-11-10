using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;
using Unity.Cinemachine;

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
    public GameObject EnemySpawner;
    
    public CinemachineCamera cinemachineCamera;  // Using CinemachineCamera as specified
    public Light2D sceneLight;
    private Color midgardLightColor = Color.white;
    private Color helheimsLightColor = Color.gray * 0.3f;

    private float originalCameraZoom;
    
    public PlayerHealth playerHealth;

    public List<Upgrade> upgradesAvailiable = new List<Upgrade>();

    private void Awake()
    {
        Instance = this;
    }

    public int timesKilled = 0;

    void Start()
    {
        originalCameraZoom = cinemachineCamera.Lens.OrthographicSize;
        gameState = GameState.Midgard;
        
        player = GameObject.Find("Player");
        
        playerHealth  = player.GetComponent<PlayerHealth>();
        
    }

    [ContextMenu("ChangeGameState")]
    public void ChangeGameState()
    {
        DestroyAllEnemyHealthObjects();

        if (gameState == GameState.Midgard)
        {
            gameState = GameState.Helheims;
            EnemySpawner.SetActive(false);
            StartCoroutine(ChangeWorld(GameState.Helheims));
        }
        else
        {
            gameState = GameState.Midgard;
            EnemyDrop[] allEnemiesDrops = FindObjectsOfType<EnemyDrop>();
            foreach (EnemyDrop enemyDrop in allEnemiesDrops)
            {
                Destroy(enemyDrop.gameObject);
            }
            EnemySpawner.SetActive(false);
            StartCoroutine(ChangeWorld(GameState.Midgard));
        }
        
        Debug.Log("Changing gamestate to " + gameState);
    }

    private void DestroyAllEnemyHealthObjects()
    {
        EnemyHealth[] allEnemies = FindObjectsOfType<EnemyHealth>();
        foreach (EnemyHealth enemy in allEnemies)
        {
            Destroy(enemy.gameObject);
        }
    }

    private IEnumerator ChangeWorld(GameState targetGameState)
    {
        Debug.Log("Changing world to " + targetGameState);
        
        player.GetComponent<Animator>().Play("FlyUp");
        
        EnemyDrop[] allEnemiesDrops = FindObjectsOfType<EnemyDrop>();
        foreach (EnemyDrop enemyDrop in allEnemiesDrops)
        {
            enemyDrop.gameObject.GetComponent<Animator>().Play("FlyUp");
        }
        
        TeleportToMidgaard[] allTeleporters = FindObjectsOfType<TeleportToMidgaard>();
        foreach (TeleportToMidgaard teleporter in allTeleporters)
        {
            Animator animator = teleporter.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play("FlyUp");
            }
        }
        
        LockMovement();
        yield return new WaitForSeconds(1);

        float targetZoom = 30f;
        float zoomSpeed = 4f;
        
        Debug.Log("Zooming out");
        while (Mathf.Abs(cinemachineCamera.Lens.OrthographicSize - targetZoom) > 0.05f)
        {
            cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(cinemachineCamera.Lens.OrthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
            yield return null;
        }
        cinemachineCamera.Lens.OrthographicSize = targetZoom;
        Debug.LogWarning("Zooming out complete");

        yield return RotateWorld(targetGameState);

        Debug.Log("Changing light color");
        Color targetColor = targetGameState == GameState.Helheims ? helheimsLightColor : midgardLightColor;
        float colorTransitionSpeed = 4f;
        float colorTolerance = 0.01f;

        while (Vector3.Distance(new Vector3(sceneLight.color.r, sceneLight.color.g, sceneLight.color.b),
                                new Vector3(targetColor.r, targetColor.g, targetColor.b)) > colorTolerance)
        {
            sceneLight.color = Color.Lerp(sceneLight.color, targetColor, Time.deltaTime * colorTransitionSpeed);
            yield return null;
        }
        sceneLight.color = targetColor;

        Debug.Log("Zooming in");
        while (Mathf.Abs(cinemachineCamera.Lens.OrthographicSize - originalCameraZoom) > 0.05f)
        {
            cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(cinemachineCamera.Lens.OrthographicSize, originalCameraZoom, Time.deltaTime * zoomSpeed);
            yield return null;
        }
        cinemachineCamera.Lens.OrthographicSize = originalCameraZoom;

        player.GetComponent<Animator>().Play("FlyDown");
        
        foreach (EnemyDrop enemyDrop in allEnemiesDrops)
        {
            if (enemyDrop == null)
            {
                continue;
            }
            enemyDrop.gameObject.GetComponent<Animator>().Play("FlyDown");
        }
        
        foreach (TeleportToMidgaard teleporter in allTeleporters)
        {
            if (teleporter == null)
            {
                continue;
            }
            teleporter.gameObject.GetComponent<Animator>().Play("FlyDown");
        }
        
        yield return new WaitForSeconds(0.45f);
        CameraManager.Instance.Shake();
        
        // Start the DamageOverTime coroutine at "/Here"
        if (gameState == GameState.Helheims)
        {
            player.SetActive(true);

            playerHealth.ToggleDamageOverTime(true);
            
            EnemyDrop[] allPowerUps = FindObjectsOfType<EnemyDrop>();
            foreach (EnemyDrop powerUp in allPowerUps)
            {
                powerUp.ChangeState();
            }



            
            

            PlayerActions playerActions = player.GetComponent<PlayerActions>();
            if (playerHealth != null && playerActions != null)
            {
                playerHealth.SetHealth(playerActions.maxHP);
            }
            
        }
        
        
        
        if (gameState == GameState.Midgard)
        {
            playerHealth.ToggleDamageOverTime(true);
        }
        
        
        
        
        
        ResumeMovement();
        
        playerHealth.ToggleDamageOverTime(true);
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
