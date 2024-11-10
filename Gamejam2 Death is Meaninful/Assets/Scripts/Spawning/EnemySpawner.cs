using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;                      // Prefab for the regular enemy
    public float initialEnemySpawnInterval = 2f;        // Initial spawn interval for the regular enemy
    private float enemySpawnInterval;                   // Adjusted spawn interval for regular enemies
    public int initialMaxEnemyCount = 10;               // Initial max count for regular enemies
    private int maxEnemyCount;                          // Adjusted max count based on difficulty

    [Header("RunAwayEnemy Settings")]
    public GameObject runAwayEnemyPrefab;               // Prefab for the RunAwayEnemy
    public float initialRunAwayEnemySpawnInterval = 3f; // Initial spawn interval for the RunAwayEnemy
    private float runAwayEnemySpawnInterval;            // Adjusted spawn interval for RunAwayEnemies
    public int initialMaxRunAwayCount = 5;              // Initial max count for RunAwayEnemies
    private int maxRunAwayCount;                        // Adjusted max count based on difficulty

    [Header("Difficulty Multipliers")]
    public float regularEnemyCountMultiplier = 1.1f;    // Multiplier to increase max count of regular enemies
    public float regularEnemySpawnRateMultiplier = 0.95f; // Multiplier to decrease spawn interval for regular enemies
    public float runAwayEnemyCountMultiplier = 1.2f;    // Multiplier to increase max count of RunAwayEnemies
    public float runAwayEnemySpawnRateMultiplier = 0.9f;  // Multiplier to decrease spawn interval for RunAwayEnemies
    private int difficultyCycle = 1;                    // Tracks difficulty cycles

    private Camera mainCamera;                           // Camera for spawning enemies

    private float enemySpawnTimer = 0f;                 // Timer for regular enemy spawning
    private float runAwayEnemySpawnTimer = 0f;          // Timer for RunAwayEnemy spawning
    private int currentEnemyCount = 0;                  // Current count of regular enemies
    private int currentRunAwayCount = 0;                // Current count of RunAwayEnemies

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;                   // Use main camera if none is assigned
    }

    void OnEnable()
    {
        // Reset timers and counters when spawner is re-enabled
        enemySpawnTimer = 0f;
        runAwayEnemySpawnTimer = 0f;
        currentEnemyCount = 0;
        currentRunAwayCount = 0;

        // Adjust spawn intervals and max counts based on difficulty cycles
        maxEnemyCount = Mathf.CeilToInt(initialMaxEnemyCount * Mathf.Pow(regularEnemyCountMultiplier, difficultyCycle - 1));
        enemySpawnInterval = initialEnemySpawnInterval * Mathf.Pow(regularEnemySpawnRateMultiplier, difficultyCycle - 1);

        maxRunAwayCount = Mathf.CeilToInt(initialMaxRunAwayCount * Mathf.Pow(runAwayEnemyCountMultiplier, difficultyCycle - 1));
        runAwayEnemySpawnInterval = initialRunAwayEnemySpawnInterval * Mathf.Pow(runAwayEnemySpawnRateMultiplier, difficultyCycle - 1);

        difficultyCycle++;  // Increment for next cycle
    }

    void Update()
    {
        // Update spawn timers
        enemySpawnTimer += Time.deltaTime;
        runAwayEnemySpawnTimer += Time.deltaTime;

        // Spawn a regular enemy if the timer exceeds the interval and the count is below max
        if (enemySpawnTimer >= enemySpawnInterval && currentEnemyCount < maxEnemyCount)
        {
            SpawnEnemy(enemyPrefab, ref currentEnemyCount);
            enemySpawnTimer = 0f;                       // Reset timer for regular enemy
        }

        // Spawn a RunAwayEnemy if the timer exceeds the interval and the count is below max
        if (runAwayEnemySpawnTimer >= runAwayEnemySpawnInterval && currentRunAwayCount < maxRunAwayCount)
        {
            SpawnEnemy(runAwayEnemyPrefab, ref currentRunAwayCount);
            runAwayEnemySpawnTimer = 0f;                // Reset timer for RunAwayEnemy
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

    void OnDisable()
    {
        // Reset to initial state on disable
        enemySpawnTimer = 0f;
        runAwayEnemySpawnTimer = 0f;
        currentEnemyCount = 0;
        currentRunAwayCount = 0;
    }
}
