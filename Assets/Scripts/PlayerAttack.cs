using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject battlePrefab;   
    public Transform firePoint;
    public float cooldown = 0.5f;

    float lastShootTime;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && Time.time > lastShootTime)
        {
            Shoot();
            lastShootTime = Time.time + cooldown;
        }
    }

    void Shoot()
    {
        Instantiate(battlePrefab, firePoint.position, firePoint.rotation);
    }
}