using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TextMeshProUGUI scoreText;
    int score = 0;

    void Awake()
    {
        instance = this;
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString();
    }

    public int GetScore()
    {
        return score;
    }
    public void ResetScore()
    {
        score = 0;
        scoreText.text = "0";
    }
}