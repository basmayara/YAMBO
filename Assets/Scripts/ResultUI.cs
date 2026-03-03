using UnityEngine;
using TMPro;

public class ResultUI : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText; // Text ديال TOTAL SCORE

    void OnEnable()
    {
        // كنجلبو Score الحالي من ScoreManager
        finalScoreText.text = ScoreManager.instance.GetScore().ToString();
    }
}