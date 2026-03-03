using UnityEngine;

public class Batlle : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 1;

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
                enemy.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}