using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{

    public static HudManager Instance { get; private set; }

    [SerializeField] RectTransform hud;
    private Canvas canvas;
    private Transform player;
    private PlayerController playerController;
    private RectTransform ammoBar;
    private RectTransform currentAmmo;
    private RectTransform crosshair;
    private float maxBarHeight;
    private bool isActive;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        canvas = gameObject.GetComponentInChildren<Canvas>();
        hud = FindChildWithTag.FindInChildrenBFS(transform, "Hud").GetComponent<RectTransform>();
        ammoBar = (RectTransform)hud.GetChild(0);
        crosshair = (RectTransform)hud.GetChild(1);
        if (ammoBar == null)
            Debug.Log("No bar");
        currentAmmo = (RectTransform)ammoBar.GetChild(0);
        maxBarHeight = currentAmmo.sizeDelta.y;
        isActive = false;
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

    private void UpdateAmmoBar()
    {
        currentAmmo = (RectTransform)ammoBar.GetChild(0);
        currentAmmo.sizeDelta = new Vector2(currentAmmo.sizeDelta.x, playerController.GetAmmo() * maxBarHeight / 100);
    }

    private void ShowCrosshair()
    {
        if (playerController.GetAim())
            crosshair.GetComponent<Image>().enabled = true;
        else
            crosshair.GetComponent<Image>().enabled = false;
    }

    public void InitializeHud()
    {
        player = GameObject.FindWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        canvas.enabled = true;
        isActive = true;
    }
}
