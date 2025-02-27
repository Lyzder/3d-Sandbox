using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveHeight;
    public float moveDistance;
    public float speed;
    public float waitTime;

    private Vector3 startPos, targetPos;
    private bool movingToTarget = true;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Get Rigidbody
        startPos = transform.position;
        targetPos = startPos + new Vector3(moveDistance, moveHeight, 0);
        StartCoroutine(MovePlatform());
    }

    IEnumerator MovePlatform()
    {
        while (true)
        {
            Vector3 destination = movingToTarget ? targetPos : startPos;

            while (Vector3.Distance(transform.position, destination) > 0.01f)
            {
                Vector3 newPos = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                rb.MovePosition(newPos);  // Use MovePosition for proper movement
                yield return null;
            }

            yield return new WaitForSeconds(waitTime);
            movingToTarget = !movingToTarget;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}
