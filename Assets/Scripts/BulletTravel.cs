using UnityEngine;

public class BulletTravel : MonoBehaviour
{
    public float bulletSpeed;
    public float aliveTime;
    public GameObject impactEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        aliveTime -= Time.deltaTime;
        if (aliveTime > 0)
        {
            transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 hitPoint = transform.position; // Default to bullet position

        // Perform a raycast backward to find the exact impact point
        if (Physics.Raycast(transform.position - transform.forward, transform.forward, out RaycastHit hit, bulletSpeed * Time.deltaTime * 2))
        {
            hitPoint = hit.point;
        }

        // Instantiate impact effect at the hit point
        if (impactEffect != null)
        {
            Instantiate(impactEffect, hitPoint, Quaternion.identity);
        }

        // If the bullet hits a target, destroy it
        if (other.CompareTag("Target"))
            Destroy(other.gameObject);

        // Bullet is always destroyed upon impact
        Destroy(gameObject);
    }
}
