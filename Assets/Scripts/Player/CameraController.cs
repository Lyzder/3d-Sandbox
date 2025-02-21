using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    public float mouseSensitivity = 100f;
    public float distanceFromPlayer = 5f; // Distance between camera and player
    public float aimDistanceFromPlayer = 1f;
    public float pivotOffset = 2f;
    public float shoulderOffset = -0.3f;
    public Vector2 verticalClamp = new Vector2(-30f, 70f); // Clamp vertical rotation

    [Header("Aiming Settings")]
    public float aimDistance = 2f; // Closer distance when aiming
    public float aimSideOffset = 0.5f; // Side offset for over-the-shoulder
    public float transitionSpeed = 10f; // Smoothing factor

    private Transform playerPivot; // The pivot inside the player object
    private Vector2 lookInput;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool isAiming;
    private Vector3 defaultPivotPosition;
    private Vector3 aimingPivotPosition;
    private Quaternion rotation;
    private Vector3 offset;
    private Vector3 pivotPoint;
    private float targetDistance;
    private float currentDistance;

    private void Awake()
    {

    }

    private void Start()
    {
        playerPivot = transform.parent.transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isAiming = false;

        // Store default positions
        defaultPivotPosition = playerPivot.localPosition;
        aimingPivotPosition = defaultPivotPosition + new Vector3(shoulderOffset, 0f, 0f);
        currentDistance = distanceFromPlayer;
    }

    private void LateUpdate()
    {
        HandleCameraRotation();
        MovePivot();
        OrbitCamera();
    }

    public void SetLookInput(Vector2 input)
    {
        lookInput = input;
    }

    private void HandleCameraRotation()
    {
        // Get the mouse delta input
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        // Rotate the camera vertically and horizontally
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, verticalClamp.x, verticalClamp.y); // Clamp vertical rotation
    }

    private void OrbitCamera()
    {
        targetDistance = isAiming ? aimDistanceFromPlayer : distanceFromPlayer;
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * transitionSpeed);

        // Calculate the camera's position based on rotation and distance
        rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        offset = rotation * new Vector3(0, 0, -currentDistance);

        pivotPoint = playerPivot.position + Vector3.up * pivotOffset;

        // Update camera position and rotation
        transform.position = pivotPoint + offset;
        transform.LookAt(pivotPoint);
    }

    public void SetAimingState(bool aiming)
    {
        isAiming = aiming;
    }

    private void MovePivot()
    {
        playerPivot.localPosition = Vector3.Lerp(playerPivot.localPosition, isAiming ? aimingPivotPosition : defaultPivotPosition, Time.deltaTime * transitionSpeed);
    }
}