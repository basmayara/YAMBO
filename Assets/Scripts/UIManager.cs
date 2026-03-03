using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void TryAgain()
    {
        ScoreManager.instance.ResetScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}