using UnityEngine;
using UnityEngine.InputSystem;

public class Ship_Shooting : MonoBehaviour
{
    [Header("Shooting References")]
    [Tooltip("Point from where projectiles are spawned.")]
    [SerializeField] private Transform firePoint;

    [Tooltip("Projectile prefab to instantiate.")]
    [SerializeField] private GameObject projectilePrefab;

    [Header("Custom Cursor Settings")]
    [Tooltip("Texture image to use as custom crosshair cursor.")]
    [SerializeField] private Texture2D customCursorTexture;

    [Header("Shooting Settings")]
    [Tooltip("Cooldown delay between consecutive shots in seconds.")]
    [SerializeField] private float fireRate = 0.15f;

    private Camera mainCamera;
    private float nextFireTime = 0f;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        SetCustomHardwareCursor();
    }

    private void SetCustomHardwareCursor()
    {
        if (customCursorTexture != null)
        {
            Vector2 cursorHotspot = new Vector2(customCursorTexture.width / 2f, customCursorTexture.height / 2f);
            Cursor.SetCursor(customCursorTexture, cursorHotspot, CursorMode.Auto);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        HandleShootingInput();
    }

    private void HandleShootingInput()
    {
        bool isFiring = false;

        if (Pointer.current != null)
        {
            isFiring = Pointer.current.press.isPressed;
        }
        else if (Mouse.current != null)
        {
            isFiring = Mouse.current.leftButton.isPressed;
        }

        // Solo dispara si hace clic Y el temporizador de cadencia lo permite
        if (isFiring && Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        nextFireTime = Time.time + fireRate; // Restaura el enfriamiento para que no sea constante

        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError("¡Falta asignar el projectilePrefab o el firePoint!");
            return;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        // 1. Obtener posición del ratón en pantalla
        Vector2 mouseScreenPos = Vector2.zero;
        if (Pointer.current != null)
        {
            mouseScreenPos = Pointer.current.position.ReadValue();
        }

        // 2. Lanzar un rayo desde la cámara hacia el ratón
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);

        // 3. Crear un plano horizontal a la altura de la nave (Eje Y de la nave)
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, firePoint.position.y, 0));

        // 4. Calcular el punto exacto donde el rayo del mouse cruza el plano del mundo
        if (groundPlane.Raycast(ray, out float enterDistance))
        {
            Vector3 worldTarget = ray.GetPoint(enterDistance);
            
            // Dirección exacta hacia el cursor en el suelo del juego
            Vector3 shootDirection = (worldTarget - firePoint.position).normalized;
            shootDirection.y = 0; // Mantener los disparos nivelados en el plano de la nave

            // 5. Instanciar la bala con la rotación correcta
            GameObject projInstance = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(shootDirection));

            if (projInstance.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.SetupDirection(shootDirection);
            }
        }
    }

    private void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}