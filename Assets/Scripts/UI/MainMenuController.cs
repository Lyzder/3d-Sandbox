using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Animator transitionAnimator; // Referencia al Animator para gestionar transiciones de escena

    public Slider musicSlider, sfxSlider; // Sliders para ajustar el volumen de la m�sica y efectos de sonido
    public Toggle muteBmgChecker, muteSfxChecker; // Toggle para activar/desactivar el silencio
    public Sprite mutedSprite, unmutedSprite;
    public bool inSettings;

    public void Awake()
    {
        inSettings = false;
        Cursor.lockState = CursorLockMode.None; // Unlock cursor if needed
        Cursor.visible = true;
    }

    // M�todo para iniciar la transici�n al juego
    public void PlayGameTransition(int difficulty)
    {
        GameManager.Instance.SetDifficulty((ushort)difficulty);
        StartCoroutine(ToGameTransition()); // Inicia la corrutina de transici�n
    }

    // Corrutina que maneja la transici�n de salida antes de empezar el juego
    IEnumerator ToGameTransition()
    {
        AudioManager.Instance.StopMusic();
        yield return new WaitForSecondsRealtime(0.5f); // Espera el tiempo que dura la animaci�n
        GameManager.Instance.TransitionToScene(1); // Comienza el juego
        this.gameObject.SetActive(false); // Desactiva este objeto (probablemente el men� principal)
    }

    // M�todo para cambiar el volumen de la m�sica basado en el valor del slider
    public void ChangeMusicVolume()
    {
        AudioManager.Instance.SetMusicVolume(musicSlider.value);
        if (musicSlider.value > 0)
            muteBmgChecker.isOn = false;
        else
            muteBmgChecker.isOn = true;
        UpdateIcons();
    }

    // M�todo para cambiar el volumen de los efectos de sonido basado en el valor del slider
    public void ChangeSFXVolume()
    {
        AudioManager.Instance.SetSfxVolume(sfxSlider.value);
        if (sfxSlider.value > 0)
            muteSfxChecker.isOn = false;
        else
            muteSfxChecker.isOn = true;
        UpdateIcons();
    }

    // M�todo para alternar entre activar y desactivar el silencio
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
        musicSlider.value = PlayerPrefs.GetFloat(AudioManager.Instance.musicSavedValue); // Actualiza el slider de m�sica
        sfxSlider.value = PlayerPrefs.GetFloat(AudioManager.Instance.sfxSavedValue); // Actualiza el slider de efectos de sonido
        muteBmgChecker.isOn = PlayerPrefs.GetInt(AudioManager.Instance.musicIsMuted) == 1; // Actualiza el toggle de silencio
        muteSfxChecker.isOn = PlayerPrefs.GetInt(AudioManager.Instance.sfxIsMuted) == 1; // Actualiza el toggle de silencio
        AudioManager.Instance.LoadSoundPreferences(); // Carga los valores guardados en AudioManager
    }

    public void ShowMenu(int index)
    {
        transitionAnimator.SetInteger("menuIndex", index);
    }

}
