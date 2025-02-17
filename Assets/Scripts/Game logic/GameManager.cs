using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] AudioMixer bgmMixer;
    [SerializeField] bool isPaused, lostFlag, wonFlag;
    private TargetSpawner targetSpawner;
    private float timer;

    public AudioClip winCheer;

    private void OnEnable()
    {
        ScoreManager.OnWinning += WinGame;
    }

    private void OnDisable()
    {
        ScoreManager.OnWinning -= WinGame;
    }

    // Start is called before the first frame update
    void Start()
    {
        targetSpawner = FindAnyObjectByType<TargetSpawner>();
        timer = 0;
        targetSpawner.canSpanw = true;
        AudioManager.Instance.SetMusicVolume(0.18f);
        AudioManager.Instance.PlayMusic(AudioManager.Instance.InitialMusic, 1290240, 5274964);
    }

    // Update is called once per frame
    void Update()
    {
        if (wonFlag)
        {
            timer += Time.deltaTime;

            if (timer >= 3.0f)
            {
                Time.timeScale = 1.0f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        else if (lostFlag)
        {
            GameOverLose(true);
        }
    }

    public void GameOverLose(bool lose)
    {
        if (lose)
        {
            lostFlag = true;
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void WinGame()
    {
        wonFlag = true;
        Time.timeScale = 0.5F;
        targetSpawner.canSpanw = false;
        AudioManager.Instance.PlaySFX(winCheer);
    }
}
