using UnityEngine;
using TMPro;
using System.IO;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TextMeshProUGUI scoreText; // Branchi hada f kol Scene!
    private int currentScore = 0;
    private string path;

    void Awake()
    {
        instance = this;
        path = Application.persistentDataPath + "/save.json";
        LoadScore(); // Auto-load melli kat-switchi l'map
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateUI();
        SaveScore();
    }

    public void UpdateUI()
    {
        if (scoreText != null) scoreText.text = currentScore.ToString();
    }

    public void SaveScore()
    {
        File.WriteAllText(path, currentScore.ToString());
    }

    public void LoadScore()
    {
        if (File.Exists(path))
        {
            string val = File.ReadAllText(path);
            currentScore = int.Parse(val);
            UpdateUI();
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        SaveScore();
        UpdateUI();
    }
    public int GetScore()
    {
        return currentScore;
    }
}