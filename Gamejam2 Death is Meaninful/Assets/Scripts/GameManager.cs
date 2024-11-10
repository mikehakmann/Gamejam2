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

    private Animator HpBarAnim;
    public CinemachineCamera cinemachineCamera; // Using CinemachineCamera as specified
    public Light2D sceneLight;
    private Color midgardLightColor = Color.white;
    private Color helheimsLightColor = Color.gray * 0.3f;

    private float originalCameraZoom = 4.67f;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gameState = GameState.Midgard;
        HpBarAnim = GameObject.Find("Hp").GetComponent<Animator>();
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

        PlayFlyUpOnAllTargets();  // Play "FlyUp" animation on all relevant objects
    }

    private void PlayFlyUpOnAllTargets()
    {
        // Get all GameObjects with TeleportToMidgaard and EnemyDrop components
        TeleportToMidgaard[] teleporters = FindObjectsOfType<TeleportToMidgaard>();
        EnemyDrop[] enemies = FindObjectsOfType<EnemyDrop>();

        // Play "FlyUp" animation on all relevant objects with an Animator
        foreach (var teleporter in teleporters)
        {
            Animator animator = teleporter.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play("FlyUp");
            }
        }

        foreach (var enemy in enemies)
        {
            Animator animator = enemy.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play("FlyUp");
            }
        }
    }
    
    private void PlayFlyUpDownAllTargets()
    {
        // Get all GameObjects with TeleportToMidgaard and EnemyDrop components
        TeleportToMidgaard[] teleporters = FindObjectsOfType<TeleportToMidgaard>();
        EnemyDrop[] enemies = FindObjectsOfType<EnemyDrop>();

        // Play "FlyDown" animation on all relevant objects with an Animator
        foreach (var teleporter in teleporters)
        {
            Animator animator = teleporter.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play("FlyDown");
            }
        }

        foreach (var enemy in enemies)
        {
            Animator animator = enemy.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play("FlyDown");
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
        
        Debug.Log("Zooming out");
        while (Mathf.Abs(cinemachineCamera.Lens.OrthographicSize - targetZoom) > 0.05f)
        {
            cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(cinemachineCamera.Lens.OrthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
            yield return null;
        }
        cinemachineCamera.Lens.OrthographicSize = targetZoom; // Snap to the exact target zoom
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
        PlayFlyUpDownAllTargets();
        yield return new WaitForSeconds(0.45f);
        CameraManager.Instance.Shake();

        if(targetGameState == GameState.Helheims)
        {
        HpBarAnim.Play("HelheimTimer");
        }
        else
        {
         HpBarAnim.Play("MidgardTimer");
        }
        ResumeMovement();

        ChangeStateOnAllEnemyDrops();  // Change state on all EnemyDrops after ResumeMovement
    }

    private void ChangeStateOnAllEnemyDrops()
    {
        EnemyDrop[] enemies = FindObjectsOfType<EnemyDrop>();
        foreach (var enemy in enemies)
        {
            enemy.ChangeState();
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
