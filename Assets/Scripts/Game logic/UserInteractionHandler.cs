using UnityEngine;

public class UserInteractionHandler : MonoBehaviour
{
    private AudioSource audioSource;
    private bool hasInteracted = false;

    public void Setup(AudioSource source)
    {
        audioSource = source;
    }

    private void Update()
    {
        if (!hasInteracted && (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
        {
            hasInteracted = true;
            audioSource.Play();
            Destroy(this); // Remove script once audio starts
        }
    }
}