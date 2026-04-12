using UnityEngine;

public class CodeFragment : MonoBehaviour
{
    public int digitValue;      // Raqm (masalan 8)
    public int positionInCode; // Blasto (0 l'lowel, 1 l'tani...)
    public GameObject effectPrefab; // Chi effect dyal l'ghobra melli i-t-qiss

    private bool isCollected = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ila l'player n-qez fouq l'objet (check b-Tag awla Velocity)
        if (collision.gameObject.CompareTag("Player") && !isCollected)
        {
            isCollected = true;

            // Sifet l'raqm l'CodeManager
            CodeManager.instance.CollectDigit(digitValue, positionInCode);

            // Dir effect w ms7 l'objet
            if (effectPrefab != null) Instantiate(effectPrefab, transform.position, Quaternion.identity);
        }
    }
}