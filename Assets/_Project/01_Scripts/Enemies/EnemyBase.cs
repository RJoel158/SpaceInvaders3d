using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] protected float maxHealth = 50f;
    [SerializeField] protected float moveSpeed = 10f;
    
    [Header("Effects")]
    [Tooltip("Optional: Particle system to spawn on death")]
    [SerializeField] private GameObject explosionPrefab;

    protected float currentHealth;
    protected bool isDead = false; // Bandera para evitar bucles

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        if (isDead) return;

        Move();
        CheckOffScreen();
    }

    protected virtual void Move()
    {
        transform.position += Vector3.back * moveSpeed * Time.deltaTime;
    }

    public virtual void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // 🔒 BLOQUEO TOTAL: Si ya está muerto, se corta aquí y no se repite nunca más
        if (isDead) return;
        isDead = true;

        // Avisar al Spawner para que cuente la baja
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.RegisterKill();
        }

        // Instancia la explosión exactamente una sola vez
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject);
    }

    private void CheckOffScreen()
    {
        if (transform.position.z < -20f || transform.position.y < -20f)
        {
            Destroy(gameObject);
        }
    }
}