using UnityEngine;

public class Fire : MonoBehaviour
{
    public GameObject firePrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    public float fireSpeed = 5f;

    private float nextFireTime;
    private bool playerInZone;

    void Update()
    {
        if (playerInZone && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void Shoot()
    {
        GameObject fire = Instantiate(firePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = fire.GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.velocity = Vector2.left * fireSpeed;

        Destroy(fire, 3f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInZone = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInZone = false;
    }
}
