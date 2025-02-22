using UnityEngine;
using System.Collections;

public class SimpleMovement : MonoBehaviour
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
        StartCoroutine(MoveObject());
    }

    IEnumerator MoveObject()
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
}
