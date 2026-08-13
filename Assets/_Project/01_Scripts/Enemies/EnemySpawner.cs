using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración de Enemigos")]
    public GameObject[] enemyPrefabs; 
    public GameObject bossPrefab;
    
    [Header("Configuración de Spawn Frontal")]
    public Transform playerTransform;    
    public float minSpawnDistance = 12f;  // Distancia mínima: Nunca aparecerán más cerca que esto
    public float maxSpawnDistance = 20f;  // Distancia máxima hacia adelante
    public float spawnSpreadX = 12f;     // Ancho de dispersión a los lados
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
            // Calcula una distancia aleatoria entre el mínimo y el máximo seguro enfrente del jugador
            float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 forwardOffset = playerTransform.forward * randomDistance;
            
            Vector3 randomSpread = new Vector3(Random.Range(-spawnSpreadX, spawnSpreadX), 0, Random.Range(-2f, 2f));
            Vector3 spawnPoint = playerTransform.position + forwardOffset + randomSpread;
            spawnPoint.y = 0; 

            // Verifica que el punto esté libre de otros colliders
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
        bossSpawnPoint.y = 0;
        
        Instantiate(bossPrefab, bossSpawnPoint, Quaternion.identity);
        Debug.Log("¡El Boss ha aparecido frente a ti!");
    }
}   