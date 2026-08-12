using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs; // Add Kamikaze and Sniper here
    [SerializeField] private GameObject bossPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnWidth = 20f; // Horizontal range for spawning
    [SerializeField] private float spawnZPosition = 80f; // Distance deep into the screen

    [Header("Wave Settings")]
    [SerializeField] private int enemiesBeforeBoss = 20;

    private int enemiesSpawned = 0;
    private bool bossSpawned = false;

    private void Start()
    {
        StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        while (!bossSpawned)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (enemiesSpawned >= enemiesBeforeBoss)
            {
                SpawnBoss();
                yield break; // Stop standard spawning
            }

            SpawnRandomEnemy();
        }
    }

    private void SpawnRandomEnemy()
    {
        if (enemyPrefabs.Length == 0) return;

        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject selectedPrefab = enemyPrefabs[randomIndex];

        // Random X position
        float randomX = Random.Range(-spawnWidth / 2f, spawnWidth / 2f);
        Vector3 spawnPos = new Vector3(randomX, 0f, spawnZPosition); // Adjust Y to your ship's level

        Instantiate(selectedPrefab, spawnPos, Quaternion.Euler(0, 180, 0)); // Rotated to face player
        
        enemiesSpawned++;
    }

    private void SpawnBoss()
    {
        if (bossPrefab == null) return;
        bossSpawned = true;
        
        Vector3 spawnPos = new Vector3(0f, 0f, spawnZPosition + 20f);
        Instantiate(bossPrefab, spawnPos, Quaternion.Euler(0, 180, 0));
    }
}