using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float sprintMultiplier = 2.1f;
    public float sprintBackMultiplier = 1.7f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float fallMultiplier = 2.5f;
    public Transform cameraTransform;

    // Movement variables
    private Vector2 moveInput;
    private Vector3 velocity;
    private bool sprinting;
    private bool isGrounded;
    private bool doubleJump;
    private Animator animator;
    private Vector3 cameraForward;
    private Vector3 cameraRight;
    private float currentSpeed;
    private Vector3 move;
    private Vector3 moveDirection;
    private Vector3 lookDirection;
    private Quaternion targetRotation;
    private Vector3 bottomController;
    private Vector3 topController;

    // Camera variables
    [SerializeField] CameraController cameraController;
    private Vector2 lookInput;

    // Shooting variables
    [Header("Shooting Settings")]
    [SerializeField] GameObject bullet;
    [SerializeField] float ammoCharge;
    [SerializeField] float ammoBuffDuration;
    public int rechargeRate;
    private GameObject bulletSpawn;
    private Vector3 spawnPosition;
    private Vector2 valueClamp;
    private bool isAiming;
    private bool isAmmoBuff;
    private float buffTimer;
    private bool endBuffClipPlayed;
    public AudioClip shootSfx;
    public AudioClip ammoBuffStartSfx;
    public AudioClip ammoBuffEndSfx;

    // Input
    private CharacterController characterController;
    private InputSystem_Actions inputActions;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Initialize Input Actions
        inputActions = new InputSystem_Actions();

        ammoCharge = 100f;
        valueClamp = new Vector2(0f, 100f);
        isAiming = false;
        isAmmoBuff = false;

        bulletSpawn = FindChildWithTag.FindInChildrenBFS(transform, "BulletSpawn");
    }

    private void OnEnable()
    {
        inputActions.Enable();
        // Player actions
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprint;
        inputActions.Player.Attack.performed += ctx => Shoot();
        inputActions.Player.Jump.performed += ctx => Jump();
        inputActions.Player.Aim.performed += ctx => SetAiming(true);
        inputActions.Player.Aim.canceled += ctx => SetAiming(false);
        inputActions.Player.Pause.performed += ctx => Pause();
        inputActions.UI.MenuButton.performed += ctx => Pause();

        // Camera movement
        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();

        // UI
        UIManager.OnPausing += ToggleInputActions;
        UIManager.OnUnpausing += ToggleInputActions;

        // Game logic
        GameManager.OnStart += StartPlayer;
        GameManager.OnGameOver += EndActions;
        GameManager.OnVictory += EndActions;
    }

    private void OnDisable()
    {
        inputActions.Disable();
        // Player actions
        inputActions.Player.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled -= ctx => moveInput = Vector2.zero;
        inputActions.Player.Sprint.performed -= OnSprint;
        inputActions.Player.Sprint.canceled -= OnSprint;
        inputActions.Player.Attack.performed -= ctx => Shoot();
        inputActions.Player.Jump.performed -= ctx => Jump();
        inputActions.Player.Aim.performed -= ctx => SetAiming(true);
        inputActions.Player.Aim.canceled -= ctx => SetAiming(false);
        inputActions.Player.Pause.performed -= ctx => Pause();
        inputActions.UI.MenuButton.performed -= ctx => Pause();

        // Camera movement
        inputActions.Player.Look.performed -= ctx => lookInput = Vector2.zero;

        // UI
        UIManager.OnPausing -= ToggleInputActions;
        UIManager.OnUnpausing -= ToggleInputActions;

        // Game logic
        GameManager.OnStart -= StartPlayer;
        GameManager.OnGameOver -= EndActions;
        GameManager.OnVictory -= EndActions;
    }

    private void Start()
    {
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        ToggleInputActions();
        buffTimer = 0;
    }

    private void Update()
    {
        cameraController.SetLookInput(lookInput);
        lookInput = Vector2.zero;
        RechargeAmmo();
        if (isAmmoBuff)
            BuffCountdown();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        ApplyGravity();
    }

    private void LateUpdate()
    {
        RotatePlayerModel();
    }

    private void MovePlayer()
    {
        // Convert input to world space
        move = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        if (move.magnitude >= 0.1f)
        {
            // Get the camera's forward and right vectors (ignore vertical rotation)
            cameraForward = cameraTransform.forward;
            cameraRight = cameraTransform.right;
            currentSpeed = sprinting ? moveSpeed * sprintMultiplier : moveSpeed;
            if (moveInput.y < 0f)
            {
                animator.SetBool("isBackward", true);
                currentSpeed = sprinting ? moveSpeed * sprintBackMultiplier : moveSpeed;
            }
            else
            {
                animator.SetBool("isBackward", false);
                currentSpeed = sprinting ? moveSpeed * sprintMultiplier : moveSpeed;
            }
            if (sprinting)
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isSprinting", true);
            }
            else
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("isSprinting", false);
            }
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calculate the movement direction relative to the camera
            moveDirection = (cameraForward * move.z + cameraRight * move.x).normalized;

            // Move the player
            characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isSprinting", false);
            animator.SetBool("isBackward", false);
        }
    }

    private void RotatePlayerModel()
    {
        if (moveInput.sqrMagnitude > 0.01f || isAiming)
        {
            // Get the camera's forward direction, ignoring the y-axis
            lookDirection = cameraTransform.forward;
            lookDirection.y = 0f; // Ensure the rotation is only on the horizontal plane

            if (lookDirection != Vector3.zero)
            {
                // Smooth rotation towards the camera direction
                targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 20f);
            }
        }
    }

    private void ApplyGravity()
    {
        bottomController = transform.position + characterController.center - Vector3.up * (characterController.height / 2 - characterController.radius + 0.3f);
        topController = transform.position + characterController.center + Vector3.up * (characterController.height / 2 - characterController.radius);

        isGrounded = Physics.CheckCapsule(topController, bottomController, characterController.radius * 0.95f, LayerMask.GetMask("Ground"));


        if (isGrounded && velocity.y < 0) // On ground
        {
            velocity.y = -2f;  // Reset velocity when touching the ground
            doubleJump = true;
            animator.SetBool("Landed", true);
        }
        else if (velocity.y < 0) // Falling
        {
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
            animator.SetBool("Landed", false);
        }
        else // Rising
        {
            velocity.y += gravity * Time.deltaTime * 2.5f;
        }

        characterController.Move(velocity * Time.deltaTime);
        animator.SetFloat("verticalSpeed", velocity.y);
    }

    private void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity) * 2f;
            animator.SetBool("Landed", false);
        }
        else if (doubleJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity) * 2f;
            doubleJump = false;
        }
        animator.SetFloat("verticalSpeed", velocity.y);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            sprinting = true; // Sprint key is held down
        }
        else if (context.canceled)
        {
            sprinting = false; // Sprint key is released
        }
    }

    private void SetAiming(bool isAiming)
    {
        this.isAiming = isAiming;
        cameraController.SetAimingState(isAiming); // Notify camera
    }

    public bool GetAim()
    {
        return isAiming;
    }

    private void Shoot()
    {
        if (ammoCharge < 20f && !isAmmoBuff)
            return;
        ammoCharge -= isAmmoBuff ? 0 : 20;
        spawnPosition = bulletSpawn.transform.position;
        Instantiate(bullet, spawnPosition, Quaternion.LookRotation(cameraTransform.forward));
        AudioManager.Instance.PlaySFX(shootSfx);
    }

    private void RechargeAmmo()
    {
        ammoCharge += rechargeRate * Time.deltaTime;
        ammoCharge = Mathf.Clamp(ammoCharge, valueClamp.x, valueClamp.y);
    }
    public float GetAmmo()
    {
        return ammoCharge;
    }

    public bool GetIsBuff()
    {
        return isAmmoBuff;
    }

    public void SetBuff()
    {
        isAmmoBuff = true;
        AudioManager.Instance.PlaySFX(ammoBuffStartSfx);
        endBuffClipPlayed = false;
    }

    private void BuffCountdown()
    {
        buffTimer += Time.deltaTime;
        if (buffTimer >= ammoBuffDuration)
        {
            buffTimer = 0;
            isAmmoBuff = false;
        }
        if (!endBuffClipPlayed && ammoBuffDuration - buffTimer <= ammoBuffEndSfx.length) { 
            AudioManager.Instance.PlaySFX(ammoBuffEndSfx);
            endBuffClipPlayed = true;
        }
    }

    private void Pause()
    {
        UIManager.Instance.HandlePause();
    }

    private void ToggleInputActions()
    {
        if (UIManager.Instance.GetPaused())
        {
            inputActions.UI.Enable();
            inputActions.Player.Disable();
        }
        else
        {
            inputActions.UI.Disable();
            inputActions.Player.Enable();
        }
    }

    private void StartPlayer()
    {
        inputActions.Player.Enable();
        inputActions.UI.Disable();
        ammoCharge = 100f;
        buffTimer = 0;
        isAmmoBuff = false;
        isAiming = false;
    }

    private void EndActions()
    {
        inputActions.Player.Disable();
        inputActions.UI.Enable();
    }

    private void OnDrawGizmos()
    {
        if (characterController == null) return;

        Vector3 bottom = transform.position + characterController.center - Vector3.up * (characterController.height / 2 - characterController.radius - 0.05f);
        Vector3 top = transform.position + characterController.center + Vector3.up * (characterController.height / 2 - characterController.radius);

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(bottom, characterController.radius * 0.95f);
        Gizmos.DrawWireSphere(top, characterController.radius * 0.95f);
        Gizmos.DrawLine(top, bottom);
    }
}
