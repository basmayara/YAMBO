using UnityEngine;

public class FireHit : MonoBehaviour
{
    private bool hasHit = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        if (collision.CompareTag("Player"))
        {
            hasHit = true;

            PlayerHealth health = collision.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(1);
            }

            Destroy(gameObject); 
        }
    }
}
