using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveHeight;     // Vertical movement distance
    public float moveDistance;   // Horizontal movement distance
    public float speed;          // Movement speed
    public float waitTime;       // Wait time at each position

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool movingToTarget = true;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + new Vector3(moveDistance, moveHeight, 0);  // Adjust movement direction here
        StartCoroutine(MovePlatform());
    }

    IEnumerator MovePlatform()
    {
        while (true)
        {
            Vector3 destination = movingToTarget ? targetPos : startPos;

            // Move towards the destination
            while (Vector3.Distance(transform.position, destination) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                yield return null;
            }

            // Wait at the destination
            yield return new WaitForSeconds(waitTime);

            // Swap movement direction
            movingToTarget = !movingToTarget;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
