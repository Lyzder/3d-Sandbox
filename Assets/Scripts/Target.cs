using System;
using UnityEngine;

public class Target : MonoBehaviour
{
    public static event Action OnTargetDestroyed;
    public AudioClip destroyedSfx;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            OnTargetDestroyed?.Invoke();
            AudioManager.Instance.PlaySFX(destroyedSfx);
            Destroy(gameObject);
        }
    }
}
