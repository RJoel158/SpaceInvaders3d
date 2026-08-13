using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] private float lifetime = 2.0f; // Tiempo antes de borrarse

    void Start()
    {
        // Se destruye automáticamente después de 2 segundos (lo que dura la explosión)
        Destroy(gameObject, lifetime);
    }
}