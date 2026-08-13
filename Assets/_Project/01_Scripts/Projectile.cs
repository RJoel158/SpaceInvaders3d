using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 60f; // Aumentado para mayor precisión a distancia
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float damage = 25f;

    [Tooltip("Tag to ignore. If this is player projectile, put 'Player'. If enemy projectile, put 'Enemy'.")]
    [SerializeField] private string ignoreTag = "Player";

    [Tooltip("Material to force-apply on all renderers (fixes pink/magenta shaders).")]
    [SerializeField] private Material overrideMaterial;

    private Vector3 moveDirection = Vector3.zero;

    private void Awake()
    {
        if (overrideMaterial != null)
        {
            ApplyMaterialToAllRenderers(gameObject);
        }
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);

        if (moveDirection == Vector3.zero)
        {
            moveDirection = transform.forward;
        }
    }

    public void SetupDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    private void Update()
    {
        // Movimiento ultra preciso por frame
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ignoreTag)) return;

        // 1. Dañar a cualquier enemigo que herede de EnemyBase (Kamikaze, Sniper, Boss)
        if (other.TryGetComponent<EnemyBase>(out var enemy))
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject); 
            return;
        }
        
        // 2. Dañar al jugador (si la bala es enemiga)
        if (other.CompareTag("Player"))
        {
            // Descomenta la línea de abajo si ya tienes tu script de vida del jugador configurado:
            // if (other.TryGetComponent<PlayerHealth>(out var player)) { player.TakeDamage(damage); }
            
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<Fracture>(out var fractureObject))
        {
            fractureObject.FractureObject();
            Destroy(gameObject); // Destruye la bala
            return;
        }
        // Si choca con cualquier cosa que herede de EnemyBase (incluyendo el Boss)
        if (other.TryGetComponent<EnemyBase>(out var baseEnemy))
        {
            baseEnemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        

        if (CompareTag("EnemyProjectile")) // Asegúrate de que tu bala enemiga tenga el Tag o un booleano que la identifique
        {
            if (other.CompareTag("Enemy") || other.TryGetComponent<Fracture>(out _))
            {
                return; // Ignora la colisión por completo
            }
        }
        // Destruir la bala si choca con el entorno (paredes, suelo, etc.)
        Destroy(gameObject);
    }

    private void ApplyMaterialToAllRenderers(GameObject target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer rend in renderers)
        {
            Material[] mats = new Material[rend.sharedMaterials.Length];
            for (int i = 0; i < mats.Length; i++) mats[i] = overrideMaterial;
            rend.materials = mats;
        }
    }
}