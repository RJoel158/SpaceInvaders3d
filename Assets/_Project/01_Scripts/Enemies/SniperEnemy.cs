using UnityEngine;

public class SniperEnemy : EnemyBase
{
    [Header("Movimiento Sniper")]
    private Transform player;
    public float stoppingDistance = 15f; // Distancia a la que se detiene para disparar

    [Header("Combate")]
    public GameObject bulletPrefab;
    public Transform firePoint;          // Desde dónde sale la bala
    public float fireRate = 1.5f;        // Balas por segundo
    private float nextFireTime = 0f;     // Temporizador de disparo

    protected override void Start()
    {
        base.Start(); // Inicializa la vida de EnemyBase

        // Busca al jugador automáticamente si no lo arrastraste en el Inspector
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    protected override void Update()
    {
        // Si está muerto o no hay jugador, no hace nada
        if (isDead || player == null) return;

        HandleMovementAndAim();
        HandleShooting();
    }

    void HandleMovementAndAim()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Mirar siempre al jugador
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Evita que el enemigo se incline hacia el suelo o cielo
        
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        // Moverse hacia el jugador SOLO si está más lejos que su distancia de frenado
        if (distanceToPlayer > stoppingDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    void HandleShooting()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Si el jugador está dentro del rango de visión y el temporizador lo permite
        if (distanceToPlayer <= stoppingDistance + 2f && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + (1f / fireRate); // Reinicia el temporizador
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // Crea la bala en la posición y rotación del FirePoint
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
        else
        {
            Debug.LogError("¡Falta asignar la Bala o el FirePoint en el Inspector del Sniper!");
        }
    }
}