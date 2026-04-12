using UnityEngine;
using System.Collections;

public class ChestD : MonoBehaviour
{
    private bool isOpened = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpened)
        {
            isOpened = true;

            // 1. Zid diamonds f CoinManager l'jdid
            if (CoinManager.instance != null)
            {
                CoinManager.instance.AddDiamonds(10);
            }
            else
            {
                Debug.LogError("CoinManager ma-m7toutch f l'map!");
            }

            // 2. Control d'animation (ybqa kima hwa)
            Animator playerAnim = other.GetComponent<Animator>();
            if (playerAnim != null)
            {
                StartCoroutine(HappyRoutine(playerAnim));
            }
        }
    }

    IEnumerator HappyRoutine(Animator anim)
    {
        // Khdem l'animation Happy
        anim.SetTrigger("Happy");
        Debug.Log("Filla Happy t-launchat!");

        // Tsennay 5 thwani
        yield return new WaitForSeconds(5f);

        // Trje3 l'Normal (khass ikon 3ndek Trigger smito 'Normal' f l'Animator)
        anim.SetTrigger("Normal");
        Debug.Log("Trj3at l'Idle.");
    }
}