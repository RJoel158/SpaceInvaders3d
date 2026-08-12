using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSpeedEffect : MonoBehaviour
{
    [Header("FOV Dynamics")]
    [Tooltip("Base Field of View of the camera.")]
    [SerializeField] private float defaultFOV = 60f;
    [Tooltip("Target Field of View when moving forward or boosting.")]
    [SerializeField] private float speedFOV = 68f;
    [Tooltip("Smoothness speed of FOV transitions.")]
    [SerializeField] private float fovTransitionSpeed = 4f;

    [Header("Camera Tilt & Shake")]
    [Tooltip("Maximum camera tilt angle when ship moves left/right.")]
    [SerializeField] private float maxTiltAngle = 2f;
    [Tooltip("Smoothness speed of tilt transitions.")]
    [SerializeField] private float tiltSpeed = 5f;

    private Camera cam;
    private float currentTilt = 0f;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.fieldOfView = defaultFOV;
        }
    }

    private void Update()
    {
        HandleDynamicFOV();
        HandleCameraTilt();
    }

    /// <summary>
    /// Smoothly increases camera FOV based on forward input or speed.
    /// </summary>
    private void HandleDynamicFOV()
    {
        if (cam == null || Keyboard.current == null) return;

        bool isMovingForward = Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed;
        float targetFOV = isMovingForward ? speedFOV : defaultFOV;

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovTransitionSpeed);
    }

    /// <summary>
    /// Adds subtle counter-tilt to camera when steering horizontally.
    /// </summary>
    private void HandleCameraTilt()
    {
        if (Keyboard.current == null) return;

        float moveX = 0f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;

        float targetTilt = -moveX * maxTiltAngle;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);

        // Apply tilt on Z rotation without overriding existing pitch
        Vector3 currentEuler = transform.localEulerAngles;
        transform.localRotation = Quaternion.Euler(currentEuler.x, currentEuler.y, currentTilt);
    }
}