using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración de Enemigos")]
    public GameObject[] enemyPrefabs; 
    public GameObject bossPrefab;
    
    [Header("Configuración de Spawn Frontal y Alturas")]
    public Transform playerTransform;    
    public float minSpawnDistance = 12f;  
    public float maxSpawnDistance = 25f;  
    public float spawnSpreadX = 12f;     
    
    [Header("Variación de Alturas (Eje Y)")]
    public float minHeight = 0f;         // Altura mínima (ej. nivel del suelo)
    public float maxHeight = 6f;         // Altura máxima (ej. enemigos elevados o voladores)

    public float checkRadius = 2f; 
    
    [Header("Progresión")]
    public int enemiesToKillForBoss = 10;
    private int totalEnemiesKilled = 0;
    private bool bossSpawned = false;
    private float timer = 0f;
    public float spawnInterval = 3f;

    void Start()
    {
        if (playerTransform == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }
    }

    void Update()
    {
        if (bossSpawned || playerTransform == null) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        for (int i = 0; i < 15; i++)
        {
            float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 forwardOffset = playerTransform.forward * randomDistance;
            
            // Dispersión horizontal y aleatoriedad en la altura (Eje Y)
            Vector3 randomSpread = new Vector3(Random.Range(-spawnSpreadX, spawnSpreadX), 0, Random.Range(-2f, 2f));
            Vector3 spawnPoint = playerTransform.position + forwardOffset + randomSpread;
            
            // Asigna una altura aleatoria dentro del rango configurado
            spawnPoint.y = playerTransform.position.y + Random.Range(minHeight, maxHeight);

            // Verifica que el punto esté libre de obstáculos
            if (!Physics.CheckSphere(spawnPoint, checkRadius))
            {
                int randomIndex = Random.Range(0, enemyPrefabs.Length);
                Instantiate(enemyPrefabs[randomIndex], spawnPoint, Quaternion.identity);
                return;
            }
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
        bossSpawnPoint.y = playerTransform.position.y + minHeight; // El Boss aparece a una altura base segura
        
        Instantiate(bossPrefab, bossSpawnPoint, Quaternion.identity);
        Debug.Log("¡El Boss ha aparecido!");
    }
}