using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public static event Action OnPausing;
    public static event Action OnUnpausing;

    [SerializeField] GameObject pauseMenu;
    public Animator transitionAnimator; // Referencia al Animator para gestionar transiciones de escena
    public Slider musicSlider, sfxSlider; // Sliders para ajustar el volumen de la música y efectos de sonido
    public Toggle muteBmgChecker, muteSfxChecker; // Toggle para activar/desactivar el silencio
    public Sprite mutedSprite, unmutedSprite;
    public bool isPaused;

    private ushort screenIndex;

    private void Awake()
    {
        // Create singleton isntance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            isPaused = false;
            screenIndex = 0;
            pauseMenu.SetActive(false);

            GameManager.OnVictory += VictoryTransition;
            GameManager.OnGameOver += GameOverTransition;
            GameManager.OnStart += StartUI;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        GameManager.OnVictory -= VictoryTransition;
        GameManager.OnGameOver -= GameOverTransition;
        GameManager.OnStart -= StartUI;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StartUI()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor back (if FPS-style game)
        Cursor.visible = false;
        Time.timeScale = 1f;
        screenIndex = 0;
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f; // Stop game time
            Cursor.lockState = CursorLockMode.None; // Unlock cursor if needed
            Cursor.visible = true;
            LoadOptions();
            OnPausing?.Invoke();
        }
        else
        {
            Time.timeScale = 1f; // Resume game time
            Cursor.lockState = CursorLockMode.Locked; // Lock cursor back (if FPS-style game)
            Cursor.visible = false;
            OnUnpausing?.Invoke();
        }
    }

    // Método para cambiar el volumen de la música basado en el valor del slider
    public void ChangeMusicVolume()
    {
        AudioManager.Instance.SetMusicVolume(musicSlider.value);
        if (musicSlider.value > 0)
            muteBmgChecker.isOn = false;
        else
            muteBmgChecker.isOn = true;
        UpdateIcons();
    }

    // Método para cambiar el volumen de los efectos de sonido basado en el valor del slider
    public void ChangeSFXVolume()
    {
        AudioManager.Instance.SetSfxVolume(sfxSlider.value);
        if (sfxSlider.value > 0)
            muteSfxChecker.isOn = false;
        else
            muteSfxChecker.isOn = true;
        UpdateIcons();
    }

    // Método para alternar entre activar y desactivar el silencio
    public void MuteBGM()
    {
        if (musicSlider.value == 0)
            return;
        AudioManager.Instance.ToggleMuteMusic(musicSlider.value);
        UpdateIcons();
    }

    public void MuteSFX()
    {
        if (sfxSlider.value == 0)
            return;
        AudioManager.Instance.ToggleMuteSfx(sfxSlider.value);
        UpdateIcons();
    }

    private void UpdateIcons()
    {
        if (muteBmgChecker.isOn)
            muteBmgChecker.GetComponent<Image>().sprite = mutedSprite;
        else
            muteBmgChecker.GetComponent<Image>().sprite = unmutedSprite;
        if (muteSfxChecker.isOn)
            muteSfxChecker.GetComponent<Image>().sprite = mutedSprite;
        else
            muteSfxChecker.GetComponent<Image>().sprite = unmutedSprite;
    }

    // Guarda las preferencias de sonido en PlayerPrefs
    public void SaveOptions()
    {
        AudioManager.Instance.SaveSoundPreferences(musicSlider.value, sfxSlider.value, muteBmgChecker.isOn, muteSfxChecker.isOn);
    }

    // Carga las preferencias de sonido desde PlayerPrefs y las aplica a los controles de la UI
    public void LoadOptions()
    {

        musicSlider.value = PlayerPrefs.GetFloat(AudioManager.Instance.musicSavedValue); // Actualiza el slider de música
        sfxSlider.value = PlayerPrefs.GetFloat(AudioManager.Instance.sfxSavedValue); // Actualiza el slider de efectos de sonido
        muteBmgChecker.isOn = PlayerPrefs.GetInt(AudioManager.Instance.musicIsMuted) == 1; // Actualiza el toggle de silencio
        muteSfxChecker.isOn = PlayerPrefs.GetInt(AudioManager.Instance.sfxIsMuted) == 1; // Actualiza el toggle de silencio
        AudioManager.Instance.LoadSoundPreferences(); // Carga los valores guardados en AudioManager
    }

    // Método para iniciar la transición al menu
    public void PlayMenuTransition()
    {
        StartCoroutine(ToScene(0)); // Inicia la corrutina de transición
    }

    // Método para iniciar la transición al juego
    public void PlayGameTransition()
    {
        StartCoroutine(ToScene(1)); // Inicia la corrutina de transición
    }

    // Corrutina que maneja la transición de salida antes de empezar el juego
    IEnumerator ToScene(ushort index)
    {
        AudioManager.Instance.StopMusic();
        yield return new WaitForSecondsRealtime(0.5f); // Espera el tiempo que dura la animación
        HudManager.Instance.Deactivate();
        GameManager.Instance.TransitionToScene(index); // Comienza el juego
        gameObject.SetActive(false); // Desactiva este objeto (probablemente el menú principal)
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void HandlePause()
    {
        switch (screenIndex)
        {
            case 0:
                TogglePause();
                break;
            case 1:
                ShowScreen(0);
                break;
            default:
                TogglePause();
                break;
        }
    }

    public bool GetPaused()
    {
        return isPaused;
    }

    public void ShowScreen(int index)
    {
        transitionAnimator.SetInteger("screenIndex", index);
        screenIndex = (ushort)index;
    }

    private void GameOverTransition()
    {
        StartCoroutine(GameOver(5));
    }

    private void VictoryTransition()
    {
        StartCoroutine(GameOver(6));
    }

    IEnumerator GameOver(int index)
    {
        Time.timeScale = 0f; // Stop game time
        transitionAnimator.SetInteger("screenIndex", index);
        pauseMenu.SetActive(true);
        yield return new WaitForSecondsRealtime(1.33f);
        Cursor.lockState = CursorLockMode.None; // Unlock cursor if needed
        Cursor.visible = true;
    }
}
