using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;          // Prefab for the regular enemy
    public float enemySpawnInterval = 2f;   // Spawn interval for the regular enemy
    public int maxEnemyCount = 10;          // Max number of regular enemies

    [Header("RunAwayEnemy Settings")]
    public GameObject runAwayEnemyPrefab;   // Prefab for the RunAwayEnemy
    public float runAwaySpawnInterval = 3f; // Spawn interval for the RunAwayEnemy
    public int maxRunAwayCount = 5;         // Max number of RunAwayEnemies

    public Camera mainCamera;               // Camera in which to spawn enemies

    private float enemySpawnTimer = 0f;     // Timer to control regular enemy spawning
    private float runAwaySpawnTimer = 0f;   // Timer to control RunAwayEnemy spawning
    private int currentEnemyCount = 0;      // Current count of regular enemies
    private int currentRunAwayCount = 0;    // Current count of RunAwayEnemies

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;       // Use main camera if none is assigned
    }

    void Update()
    {
        // Update spawn timers
        enemySpawnTimer += Time.deltaTime;
        runAwaySpawnTimer += Time.deltaTime;

        // Check if it's time to spawn a regular enemy
        if (enemySpawnTimer >= enemySpawnInterval && currentEnemyCount < maxEnemyCount)
        {
            SpawnEnemy(enemyPrefab, ref currentEnemyCount);
            enemySpawnTimer = 0f;           // Reset the timer for regular enemy
        }

        // Check if it's time to spawn a RunAwayEnemy
        if (runAwaySpawnTimer >= runAwaySpawnInterval && currentRunAwayCount < maxRunAwayCount)
        {
            SpawnEnemy(runAwayEnemyPrefab, ref currentRunAwayCount);
            runAwaySpawnTimer = 0f;         // Reset the timer for RunAwayEnemy
        }
    }

    void SpawnEnemy(GameObject prefab, ref int currentCount)
    {
        // Get random spawn position within the camera's view
        Vector3 screenPosition = new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), mainCamera.nearClipPlane);
        Vector3 spawnPosition = mainCamera.ScreenToWorldPoint(screenPosition);

        // Set the z-position to 0 to ensure the enemy spawns at the correct depth
        spawnPosition.z = 0f;

        // Spawn the enemy and increment the count
        Instantiate(prefab, spawnPosition, Quaternion.identity);
        currentCount++;
    }

    // Decrease the enemy count when an enemy is destroyed
    public void OnEnemyDestroyed(GameObject enemy)
    {
        // Check if the destroyed enemy is of type Enemy or RunAwayEnemy
        if (enemyPrefab != null && enemyPrefab.name == enemy.name)
        {
            currentEnemyCount--;
        }
        else if (runAwayEnemyPrefab != null && runAwayEnemyPrefab.name == enemy.name)
        {
            currentRunAwayCount--;
        }
    }
}
