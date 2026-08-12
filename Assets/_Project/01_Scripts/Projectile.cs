using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [Tooltip("Movement speed of the projectile.")]
    [SerializeField] private float speed = 30f;

    [Tooltip("Lifetime in seconds before auto-destroying.")]
    [SerializeField] private float lifeTime = 10f;

    private Vector3 moveDirection;

    private void Start()
    {
        // Auto-destroy after specified lifetime (10 seconds)
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// Initializes direction vector for projectile movement.
    /// </summary>
    /// <param name="direction">Normalized world direction.</param>
    public void SetupDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    private void Update()
    {
        // Move linear towards initialized direction
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Placeholder for collision logic with enemies
        // We will handle enemy damage logic in future steps
    }
}