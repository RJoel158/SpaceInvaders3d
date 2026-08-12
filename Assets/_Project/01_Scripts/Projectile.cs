using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [Tooltip("Movement speed of the projectile.")]
    [SerializeField] private float speed = 40f;

    [Tooltip("Lifetime in seconds before auto-destroying.")]
    [SerializeField] private float lifeTime = 10f;

    [Tooltip("Material to force-apply on all renderers (fixes pink/magenta shaders).")]
    [SerializeField] private Material overrideMaterial;

    private Vector3 moveDirection = Vector3.forward;

    private void Awake()
    {
        // Force replace any broken/pink material on this object and all its children
        if (overrideMaterial != null)
        {
            ApplyMaterialToAllRenderers(gameObject);
        }
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// Sets initial world movement direction.
    /// </summary>
    public void SetupDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    /// <summary>
    /// Recursively overrides materials on MeshRenderers, TrailRenderers, and ParticleSystems.
    /// </summary>
    private void ApplyMaterialToAllRenderers(GameObject target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer rend in renderers)
        {
            Material[] mats = new Material[rend.sharedMaterials.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = overrideMaterial;
            }
            rend.materials = mats;
        }
    }
}