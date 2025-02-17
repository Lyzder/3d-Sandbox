using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    private int destroyedTargets;

    private void OnEnable()
    {
        ScoreManager.OnScoring += RaiseFlag;
    }

    private void OnDisable()
    {
        ScoreManager.OnScoring -= RaiseFlag;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RaiseFlag()
    {
        transform.localPosition += new Vector3(0, 0.5f, 0);
    }
}
