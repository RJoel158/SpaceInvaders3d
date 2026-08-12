using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 40f;
    [SerializeField] private float lifeTime = 10f;
    [SerializeField] private float damage = 10f;

    [Tooltip("Tag to ignore. If this is player projectile, put 'Player'. If enemy projectile, put 'Enemy'.")]
    [SerializeField] private string ignoreTag = "Player";

    [Tooltip("Material to force-apply on all renderers (fixes pink/magenta shaders).")]
    [SerializeField] private Material overrideMaterial;

    private Vector3 moveDirection = Vector3.forward;

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
    }

    public void SetupDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    // --- SECCIÓN NUEVA: DAÑO Y COLISIÓN ---
    private void OnTriggerEnter(Collider other)
    {
        // Don't hit ourselves
        if (other.CompareTag(ignoreTag)) return;

        // Try to get the EnemyBase component from the object we hit
        if (other.TryGetComponent<EnemyBase>(out var enemy))
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject); // Destroy bullet after hitting enemy
        }
        else 
        {
            // Optional: destroy bullet if it hits an asteroid or wall
            // Destroy(gameObject);
        }
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