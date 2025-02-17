using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudManager : MonoBehaviour
{
    [SerializeField] RectTransform hud;

    private Transform player;
    private PlayerShoot shootScript;
    private RectTransform ammoBar;
    private RectTransform currentAmmo;
    private float maxBarHeight;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        shootScript = player.GetComponent<PlayerShoot>();
        ammoBar = (RectTransform)hud.GetChild(0);
        if (ammoBar == null)
            Debug.Log("No bar");
        currentAmmo = (RectTransform)ammoBar.GetChild(0);
        maxBarHeight = currentAmmo.sizeDelta.y;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAmmoBar();
    }

    void UpdateAmmoBar()
    {
        currentAmmo = (RectTransform)ammoBar.GetChild(0);
        currentAmmo.sizeDelta = new Vector2(currentAmmo.sizeDelta.x, shootScript.GetAmmo() * maxBarHeight / 100);
    }
}
