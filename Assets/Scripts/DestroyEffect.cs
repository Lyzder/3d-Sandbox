using System.Threading;
using UnityEngine;

public class KillEffect : MonoBehaviour
{

    private ParticleSystem _particleSystem;
    private float life;
    private float timer;
    private bool loop;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }
    // Start is called before the first frame update
    void Start()
    {
        life = _particleSystem.main.duration;
        loop = _particleSystem.main.loop;
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!loop)
            Countdown();
    }

    void Countdown()
    {
        timer += Time.deltaTime;
        if (timer >= life)
        {
            Destroy(gameObject);
        }
    }
}
