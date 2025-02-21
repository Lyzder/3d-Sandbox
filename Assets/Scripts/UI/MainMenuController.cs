using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Animator transitionAnimator; // Referencia al Animator para gestionar transiciones de escena

    public Slider musicSlider, sfxSlider; // Sliders para ajustar el volumen de la música y efectos de sonido
    public Toggle muteBmgChecker, muteSfxChecker; // Toggle para activar/desactivar el silencio
    public Sprite mutedSprite, unmutedSprite;
    public bool inSettings;

    public void Awake()
    {
        inSettings = false;
    }

    // Método para iniciar la transición al juego
    public void PlayGameTransition()
    {
        StartCoroutine(ToGameTransition()); // Inicia la corrutina de transición
    }

    // Corrutina que maneja la transición de salida antes de empezar el juego
    IEnumerator ToGameTransition()
    {
        //transitionAnimator.SetBool("toOut", true); // Activa la animación de transición de salida
        yield return new WaitForSeconds(3.5f); // Espera el tiempo que dura la animación
        SceneManager.LoadScene("TestingGrounds"); // Comienza el juego
        this.gameObject.SetActive(false); // Desactiva este objeto (probablemente el menú principal)
    }

    // Método para cambiar el volumen de la música basado en el valor del slider
    public void ChangeMusicVolume()
    {
        AudioManager.Instance.SetMusicVolume(musicSlider.value);
    }

    // Método para cambiar el volumen de los efectos de sonido basado en el valor del slider
    public void ChangeSFXVolume()
    {
        AudioManager.Instance.SetSfxVolume(sfxSlider.value);
    }

    // Método para alternar entre activar y desactivar el silencio
    public void MuteBGM()
    {
        AudioManager.Instance.ToggleMuteMusic();
        if (muteBmgChecker.isOn)
            muteBmgChecker.GetComponent<Image>().sprite = mutedSprite;
        else
            muteBmgChecker.GetComponent<Image>().sprite = unmutedSprite;
    }

    public void MuteSFX()
    {
        AudioManager.Instance.ToggleMuteSfx();
        if (muteSfxChecker.isOn)
            muteSfxChecker.GetComponent<Image>().sprite = mutedSprite;
        else
            muteSfxChecker.GetComponent<Image>().sprite = unmutedSprite;
    }

    // Guarda las preferencias de sonido en PlayerPrefs
    public void SaveOptions()
    {
        AudioManager.Instance.SaveSoundPreferences(musicSlider.value, sfxSlider.value);
    }

    // Carga las preferencias de sonido desde PlayerPrefs y las aplica a los controles de la UI
    public void LoadOptions()
    {
        AudioManager.Instance.LoadSoundPreferences(); // Carga los valores guardados en AudioManager
        musicSlider.value = PlayerPrefs.GetFloat(AudioManager.Instance.musicSavedValue); // Actualiza el slider de música
        sfxSlider.value = PlayerPrefs.GetFloat(AudioManager.Instance.sfxSavedValue); // Actualiza el slider de efectos de sonido
        muteBmgChecker.isOn = AudioManager.Instance.isMuteBgm; // Actualiza el toggle de silencio
        muteSfxChecker.isOn = AudioManager.Instance.isMuteSfx; // Actualiza el toggle de silencio
    }

    public void ShowMenu(int index)
    {
        switch (index)
        {
            case 0:
                transitionAnimator.SetInteger("menuIndex", 0);
                break;
            case 1:
                transitionAnimator.SetInteger("menuIndex", 1);
                break;
            default:
                transitionAnimator.SetInteger("menuIndex", 0);
                break;
        }
    }

}
