using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public Vector3 moveDirection = new Vector3(0f, 0f, 1f);  // Direction to move (right on the x-axis)
    public float moveSpeed = 0.1f;  // Speed of movement per frame
    private int i;
    private int dir;
    // Start is called before the first frame update
    void Start()
    {
        dir = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (i == 60)
        {
            dir = -1;
        }
        else if (i == -60)
        {
            dir = 1;
        }
        moveDirection = new Vector3(0f, 0f, 1f * dir);
        transform.position += moveDirection * moveSpeed;
        i += dir;
    }
}
