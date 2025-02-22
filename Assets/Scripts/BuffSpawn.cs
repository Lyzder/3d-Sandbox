using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSpawn : MonoBehaviour
{

    [SerializeField] GameObject ammoBuff;
    public float respawnTime;
    public bool isEnabled;

    private float timer;
    private GameObject childBuff;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled && childBuff == null)
            SpawnCountdown();
    }

    private void SpawnCountdown()
    {
        timer += Time.deltaTime;
        if (timer >= respawnTime)
        {
            childBuff = GameObject.Instantiate(ammoBuff, transform, true);
            timer = 0;
        }
    }
}
