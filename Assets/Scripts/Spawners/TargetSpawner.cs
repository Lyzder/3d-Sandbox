using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField] GameObject obstacle;
    public float time;
    public bool canSpawn;
    public bool hardMode;

    private GameObject spawnChild;
    private ushort difficulty;

    private void Awake()
    {
        GameManager.OnStart += EnableSpawn;
        ScoreManager.OnWinning += DisableSpawn;
        ScoreManager.OnLosing += DisableSpawn;

        difficulty = 0;
    }

    private void OnDestroy()
    {
        GameManager.OnStart -= EnableSpawn;
        ScoreManager.OnWinning -= DisableSpawn;
        ScoreManager.OnLosing -= DisableSpawn;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canSpawn)
        {
            if (spawnChild == null)
            {
                time -= Time.deltaTime;

                if (time <= 0)
                {
                    spawnChild = Instantiate(obstacle, transform.position, transform.rotation);
                    spawnChild.transform.parent = transform;
                    time = (float)Random.Range(5, difficulty == 0 ? 10 : 15);
                }
            }
        }
    }

    private void EnableSpawn()
    {
        difficulty = GameManager.Instance.GetDifficulty();
        if (difficulty == 0)
        {
            canSpawn = !hardMode;
            time = (float)Random.Range(3, 10);
        }
        else
        {
            canSpawn = true;
            time = (float)Random.Range(3, 15);
        }
    }

    private void DisableSpawn()
    {
        canSpawn = false;
    }
}
