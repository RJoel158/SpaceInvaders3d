using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración de Enemigos")]
    public GameObject[] enemyPrefabs; 
    public GameObject bossPrefab;
    
    [Header("Configuración de Asteroides")]
    public GameObject[] asteroidPrefabs; // Arrastra aquí tus asteroides
    public float asteroidSpawnInterval = 5f; // Cada cuánto tiempo aparecen

    [Header("Configuración de Spawn Frontal y Alturas")]
    public Transform playerTransform;    
    public float minSpawnDistance = 12f;  
    public float maxSpawnDistance = 25f;  
    public float spawnSpreadX = 12f;     
    
    [Header("Variación de Alturas (Eje Y)")]
    public float minHeight = 0f;          // Altura mínima (ej. nivel del suelo)
    public float maxHeight = 6f;          // Altura máxima (ej. enemigos elevados o voladores)

    public float checkRadius = 2f; 
    
    [Header("Progresión")]
    public int enemiesToKillForBoss = 10;
    private int totalEnemiesKilled = 0;
    private bool bossSpawned = false;
    
    private float enemyTimer = 0f;
    public float spawnInterval = 3f;

    void Start()
    {
        if (playerTransform == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }

        // Iniciar la rutina independiente para los asteroides cada 5 segundos
        StartCoroutine(SpawnAsteroidsRoutine());
    }

    void Update()
    {
        if (bossSpawned || playerTransform == null) return;

        // Control de spawn de enemigos normales por tiempo
        enemyTimer += Time.deltaTime;
        if (enemyTimer >= spawnInterval)
        {
            SpawnEnemy();
            enemyTimer = 0f;
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        for (int i = 0; i < 15; i++)
        {
            float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 forwardOffset = playerTransform.forward * randomDistance;
            
            Vector3 randomSpread = new Vector3(Random.Range(-spawnSpreadX, spawnSpreadX), 0, Random.Range(-2f, 2f));
            Vector3 spawnPoint = playerTransform.position + forwardOffset + randomSpread;
            
            spawnPoint.y = playerTransform.position.y + Random.Range(minHeight, maxHeight);

            if (!Physics.CheckSphere(spawnPoint, checkRadius))
            {
                int randomIndex = Random.Range(0, enemyPrefabs.Length);
                Instantiate(enemyPrefabs[randomIndex], spawnPoint, Quaternion.identity);
                return;
            }
        }
    }

    IEnumerator SpawnAsteroidsRoutine()
    {
        // Espera inicial antes de empezar a tirar asteroides (opcional)
        yield return new WaitForSeconds(2f);

        while (!bossSpawned && playerTransform != null)
        {
            yield return new WaitForSeconds(asteroidSpawnInterval);
            SpawnAsteroid();
        }
    }

   void SpawnAsteroid()
    {
        if (asteroidPrefabs == null || asteroidPrefabs.Length == 0 || playerTransform == null) return;

        int randomIndex = Random.Range(0, asteroidPrefabs.Length);
        GameObject selectedAsteroid = asteroidPrefabs[randomIndex];

        // 1. Posición aleatoria frente al jugador
        float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
        Vector3 forwardOffset = playerTransform.forward * randomDistance;
        Vector3 randomSpread = new Vector3(Random.Range(-spawnSpreadX, spawnSpreadX), 0, 0);
        
        Vector3 asteroidSpawnPoint = playerTransform.position + forwardOffset + randomSpread;
        asteroidSpawnPoint.y = playerTransform.position.y + Random.Range(minHeight, maxHeight);

        // 2. Instancia el asteroide
        GameObject asteroidInstance = Instantiate(selectedAsteroid, asteroidSpawnPoint, Random.rotation);

        // 3. Calcula la dirección recta hacia donde está el jugador (o hacia adelante de la zona de juego)
        // Si quieres que crucen directamente hacia la posición actual del jugador al momento de nacer:
        Vector3 fallDirection = (playerTransform.position - asteroidSpawnPoint).normalized;

        if (asteroidInstance.TryGetComponent<AsteroidMovement>(out var movement))
        {
            movement.Initialize(fallDirection);
        }
    }

    public void RegisterKill()
    {
        totalEnemiesKilled++;
        if (totalEnemiesKilled >= enemiesToKillForBoss && !bossSpawned)
        {
            SpawnBoss();
        }
    }

    void SpawnBoss()
    {
        bossSpawned = true;
        Vector3 bossSpawnPoint = playerTransform.position + (playerTransform.forward * minSpawnDistance);
        bossSpawnPoint.y = playerTransform.position.y + minHeight; 
        
        // Calcula la dirección hacia el jugador para que no aparezca con la cola hacia adelante
        Vector3 lookDirection = (playerTransform.position - bossSpawnPoint).normalized;
        lookDirection.y = 0; 

        Instantiate(bossPrefab, bossSpawnPoint, Quaternion.LookRotation(lookDirection));
        Debug.Log("¡El Boss ha aparecido!");
    }
}