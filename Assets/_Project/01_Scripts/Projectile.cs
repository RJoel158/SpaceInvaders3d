using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [Tooltip("Movement speed of the projectile.")]
    [SerializeField] private float speed = 40f;

    [Tooltip("Lifetime in seconds before auto-destroying.")]
    [SerializeField] private float lifeTime = 10f;

    private Vector3 moveDirection = Vector3.forward;

    private void Start()
    {
        // Auto-destroy after 10 seconds
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// Sets initial world movement direction.
    /// </summary>
    /// <param name="direction">Normalized direction vector.</param>
    public void SetupDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    private void Update()
    {
        // Move projectile linearly in target direction
        transform.position += moveDirection * speed * Time.deltaTime;
    }
}