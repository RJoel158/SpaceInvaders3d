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

    [Tooltip("Distance along Z axis in front of the ship where crosshair targets.")]
    [SerializeField] private float targetDepthDistance = 35f;

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

        if (isFiring && Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        nextFireTime = Time.time + fireRate;

        if (projectilePrefab == null || firePoint == null) return;

        // 1. Get current mouse/pointer position
        Vector2 mouseScreenPos = Vector2.zero;
        if (Pointer.current != null)
        {
            mouseScreenPos = Pointer.current.position.ReadValue();
        }

        // 2. Cast a ray from Main Camera towards the mouse position
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);

        // 3. Define an aim plane situated in front of the ship in world Z space
        Plane targetPlane = new Plane(Vector3.back, firePoint.position + Vector3.forward * targetDepthDistance);

        Vector3 targetWorldPoint;

        if (targetPlane.Raycast(ray, out float enterDistance))
        {
            targetWorldPoint = ray.GetPoint(enterDistance);
        }
        else
        {
            targetWorldPoint = firePoint.position + Vector3.forward * targetDepthDistance;
        }

        // 4. Calculate direction vector towards the target plane intersection point
        Vector3 shootDirection = (targetWorldPoint - firePoint.position).normalized;

        // 5. Instantiate projectile
        GameObject projInstance = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(shootDirection));

        if (projInstance.TryGetComponent<Projectile>(out var projectile))
        {
            projectile.SetupDirection(shootDirection);
        }
    }

    private void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}