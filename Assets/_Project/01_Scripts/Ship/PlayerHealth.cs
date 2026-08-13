using UnityEngine;
using UnityEngine.UI; // Necesario para trabajar con la barra de vida de UI

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;

    [Header("UI Reference")]
    [SerializeField] private Slider healthSlider; // Arrastra aquí tu Slider de la UI

    void Start()
    {
        currentHealth = maxHealth;

        // Configurar la barra de vida al iniciar
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    // Método para recibir daño (llamado por los proyectiles enemigos o kamikazes)
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Actualizar la barra visualmente
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        Debug.Log("Vida actual del jugador: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Detección de colisión por choque directo con el Kamikaze
    private void OnTriggerEnter(Collider other)
    {
        // Si choca con un enemigo (asegúrate de que el kamikaze tenga su tag o lo detecte)
        if (other.TryGetComponent<KamikazeEnemy>(out var kamikaze))
        {
            TakeDamage(1f); // Pierde 1 de vida por choque
        }
        
        // Si un proyectil enemigo te da (asumiendo que las balas enemigas tienen un tag o lógica propia)
        if (other.CompareTag("EnemyProjectile"))
        {
            TakeDamage(1f); // Pierde 1 de vida por proyectil
            Destroy(other.gameObject); // Destruye la bala enemiga al impactar
        }
    }

    void Die()
    {
        Debug.Log("¡El jugador ha muerto!");
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        // Aquí puedes reiniciar la escena o mostrar pantalla de Game Over
        // Ejemplo: UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        
        Destroy(gameObject);
    }
}