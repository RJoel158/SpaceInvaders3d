using UnityEngine;

public class BossEnemy : EnemyBase
{
    public enum BossState { Entering, Attacking, Dying }
    
    [Header("Boss Settings")]
    [SerializeField] private float stopDistance = 30f;
    private BossState currentState = BossState.Entering;

    protected override void Update()
    {
        // We completely override Update to use the State Machine instead of standard movement
        switch (currentState)
        {
            case BossState.Entering:
                HandleEntering();
                break;
            case BossState.Attacking:
                HandleAttacking();
                break;
            case BossState.Dying:
                // Handle death animation or delay before actual destruction
                break;
        }
    }

    private void HandleEntering()
    {
        base.Move(); // Move forward using base logic

        if (transform.position.z <= stopDistance)
        {
            currentState = BossState.Attacking;
        }
    }

    private void HandleAttacking()
    {
        // Logic for strafing left/right and firing multiple weapons
        // For example: transform.position += Vector3.right * Mathf.Sin(Time.time) * moveSpeed * Time.deltaTime;
    }

    public override void TakeDamage(float damageAmount)
    {
        if (currentState == BossState.Dying) return;

        base.TakeDamage(damageAmount);
    }

    protected override void Die()
    {
        currentState = BossState.Dying;
        Debug.Log("Boss Defeated!");
        base.Die(); // Call standard death (explosions, destroy)
    }
}