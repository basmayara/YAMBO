using UnityEngine;
using System.Collections;

public class WaterDeath : MonoBehaviour
{
    public float restartDelay = 1.0f;
    public Transform respawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(1);

                if (PlayerHealth.currentHealth > 0)
                {
                    StartCoroutine(HandleWaterFall(other.gameObject));
                }
            }
        }
    }

    IEnumerator HandleWaterFall(GameObject player)
    {
        Animator anim = player.GetComponent<Animator>();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        // Hna khoudi l-script d l-movement dyalk (beddli PlayerController b smiya li 3ndek)
        MonoBehaviour moveScript = player.GetComponent<MonoBehaviour>();

        // 1. 7ebsi l-player kamel
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false; // Bach may-t-7rrekch m3a l-ma
        }

        if (anim != null) anim.SetTrigger("Die");

        yield return new WaitForSeconds(restartDelay);

        // --- RE-SPAWN ---

        // 2. Rj3ih l-blastu
        if (respawnPoint != null)
            player.transform.position = respawnPoint.position;

        // 3. Rj3i l-physique y-khdem
        if (rb != null) rb.simulated = true;

        // 4. L-7ell dyal l-bloquage:
        if (anim != null)
        {
            anim.Rebind(); // Had l-ligne kat-mrechi l-animator men jdid (Reset)
            anim.Update(0f); // Kat-khlih y-calculi l-position jdida

            // Bla ma t-play-i Idle manual, Rebind ghadi trj3o l-Entry state 
            // li ghadi t-khlli l-movement dyalk i-t7kkem fih
        }

        // 5. Ila knti m-habsa l-script d l-movement f "TakeDamage", t-akdi t-rj3ih
        // player.GetComponent<PlayerController>().enabled = true;

        Debug.Log("Player is back to normal!");
    }
}