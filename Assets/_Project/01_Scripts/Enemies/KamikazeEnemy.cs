using UnityEngine;

public class KamikazeEnemy : EnemyBase
{
    [Header("Configuración de Ataque Kamikaze")]
    public float rushSpeed = 9f;
    public float triggerDistance = 8f;
    public float damageToPlayer = 25f;

    private Transform player;
    private bool isRushing = false;

    protected override void Start()
    {
        base.Start(); // Inicializa la vida de EnemyBase

        // Búsqueda automática del jugador por Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Kamikaze no encuentra ningún objeto con el Tag 'Player'.");
        }
    }

    protected override void Update()
    {
        // Si está muerto o no hay jugador, no hace nada (hereda la lógica de fuera de pantalla)
        if (isDead || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Aumentar velocidad si entra en el rango de activación
        if (distanceToPlayer <= triggerDistance)
        {
            isRushing = true;
        }

        float currentSpeed = isRushing ? rushSpeed : moveSpeed;
        
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);
        transform.position = Vector3.MoveTowards(transform.position, player.position, currentSpeed * Time.deltaTime);

        // Estrellarse contra el jugador
        if (distanceToPlayer <= 1.2f)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (isDead) return;

        // Aquí puedes hacer daño al jugador si lo necesitas:
        // if (player.TryGetComponent<PlayerHealth>(out var hp)) { hp.TakeDamage(damageToPlayer); }

        Die(); // Llama al Die() unificado de EnemyBase (crea explosión, avisa al spawner y se destruye)
    }
}