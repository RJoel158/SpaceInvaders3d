using UnityEngine;

public class KamikazeEnemy : EnemyBase
{
    protected override void Move()
    {
        // Moves exactly like the base enemy, but we can add slight tracking later if we want.
        // For now, it just uses the fast speed set in the inspector.
        base.Move(); 
    }
}