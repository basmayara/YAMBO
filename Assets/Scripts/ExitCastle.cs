using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitCastle : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // رجوع للـ Scene الأساسية (تأكدي من السمية)
            SceneManager.LoadScene("Map");
        }
    }
}