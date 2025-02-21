using System.Collections;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] AudioSource sfxAudio, musicAudio;
    [SerializeField] AudioMixer bgmMixer, sfxMixer;
    [SerializeField] SampleAccurateLoop sampleLoop;

    public AudioClip InitialMusic;
    public bool isMuteBgm;
    public bool isMuteSfx;
    public string musicSavedValue = "musicValue";
    public string sfxSavedValue = "sfxValue";
    public string musicIsMuted = "musicMuted";
    public string sfxIsMuted = "sfxMuted";

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

        musicAudio = transform.GetChild(0).GetComponent<AudioSource>();
        sampleLoop = GetComponentInChildren<SampleAccurateLoop>();
        sfxAudio = transform.GetChild(1).GetComponent<AudioSource>();
        isMuteBgm = false;
        isMuteSfx = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Plays a sound effect file
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySFX(AudioClip clip)
    {
        sfxAudio.PlayOneShot(clip);
    }

    /// <summary>
    /// Plays a music file with a basic start-to-end loop
    /// </summary>
    /// <param name="clip"></param>
    public void PlayMusic(AudioClip clip)
    {
        musicAudio.Stop();
        musicAudio.clip = clip;
        musicAudio.Play();
        musicAudio.loop = true;
    }

    /// <summary>
    /// Plays a music file looping from a start sample to an end sample
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="sampleRate"></param>
    /// <param name="loopStart"></param>
    /// <param name="loopEnd"></param>
    public void PlayMusic(AudioClip clip, int loopStart, int loopEnd)
    {
        musicAudio.Stop();

        sampleLoop.loopStartSample = loopStart;
        sampleLoop.loopEndSample = loopEnd;

        musicAudio.clip = clip;
        StartCoroutine(PlayWhenReady(musicAudio));
    }

    public void SetMusicVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;

        bgmMixer.SetFloat("Volume_Master", dB);
    }

    public void SetSfxVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;

        sfxMixer.SetFloat("Volume_Master", dB);
    }

    public void ToggleMuteMusic()
    {
        float currentVolume;

        if (!isMuteBgm)
        {
            bgmMixer.GetFloat("Volume_Master", out currentVolume);
            SetMusicVolume(0);
            PlayerPrefs.SetFloat(musicSavedValue, currentVolume);
            PlayerPrefs.SetInt(musicIsMuted, 1);
        }
        else
        {
            PlayerPrefs.SetInt(musicIsMuted, 0);
            SetMusicVolume(PlayerPrefs.GetFloat(musicSavedValue));
        }

        isMuteBgm = !isMuteBgm;
    }

    public void ToggleMuteSfx()
    {
        float currentVolume;

        if (!isMuteSfx)
        {
            sfxMixer.GetFloat("Volume_Master", out currentVolume);
            SetSfxVolume(0);
            PlayerPrefs.SetFloat(sfxSavedValue, currentVolume);
            PlayerPrefs.SetInt(sfxIsMuted, 1);
        }
        else
        {
            PlayerPrefs.SetInt(sfxIsMuted, 0);
            SetMusicVolume(PlayerPrefs.GetFloat(sfxSavedValue));
        }

        isMuteSfx = !isMuteSfx;
    }

    public void SaveSoundPreferences(float levelMusic, float levelSFX)
    {
        PlayerPrefs.SetFloat(musicSavedValue, levelMusic);
        PlayerPrefs.SetFloat(sfxSavedValue, levelSFX);
        if (isMuteBgm)
            PlayerPrefs.SetInt(musicIsMuted, 1);
        else
            PlayerPrefs.SetInt(musicIsMuted, 0);
        if (isMuteSfx)
            PlayerPrefs.SetInt(sfxIsMuted, 1);
        else
            PlayerPrefs.SetInt(sfxIsMuted, 0);
    }

    public void LoadSoundPreferences()
    {
        if (PlayerPrefs.HasKey(musicSavedValue))
        {
            SetMusicVolume(PlayerPrefs.GetFloat(musicSavedValue));
            SetSfxVolume(PlayerPrefs.GetFloat(sfxSavedValue));
            if (PlayerPrefs.GetInt(musicIsMuted) == 1)
                isMuteBgm = true;
            else
                isMuteBgm = false;
            if (PlayerPrefs.GetInt(sfxIsMuted) == 1)
                isMuteSfx = true;
            else
                isMuteSfx = false;
        }
    }

    IEnumerator PlayWhenReady(AudioSource audioSource)
    {
        while (audioSource.clip.loadState != AudioDataLoadState.Loaded)
        {
            yield return null;
        }
        sampleLoop.PrepareClip();
        audioSource.Play();
        musicAudio.loop = true;
    }
}
