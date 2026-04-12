using UnityEngine;
using TMPro;
using System.IO;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;

    [Header("UI References (Top Bar)")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI diamondText;

    [Header("Result UI (Try Again Screen)")]
    // Hado homa li ghadi t-jrr fihom l-Text d l-panel jdid
    public TextMeshProUGUI finalCoinsText;
    public TextMeshProUGUI finalDiamondsText;
    public GameObject resultPanel;

    [Header("Data")]
    public int currentCoins = 0;
    public int currentDiamonds = 0;

    private string path;

    void Awake()
    {
        if (instance == null) instance = this;
        path = Application.persistentDataPath + "/gameData.json";
        LoadData();
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateUI();
        SaveData();
    }

    public void AddDiamonds(int amount)
    {
        currentDiamonds += amount;
        UpdateUI();
        SaveData();
    }

    public void UpdateUI()
    {
        if (coinText != null) coinText.text = currentCoins.ToString();
        if (diamondText != null) diamondText.text = currentDiamonds.ToString();
    }

    // --- HAD L-FONCTION JDIDA BACH T-AFICHI L-PANEL ---
    public void ShowResult()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true); // Kat-tla3 l-Canva

            // HNA FIN KHASS L-UPDATE:
            if (finalCoinsText != null)
                finalCoinsText.text = currentCoins.ToString();

            if (finalDiamondsText != null)
                finalDiamondsText.text = currentDiamonds.ToString();

            Debug.Log("Panel Updated! Coins: " + currentCoins);
        }
    }

    public void SaveData()
    {
        GameData data = new GameData();
        data.coins = currentCoins;
        data.diamonds = currentDiamonds;
        File.WriteAllText(path, JsonUtility.ToJson(data));
    }

    public void LoadData()
    {
        if (File.Exists(path))
        {
            GameData data = JsonUtility.FromJson<GameData>(File.ReadAllText(path));
            currentCoins = data.coins;
            currentDiamonds = data.diamonds;
            UpdateUI();
        }
    }

    // HAD L-FONCTION BACH T-7EYYED L-ERROR CS1061
    public void ResetScore()
    {
        currentCoins = 0;
        // currentDiamonds = 0; // Ila bghiti 7ta diamonds i-r-j3o 0
        SaveData();
        UpdateUI();
    }
}

[System.Serializable]
public class GameData { public int coins; public int diamonds; }