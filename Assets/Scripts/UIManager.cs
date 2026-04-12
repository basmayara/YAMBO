using UnityEngine;
using UnityEngine.SceneManagement; // HADI darori t-kon! Hna fin saken l-SceneManager

public class UIManager : MonoBehaviour
{
    public void TryAgain()
    {
        // 1. Kankhdmo b-Script dyalk (CoinManager) bach n-re-zero-wiw l'score
        if (CoinManager.instance != null)
        {
            CoinManager.instance.ResetScore();
        }

        // 2. Kankhdmo b-Outil dyal Unity (SceneManager) bach n-reload-iw l'map
        // Reddi l-bal: mat-ktbich CoinManager.LoadScene! 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}