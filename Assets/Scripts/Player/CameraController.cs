using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    public float lookSensitivity = 100f;
    public float distanceFromPlayer = 5f;
    public float pivotOffset = 2f;
    public Vector2 verticalClamp = new Vector2(-30f, 70f);

    private Transform playerBody;
    private Vector2 lookInput;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private Quaternion rotation;
    private Vector3 offset;

    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Look.canceled += OnLook;
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Look.performed -= OnLook;
        inputActions.Player.Look.canceled -= OnLook;
        inputActions.Disable();
    }

    private void Start()
    {
        playerBody = GameObject.FindWithTag("Player").transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        HandleCameraRotation();
        OrbitCamera();
    }

    /// <summary>
    /// Rotates the camera according to where the player is moving it
    /// </summary>
    private void HandleCameraRotation()
    {
        float lookX = lookInput.x * lookSensitivity * Time.deltaTime;
        float lookY = lookInput.y * lookSensitivity * Time.deltaTime;

        yRotation += lookX;
        xRotation -= lookY;
        // Clamps the vertical rotation
        xRotation = Mathf.Clamp(xRotation, verticalClamp.x, verticalClamp.y);
    }

    /// <summary>
    /// Moves the camera around the player so it's always the focus of the shot regardless of rotation
    /// </summary>
    private void OrbitCamera()
    {
        rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        offset = rotation * new Vector3(0, 0, -distanceFromPlayer);

        Vector3 pivotPoint = playerBody.position + Vector3.up * pivotOffset;
        transform.position = pivotPoint + offset;
        transform.LookAt(pivotPoint);
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
}
