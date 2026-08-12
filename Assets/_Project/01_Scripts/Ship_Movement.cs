using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Ship_Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Movement speed of the spaceship.")]
    [SerializeField] private float moveSpeed = 12f;

    [Header("Boundaries Settings")]
    [Tooltip("Enable or disable movement restrictions on X and Z axes.")]
    [SerializeField] private bool useBoundaries = true;
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minZ = -5f;
    [SerializeField] private float maxZ = 5f;

    [Header("Tilt & Dynamics")]
    [Tooltip("Maximum roll angle when moving horizontally.")]
    [SerializeField] private float maxRollTilt = 25f;
    [Tooltip("Maximum pitch angle when moving vertically.")]
    [SerializeField] private float maxPitchTilt = 15f;
    [Tooltip("Speed at which the ship rotates towards target tilt.")]
    [SerializeField] private float rotationSpeed = 8f;

    [Header("Hover Oscillation")]
    [Tooltip("Frequency of the idle floating movement.")]
    [SerializeField] private float hoverFrequency = 2f;
    [Tooltip("Amplitude of the idle floating movement.")]
    [SerializeField] private float hoverAmplitude = 0.25f;

    [Header("Barrel Roll / Dash Settings")]
    [Tooltip("Distance the ship displaces during the barrel roll.")]
    [SerializeField] private float barrelRollDistance = 4f;
    [Tooltip("Duration of the barrel roll animation and displacement in seconds.")]
    [SerializeField] private float barrelRollDuration = 0.35f;
    [Tooltip("Cooldown time before performing another barrel roll.")]
    [SerializeField] private float barrelRollCooldown = 0.8f;

    // Components and Internal States
    private Rigidbody rb;
    private float moveX;
    private float moveZ;
    private float lastMoveX = 1f; // Default side direction if neutral
    private float initialY;
    private bool isDoingBarrelRoll = false;
    private float nextBarrelRollTime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Rigidbody configuration
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

        initialY = transform.position.y;
    }

    private void Update()
    {
        ReadInput();
        HandleBarrelRollInput();
    }

    private void FixedUpdate()
    {
        if (!isDoingBarrelRoll)
        {
            HandlePhysicsMovement();
            HandleShipVisuals();
        }
    }

    /// <summary>
    /// Reads direct key inputs from the New Input System.
    /// </summary>
    private void ReadInput()
    {
        if (Keyboard.current == null) return;

        moveX = 0f;
        moveZ = 0f;

        // Horizontal input
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            moveX = -1f;
            lastMoveX = -1f;
        }
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            moveX = 1f;
            lastMoveX = 1f;
        }

        // Vertical input
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
        {
            moveZ = 1f;
        }
        else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
        {
            moveZ = -1f;
        }
    }

    /// <summary>
    /// Checks for spacebar trigger to execute a Barrel Roll with displacement.
    /// </summary>
    private void HandleBarrelRollInput()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && Time.time >= nextBarrelRollTime && !isDoingBarrelRoll)
        {
            StartCoroutine(PerformBarrelRoll());
        }
    }

    /// <summary>
    /// Handles standard movement and hovering.
    /// </summary>
    private void HandlePhysicsMovement()
    {
        Vector3 movementDirection = new Vector3(moveX, 0f, moveZ);
        if (movementDirection.magnitude > 1f)
        {
            movementDirection.Normalize();
        }

        Vector3 nextPosition = transform.position + movementDirection * moveSpeed * Time.fixedDeltaTime;

        // Apply constant hover oscillation on Y axis
        float hoverOffset = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        nextPosition.y = initialY + hoverOffset;

        // Clamp boundaries
        if (useBoundaries)
        {
            nextPosition.x = Mathf.Clamp(nextPosition.x, minX, maxX);
            nextPosition.z = Mathf.Clamp(nextPosition.z, minZ, maxZ);
        }

        rb.MovePosition(nextPosition);
    }

    /// <summary>
    /// Calculates visual banking tilts (Pitch and Roll) based on direction.
    /// </summary>
    private void HandleShipVisuals()
    {
        float targetRoll = -moveX * maxRollTilt;
        float targetPitch = moveZ * maxPitchTilt;

        Quaternion targetRotation = Quaternion.Euler(targetPitch, 0f, targetRoll);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
    }

    /// <summary>
    /// Performs roll rotation synchronized with side displacement (Dash).
    /// </summary>
    private IEnumerator PerformBarrelRoll()
    {
        isDoingBarrelRoll = true;
        nextBarrelRollTime = Time.time + barrelRollCooldown;

        float elapsedTime = 0f;

        // Determine roll/dash direction based on current input, or last direction if stationary
        float dashDirection = moveX != 0 ? moveX : lastMoveX;
        float rollDirection = dashDirection < 0 ? 1f : -1f;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + new Vector3(dashDirection * barrelRollDistance, 0f, 0f);

        // Enforce boundary check on target position
        if (useBoundaries)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        }

        Quaternion startRotation = transform.rotation;

        while (elapsedTime < barrelRollDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / barrelRollDuration;

            // Smooth interpolation curve for physical displacement
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            // 1. Position displacement
            Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, smoothProgress);
            currentPos.y = initialY + Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
            rb.MovePosition(currentPos);

            // 2. 360-degree rotation roll
            float currentAngle = Mathf.Lerp(0f, 360f * rollDirection, progress);
            transform.rotation = startRotation * Quaternion.Euler(0f, 0f, currentAngle);

            yield return null;
        }

        isDoingBarrelRoll = false;
    }

    /// <summary>
    /// Visual representation of boundaries in Scene View.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!useBoundaries) return;

        Gizmos.color = Color.green;
        Vector3 center = new Vector3((minX + maxX) / 2f, transform.position.y, (minZ + maxZ) / 2f);
        Vector3 size = new Vector3(maxX - minX, 0.1f, maxZ - minZ);
        Gizmos.DrawWireCube(center, size);
    }
}