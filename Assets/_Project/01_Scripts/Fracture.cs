using UnityEngine;

public class Fracture : MonoBehaviour
{
    [Tooltip("\"Fractured\" is the object that this will break into")]
    [SerializeField] private GameObject fractured;

    [Header("Efecto de Explosión de Trozos")]
    [SerializeField] private float explosionForce = 5f; // Fuerza con la que salen disparados los trozos

    public void FractureObject()
    {
        if (fractured != null)
        {
            // 1. Instancia la versión rota en la misma posición y rotación
            GameObject brokenInstance = Instantiate(fractured, transform.position, transform.rotation);

            // 2. Si el objeto roto tiene Rigidbody en sus piezas hijas, aplicarles un pequeño impulso de dispersión
            Rigidbody[] rbPieces = brokenInstance.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rbPieces)
            {
                // Agrega una fuerza explosiva local para que los trozos salgan volando de forma realista
                rb.AddExplosionForce(explosionForce, transform.position, 2f, 1f, ForceMode.Impulse);
            }
        }

        // 3. Destruye el asteroide original para que desaparezca
        Destroy(gameObject);
    }
}