using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public GameObject firePrefab;
    public Transform firePoint;
    public float fireRate = 2f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= fireRate)
        {
            Shoot();
            timer = 0f;
        }
    }

    void Shoot()
    {
        Instantiate(firePrefab, firePoint.position, Quaternion.identity);
    }
}