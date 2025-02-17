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
    }

    // Start is called before the first frame update
    void Start()
    {
        musicAudio = transform.GetChild(0).GetComponent<AudioSource>();
        sampleLoop = GetComponentInChildren<SampleAccurateLoop>();
        sfxAudio = transform.GetChild(1).GetComponent<AudioSource>();

        
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
