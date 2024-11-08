using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;        // Prefab of the enemy to spawn
    public Camera mainCamera;             // The camera in which to spawn enemies
    public float spawnInterval = 2f;      // Interval between spawns
    public int maxEnemies = 10;           // Max number of enemies that can exist at once

    private float spawnTimer = 0f;
    private int currentEnemyCount = 0;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;     // Automatically use the main camera if not set
    }

    void Update()
    {
        // Increment the timer
        spawnTimer += Time.deltaTime;

        // Check if it's time to spawn a new enemy and if we haven't hit the max count
        if (spawnTimer >= spawnInterval && currentEnemyCount < maxEnemies)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    void SpawnEnemy()
    {
        // Get the bounds of the camera view
        Vector3 screenPosition = new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), mainCamera.nearClipPlane);
        Vector3 spawnPosition = mainCamera.ScreenToWorldPoint(screenPosition);

        // Spawn the enemy
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        currentEnemyCount++;
    }

    // Optional: call this method from other scripts to decrease the enemy count when an enemy is destroyed
    public void OnEnemyDestroyed()
    {
        currentEnemyCount--;
    }
}
