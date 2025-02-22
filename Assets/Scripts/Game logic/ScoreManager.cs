using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] int winningScore;
    [SerializeField] Transform flagTransform;
    private int destroyedTargets;
    private float flagMovement;
    private bool wonFlag, lostFlag;

    public static event Action OnScoring;
    public static event Action OnWinning;
    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        flagMovement = 7.5f / winningScore;
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
        destroyedTargets = 0;
        wonFlag = false;
        lostFlag = false;
        updateScore(false);
    }

    // Update is called once per frame
    void Update()
    {
        
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
            updateScore(true);
            OnWinning?.Invoke();
            wonFlag = true;
        }
        else
            updateScore(false);
    }

    private void updateScore(bool won)
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
}
