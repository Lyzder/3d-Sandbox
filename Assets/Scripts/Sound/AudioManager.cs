using System.Collections;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] AudioSource sfxAudio, musicAudio;
    [SerializeField] AudioMixer bgmMixer, sfxMixer;
    [SerializeField] SampleAccurateLoop sampleLoop;

    public bool isMuteBgm;
    public bool isMuteSfx;
    public string musicSavedValue = "musicValue";
    public string sfxSavedValue = "sfxValue";
    public string musicIsMuted = "musicMuted";
    public string sfxIsMuted = "sfxMuted";

    private BgmMetadata bgmMetadata;

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
        StopMusic();
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
        StopMusic();

        sampleLoop.loopStartSample = loopStart;
        sampleLoop.loopEndSample = loopEnd;

        musicAudio.clip = clip;
        StartCoroutine(PlayWhenReady(musicAudio));
    }

    public void StopMusic()
    {
        sampleLoop.SetPlaying(false);
        musicAudio.loop = false;
        musicAudio.Stop();
        musicAudio.time = 0;
    }

    public void PlaySceneBgm()
    {
        int[] loop;
        try
        {
            bgmMetadata = FindAnyObjectByType<BgmMetadata>();
            loop = bgmMetadata.GetLoopSamples();
            musicAudio.volume = bgmMetadata.GetVolume();
            sampleLoop.fileSampleRate = bgmMetadata.GetSampleRate();
            PlayMusic(bgmMetadata.GetBgmClip(), loop[0], loop[1]);
        }
        catch
        {
            Debug.Log("Could not load prepared music");
        }
    }

    /// <summary>
    /// Sets the volume of the BGM mixer using a 0.0 to 1.0 logarithmic scale
    /// </summary>
    /// <param name="volume">Volume in a logarithmic scale</param>
    public void SetMusicVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;

        bgmMixer.SetFloat("Volume_Master", dB);
    }

    /// <summary>
    /// Sets the volume of the BGM mixer
    /// </summary>
    /// <param name="volume">Volume in decibels</param>
    public void SetMusicVolume(int volume)
    {
        bgmMixer.SetFloat("Volume_Master", volume);
    }

    /// <summary>
    /// Sets the volume of the SFX mixer using a 0.0 to 1.0 logarithmic scale
    /// </summary>
    /// <param name="volume">Volume in a logarithmic scale</param>
    public void SetSfxVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;

        sfxMixer.SetFloat("Volume_Master", dB);
    }

    /// <summary>
    /// Sets the volume of the SFX mixer
    /// </summary>
    /// <param name="volume">Volume in decibels</param>
    public void SetSfxVolume(int volume)
    {
        sfxMixer.SetFloat("Volume_Master",volume);
    }

    public void ToggleMuteMusic(float sliderValue)
    {

        if (!isMuteBgm)
        {
            SetMusicVolume(0f);
            PlayerPrefs.SetFloat(musicSavedValue, sliderValue);
            PlayerPrefs.SetInt(musicIsMuted, 1);
        }
        else
        {
            PlayerPrefs.SetInt(musicIsMuted, 0);
            SetMusicVolume(PlayerPrefs.GetFloat(musicSavedValue));
        }
        isMuteBgm = !isMuteBgm;
    }

    public void ToggleMuteSfx(float sliderValue)
    {

        if (!isMuteSfx)
        {
            SetSfxVolume(0f);
            PlayerPrefs.SetFloat(sfxSavedValue, sliderValue);
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
        sampleLoop.ResetPosition();
        sampleLoop.SetPlaying(true);
        audioSource.Play();
    }
}
