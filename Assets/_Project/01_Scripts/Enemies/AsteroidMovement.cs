using UnityEngine;

public class AsteroidMovement : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 10f; // Tiempo antes de autodestruirse si sale de la pantalla
    private Vector3 moveDirection;

    public void Initialize(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    void Start()
    {
        // Si no se inicializó, por defecto avanza hacia adelante en el mundo
        if (moveDirection == Vector3.zero)
        {
            moveDirection = transform.forward;
        }

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Se desplaza en línea recta constante hacia la dirección fijada al nacer
        transform.position += moveDirection * speed * Time.deltaTime;
        
        // Rotación estética opcional para que gire mientras vuela
        transform.Rotate(Vector3.up * 20f * Time.deltaTime);
    }
}