using UnityEngine;
using TMPro;

public class ResultUI : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;

    void OnEnable()
    {
        // Check darori bach mat-crashich l'game
        if (ScoreManager.instance != null && finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + ScoreManager.instance.GetScore();
        }
    }
}