using UnityEngine;
using UnityEngine.SceneManagement; // ضرورية لإعادة تشغيل اللعبة

public class GameManager : MonoBehaviour
{
    // هاد الخانة هي فين غاتجري الـ Canvas اللي طفيتي (Dead Zone)
    public GameObject restartCanvas;
    public GameObject resultPanel;

    public void ShowRestartUI()
    {
        if (restartCanvas != null)
        {
            restartCanvas.SetActive(true); // كيشعل الـ Canvas فاش كتموتي
            // Time.timeScale = 0f; // إلا بغيتي اللعبة توقف فاش يطلع المنيو
        }
    }

    public void RestartGame()
    {
        // Time.timeScale = 1f; // ضروري ترجع اللعبة تتحرك قبل ما تشارجي المشهد
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver()
    {
        resultPanel.SetActive(true);
    }
}