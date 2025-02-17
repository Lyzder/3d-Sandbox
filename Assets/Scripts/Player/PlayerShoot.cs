using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Transform cameraTransform;
    public AudioClip shootSfx;

    private CharacterController characterController;
    private InputSystem_Actions inputActions;
    [SerializeField] GameObject bullet;
    [SerializeField] float ammoCharge;
    private Vector3 offset;
    private Vector3 spawnPosition;
    private Vector2 valueClamp;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        inputActions = new InputSystem_Actions();

        inputActions.Player.Attack.performed += ctx => Shoot();

        ammoCharge = 100f;
        valueClamp = new Vector2(0f, 100f);
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ammoCharge += 25 * Time.deltaTime;
        ammoCharge = Mathf.Clamp(ammoCharge,valueClamp.x,valueClamp.y);
    }

    private void Shoot()
    {
        if (ammoCharge < 20f)
            return;
        ammoCharge -= 20f;
        spawnPosition = transform.position + (characterController.center * 4 / 2) + cameraTransform.forward;
        Instantiate(bullet, spawnPosition, Quaternion.LookRotation(cameraTransform.forward));
        AudioManager.Instance.PlaySFX(shootSfx);
    }

    public float GetAmmo()
    {
        return ammoCharge;
    }
}
