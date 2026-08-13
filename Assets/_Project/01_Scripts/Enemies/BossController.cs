using System.Collections;
using UnityEngine;

public class BossController : EnemyBase
{
    public enum BossState { Phase1_NormalShot, Phase2_MoveAndMultiShot, Phase3_ErraticMovement, FinalPhase_SelfDestruct }

    [Header("Configuración del Boss")]
    [SerializeField] private BossState currentState;
    [SerializeField] private GameObject enemyProjectilePrefab;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private float moveSpeedValue = 5f;
    
    [Header("Distancias de Combate")]
    [SerializeField] private float combatDistance = 15f; // Distancia segura para las fases de movimiento

    private Transform playerTransform;
    private Vector3 erraticTarget;
    private float timer = 0f;

    protected override void Start()
    {
        base.Start(); 
        currentState = BossState.Phase1_NormalShot;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerTransform = p.transform;

        StartCoroutine(BossBehaviorRoutine());
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount); 
        UpdateBossPhase();
    }

    void UpdateBossPhase()
    {
        float healthPercentage = currentHealth / maxHealth;

        if (healthPercentage <= 0.25f && currentState != BossState.FinalPhase_SelfDestruct)
            currentState = BossState.FinalPhase_SelfDestruct;
        else if (healthPercentage <= 0.50f && currentState < BossState.Phase3_ErraticMovement)
            currentState = BossState.Phase3_ErraticMovement;
        else if (healthPercentage <= 0.75f && currentState < BossState.Phase2_MoveAndMultiShot)
            currentState = BossState.Phase2_MoveAndMultiShot;
    }

    IEnumerator BossBehaviorRoutine()
    {
        while (!isDead)
        {
            switch (currentState)
            {
                case BossState.Phase1_NormalShot:
                    // FASE 1: Estático, solo gira y dispara desde su posición actual
                    StaticShoot(false);
                    yield return new WaitForSeconds(1.5f);
                    break;

                case BossState.Phase2_MoveAndMultiShot:
                    MoveSideToSide();
                    Shoot(true);
                    yield return new WaitForSeconds(1.0f);
                    break;

                case BossState.Phase3_ErraticMovement:
                    MoveErratic();
                    Shoot(true);
                    yield return new WaitForSeconds(0.7f);
                    break;

                case BossState.FinalPhase_SelfDestruct:
                    ChargeAtPlayer();
                    yield return new WaitForSeconds(0.2f);
                    break;
            }
            yield return null;
        }
    }

    // Fase 1: Se queda totalmente quieto, solo apunta y dispara
    void StaticShoot(bool multi)
    {
        LookAtPlayer();
        Shoot(multi);
    }

    void Shoot(bool multi)
    {
        if (enemyProjectilePrefab == null) return;
        if (multi)
        {
            foreach (Transform fp in firePoints) Instantiate(enemyProjectilePrefab, fp.position, fp.rotation);
        }
        else if (firePoints.Length > 0)
        {
            Instantiate(enemyProjectilePrefab, firePoints[0].position, firePoints[0].rotation);
        }
    }

    void MoveSideToSide()
    {
        if (playerTransform == null) return;
        
        float pingPong = Mathf.Sin(Time.time * 3f) * 6f;
        Vector3 targetPos = playerTransform.position + (playerTransform.forward * combatDistance) + new Vector3(pingPong, 2f, 0);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeedValue);
        
        LookAtPlayer();
    }

    void MoveErratic()
    {
        if (playerTransform == null) return;
        timer += Time.deltaTime;
        if (timer > 2f) 
        { 
            timer = 0f; 
            Vector3 randomOffset = new Vector3(Random.Range(-8f, 8f), Random.Range(1f, 4f), Random.Range(combatDistance - 5f, combatDistance + 5f));
            erraticTarget = playerTransform.position + randomOffset; 
        }
        transform.position = Vector3.MoveTowards(transform.position, erraticTarget, moveSpeedValue * 1.5f * Time.deltaTime);
        
        LookAtPlayer();
    }

    void ChargeAtPlayer()
    {
        if (playerTransform != null) 
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeedValue * 2.5f * Time.deltaTime);
            LookAtPlayer();
        }
    }

    void LookAtPlayer()
    {
        if (playerTransform == null) return;
        Vector3 lookDir = (playerTransform.position - transform.position).normalized;
        lookDir.y = 0;
        if (lookDir != Vector3.zero) transform.rotation = Quaternion.LookRotation(lookDir);
    }

    protected override void Die()
    {
        if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) < 8f)
        {
            playerTransform.GetComponent<PlayerHealth>()?.TakeDamage(25f);
        }
        base.Die();
    }
}