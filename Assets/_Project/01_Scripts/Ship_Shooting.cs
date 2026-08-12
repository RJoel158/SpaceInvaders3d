using UnityEngine;
using UnityEngine.InputSystem;

public class Ship_Shooting : MonoBehaviour
{
    [Header("Shooting References")]
    [Tooltip("Point from where projectiles are spawned.")]
    [SerializeField] private Transform firePoint;

    [Tooltip("Projectile prefab to instantiate.")]
    [SerializeField] private GameObject projectilePrefab;

    [Tooltip("UI RectTransform representing the crosshair.")]
    [SerializeField] private RectTransform crosshairUI;

    [Tooltip("Parent Canvas RectTransform containing the crosshair.")]
    [SerializeField] private RectTransform parentCanvasRect;

    [Header("Shooting Settings")]
    [Tooltip("Cooldown delay between consecutive shots in seconds.")]
    [SerializeField] private float fireRate = 0.15f;

    [Tooltip("Distance in Z space where target plane is located.")]
    [SerializeField] private float aimDistance = 30f;

    private Camera mainCamera;
    private float nextFireTime = 0f;
    private Vector3 targetWorldPoint;

    private void Awake()
    {
        mainCamera = Camera.main;

        // Auto-assign Canvas RectTransform if not assigned manually
        if (crosshairUI != null && parentCanvasRect == null)
        {
            Canvas parentCanvas = crosshairUI.GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                parentCanvasRect = parentCanvas.GetComponent<RectTransform>();
            }
        }
    }

    private void Start()
    {
        // Hide standard OS mouse cursor so only the UI Crosshair is visible
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined; // Keeps cursor within Game window
    }

    private void Update()
    {
        UpdateCrosshairAndTarget();
        HandleShootingInput();
    }

    /// <summary>
    /// Tracks mouse position, updates UI crosshair anchored position, and calculates 3D target point.
    /// </summary>
    private void UpdateCrosshairAndTarget()
    {
        Vector2 mouseScreenPosition = Vector2.zero;

        if (Mouse.current != null)
        {
            mouseScreenPosition = Mouse.current.position.ReadValue();
        }

        // 1. Move UI Crosshair correctly inside Canvas
        if (crosshairUI != null && parentCanvasRect != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvasRect,
                mouseScreenPosition,
                null, // Use null for Screen Space - Overlay
                out Vector2 localPoint
            );

            crosshairUI.anchoredPosition = localPoint;
        }

        // 2. Create Ray from Camera towards Mouse Position
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);

        // 3. Aim Plane situated in front of the ship on Z axis
        Plane aimPlane = new Plane(Vector3.back, transform.position + Vector3.forward * aimDistance);

        if (aimPlane.Raycast(ray, out float enterDistance))
        {
            targetWorldPoint = ray.GetPoint(enterDistance);
        }
        else
        {
            targetWorldPoint = (firePoint != null ? firePoint.position : transform.position) + Vector3.forward * aimDistance;
        }
    }

    /// <summary>
    /// Detects left click input to instantiate projectiles.
    /// </summary>
    private void HandleShootingInput()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.isPressed && Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    /// <summary>
    /// Instantiates projectile targeting calculated 3D point.
    /// </summary>
    private void Shoot()
    {
        nextFireTime = Time.time + fireRate;

        if (projectilePrefab == null || firePoint == null) return;

        Vector3 shootDirection = (targetWorldPoint - firePoint.position).normalized;

        GameObject projInstance = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(shootDirection));

        if (projInstance.TryGetComponent<Projectile>(out var projectile))
        {
            projectile.SetupDirection(shootDirection);
        }
    }

    private void OnDrawGizmos()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetWorldPoint, 0.5f);
            Gizmos.DrawLine(firePoint.position, targetWorldPoint);
        }
    }
}