using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] int winningScore;
    [SerializeField] Transform flagTransform;
    private int destroyedTargets;
    [SerializeField] float time;
    private float flagMovement;
    private bool wonFlag, lostFlag;
    [SerializeField] bool timed;

    public static event Action OnScoring;
    public static event Action OnWinning;
    public static event Action OnLosing;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    private void Awake()
    {
        timed = false;
    }

    private void OnEnable()
    {
        Target.OnTargetDestroyed += TargetDestroyed;
    }

    private void OnDisable()
    {
        Target.OnTargetDestroyed -= TargetDestroyed;
    }

    // Start is called before the first frame update
    void Start()
    {
        ushort difficulty = GameManager.Instance.GetDifficulty();
        destroyedTargets = 0;
        wonFlag = false;
        lostFlag = false;
        if (difficulty == 0) { 
            EnableTimer(false);
            winningScore = 15;
        }
        else if (difficulty == 1)
        {
            EnableTimer(true);
            SetTime(180);
            winningScore = 25;
        }
        else
        {
            EnableTimer(true);
            SetTime(90);
            winningScore = 25;
        }
        flagMovement = 7.5f / winningScore;
        UpdateScore(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!wonFlag && !lostFlag && timed)
            UpdateTimer();
    }

    private void TargetDestroyed()
    {
        if (wonFlag || lostFlag)
            return;

        destroyedTargets++;
        OnScoring?.Invoke();
        RaiseFlag();
        if (destroyedTargets >= winningScore)
        {
            UpdateScore(true);
            OnWinning?.Invoke();
            wonFlag = true;
        }
        else
            UpdateScore(false);
    }

    private void UpdateScore(bool won)
    {
        if (won)
            scoreText.text = "Good job!";
        else
            scoreText.text = winningScore - destroyedTargets + " remaining";
    }

    private void RaiseFlag()
    {
        flagTransform.localPosition += new Vector3(0, flagMovement, 0);
    }

    /// <summary>
    /// Sets the time limit in seconds
    /// </summary>
    /// <param name="time"></param>
    public void SetTime(float time)
    {
        this.time = time;
    }

    /// <summary>
    /// Enables or disables the timer
    /// </summary>
    /// <param name="flag"></param>
    public void EnableTimer(bool flag)
    {
        timed = flag;
        timerText.enabled = flag;
    }

    private void UpdateTimer()
    {
        time -= Time.deltaTime;

        if (time <= 30)
            timerText.color = Color.red;
        if (time <= 0)
        {
            time = 0;
            lostFlag = true;
            OnLosing?.Invoke();
        }
        TimeSpan timeString = TimeSpan.FromSeconds(Math.Ceiling(time));
        timerText.text = timeString.ToString("mm':'ss");
    }
}
