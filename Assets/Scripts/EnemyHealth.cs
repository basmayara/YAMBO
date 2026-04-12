using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 2;

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
            Destroy(gameObject);
    }
}