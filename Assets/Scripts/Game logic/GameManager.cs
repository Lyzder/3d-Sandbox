using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action OnStart;

    [SerializeField] bool isPaused, lostFlag, wonFlag;
    private GameObject hardPlatforms;
    private ushort difficulty;
    private float timer;
    private Transform spawnPoint;
    
    public GameObject playerPrefab;
    public AudioClip winCheer;
    public bool gameStarted= false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            ScoreManager.OnWinning += WinGame;
        }
        else
        {
            Destroy(this.gameObject);
        }
        difficulty = 0;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe to avoid memory leaks
        ScoreManager.OnWinning -= WinGame;
    }

    // Start is called before the first frame update
    void Start()
    {
        
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

    public void StartGame()
    {
        timer = 0;

        if (difficulty == 0)
        {
            hardPlatforms.SetActive(false);
        }
        SpawnPlayer();
        UIManager.Instance.Activate();
        gameStarted = true;
        wonFlag = false;
        lostFlag = false;
        OnStart?.Invoke();
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
        AudioManager.Instance.PlaySFX(winCheer);
    }

    public void SpawnPlayer()
    {
        //TODO
        try
        {
            spawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;
            Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        catch {
            Instantiate(playerPrefab, new Vector3(0, 0, 0), new Quaternion());
        }
        HudManager.Instance.InitializeHud();
    }

    public void SetDifficulty(ushort difficultyIndex)
    {
        difficulty = difficultyIndex;
    }

    public ushort GetDifficulty()
    {
        return difficulty;
    }

    public void TransitionToScene(ushort sceneIndex)
    {
        switch (sceneIndex)
        {
            case 0:
                SceneManager.LoadScene("MainMenu");
                break;
            case 1:
                SceneManager.LoadScene("TestingGrounds");
                break;
            default:
                SceneManager.LoadScene("MainMenu");
                break;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioManager.Instance.PlaySceneBgm();
        if (scene.name == "TestingGrounds")
        {
            hardPlatforms = GameObject.FindGameObjectWithTag("HardMode");
            StartGame();
        }
    }
}
