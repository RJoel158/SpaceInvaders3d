using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;
    private bool isDead = false;

    [Header("UI Reference")]
    [SerializeField] private Slider healthSlider;

    [Header("Efectos de Muerte")]
    [SerializeField] private GameObject deathEffectPrefab; // Arrastra aquí tu prefab de explosión

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        // Si choca con un Kamikaze
        if (other.TryGetComponent<KamikazeEnemy>(out var kamikaze))
        {
            TakeDamage(1f);
        }
        
        // Si choca con un asteroide (asumiendo que los asteroides tienen el script Fracture o un tag de Asteroide)
        if (other.TryGetComponent<Fracture>(out var asteroid))
        {
            TakeDamage(5f); // Pierde 5 de vida por choque con asteroide
            asteroid.FractureObject(); // Opcional: rompe el asteroide al chocar con el player
        }
        
        if (other.CompareTag("EnemyProjectile"))
        {
            TakeDamage(1f);
            Destroy(other.gameObject);
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("¡El jugador ha muerto!");

        // 1. Instancia la explosión en la posición de la nave
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // 2. Reinicia la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // 3. Destruye el objeto de la nave
        Destroy(gameObject);
    }
}