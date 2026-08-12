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

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        Move();
        CheckOffScreen();
    }

    /// <summary>
    /// Default movement: straight down the Y axis (or -Z depending on your camera setup).
    /// Assumes top-down view where enemies move down the screen (negative Y or Z).
    /// </summary>
    protected virtual void Move()
    {
        // Change Vector3.back to Vector3.down if your game is vertically oriented on Y instead of Z
        transform.position += Vector3.back * moveSpeed * Time.deltaTime;
    }

    public virtual void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        
        // Add score logic here if needed
        Destroy(gameObject);
    }

    /// <summary>
    /// Destroys the enemy if it goes too far past the player to save memory.
    /// </summary>
    private void CheckOffScreen()
    {
        // Adjust this threshold based on your game's coordinates
        if (transform.position.z < -20f || transform.position.y < -20f)
        {
            Destroy(gameObject);
        }
    }
}