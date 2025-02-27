using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action OnStart, OnGameOver, OnVictory;

    [SerializeField] bool isPaused, lostFlag, wonFlag;
    private GameObject hardPlatforms;
    private GameObject flagPole;
    private ushort difficulty;
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
            ScoreManager.OnLosing += LoseGame;
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
        ScoreManager.OnLosing -= LoseGame;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {

        if (difficulty == 0)
        {
            hardPlatforms.SetActive(false);
            flagPole.transform.position = new Vector3(-15, 0, 15);
        }
        SpawnPlayer();
        UIManager.Instance.Activate();
        gameStarted = true;
        wonFlag = false;
        lostFlag = false;
        OnStart?.Invoke();
    }

    public void LoseGame()
    {
        lostFlag = true;
        StartCoroutine(EndGameTransition(false));
    }

    private void WinGame()
    {
        wonFlag = true;
        Time.timeScale = 0.5F;
        AudioManager.Instance.PlaySFX(winCheer);
        StartCoroutine(EndGameTransition(true));
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
            flagPole = GameObject.FindGameObjectWithTag("Finish");
            StartGame();
        }
    }

    IEnumerator EndGameTransition(bool won)
    {
        if (won)
        {
            yield return new WaitForSecondsRealtime(3f);
            OnVictory?.Invoke();
        }
        else
            OnGameOver?.Invoke();
    }
}
