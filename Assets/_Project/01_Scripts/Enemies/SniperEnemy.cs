using UnityEngine;

public class SniperEnemy : EnemyBase
{
    [Header("Sniper Settings")]
    [SerializeField] private float stopDistance = 20f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1.5f;

    private float nextFireTime;
    private bool isStopped = false;

    protected override void Move()
    {
        // Stop moving once it reaches a certain Z (or Y) threshold
        if (!isStopped && transform.position.z <= stopDistance)
        {
            isStopped = true;
        }

        if (!isStopped)
        {
            base.Move(); // Move normally until stopped
        }
        else
        {
            Shoot(); // Start shooting when stopped
        }
    }

    private void Shoot()
    {
        if (Time.time >= nextFireTime && projectilePrefab != null)
        {
            nextFireTime = Time.time + fireRate;
            
            // Shoots directly backwards (towards the player)
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(Vector3.back));
            if (proj.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.SetupDirection(Vector3.back);
            }
        }
    }
}   