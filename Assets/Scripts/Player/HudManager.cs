using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{

    public static HudManager Instance { get; private set; }

    [SerializeField] RectTransform hud;
    private Canvas canvas;
    private Transform player;
    private PlayerController playerController;
    private Transform ammoBar;
    private RectTransform currentAmmo;
    private RectTransform crosshair;
    private float maxBarHeight;
    private bool isActive;

    private void Awake()
    {
        // Create singleton isntance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            // Prepares the references for functionality
            canvas = gameObject.GetComponentInChildren<Canvas>();
            hud = FindChildWithTag.FindInChildrenBFS(transform, "Hud").GetComponent<RectTransform>();
            ammoBar = hud.GetChild(0);
            crosshair = (RectTransform)hud.GetChild(1);
            if (ammoBar == null)
                Debug.Log("No bar");
            currentAmmo = (RectTransform)ammoBar.GetChild(0);
            maxBarHeight = currentAmmo.sizeDelta.y;
            isActive = false;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isActive)
            return;
        UpdateAmmoBar();
        ShowCrosshair();
    }

    /// <summary>
    /// Animates the ammo bar according to the Player's ammo
    /// </summary>
    private void UpdateAmmoBar()
    {
        // If the Player has an ammo buff, make the bar golden
        if (playerController.GetIsBuff())
        {
            ammoBar.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            ammoBar.GetComponent<Image>().color = Color.white;
        }
        currentAmmo = (RectTransform)ammoBar.GetChild(0);
        currentAmmo.sizeDelta = new Vector2(currentAmmo.sizeDelta.x, playerController.GetAmmo() * maxBarHeight / 100);
    }

    /// <summary>
    /// Shows the crosshair if the Player is aiming
    /// </summary>
    private void ShowCrosshair()
    {
        if (playerController.GetAim())
            crosshair.GetComponent<Image>().enabled = true;
        else
            crosshair.GetComponent<Image>().enabled = false;
    }

    /// <summary>
    /// Initializes the hud
    /// </summary>
    public void InitializeHud()
    {
        player = GameObject.FindWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        canvas.enabled = true;
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
        canvas.enabled = false;
    }
}
