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
    protected bool isDead = false; // Bandera para evitar bucles de muerte

    // virtual: permite que un hijo pueda modificar el Start si lo necesita
    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    // virtual: BossEnemy usa override Update para su máquina de estados
    protected virtual void Update()
    {
        if (isDead) return;

        Move();
        CheckOffScreen();
    }

    // virtual: Kamikaze y Sniper usan override Move para cambiar cómo se mueven
    protected virtual void Move()
    {
        // Se mueve hacia atrás en el eje Z (hacia el jugador)
        transform.position += Vector3.back * moveSpeed * Time.deltaTime;
    }

    // virtual: BossEnemy usa override TakeDamage para evitar daño cuando ya está muriendo
    public virtual void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // virtual: BossEnemy usa override Die para hacer cosas extras antes de destruirse
    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        // --- NUEVO: Avisar al Spawner para que cuente la baja y ver si sale el Boss ---
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.RegisterKill();
        }
        // --------------------------------------------------------------------------

        // Si asignaste un efecto de explosión, lo crea antes de morir
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        
        // Destruye el objeto del enemigo
        Destroy(gameObject);
    }

    /// <summary>
    /// Destruye al enemigo si pasa de largo al jugador para no consumir memoria infinita.
    /// </summary>
    private void CheckOffScreen()
    {
        // Ajusta este -20f dependiendo de dónde esté tu cámara
        if (transform.position.z < -20f || transform.position.y < -20f)
        {
            Destroy(gameObject);
        }
    }
}