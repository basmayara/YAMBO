using UnityEngine;
using System.Threading.Tasks;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public PlayerData currentData = new PlayerData();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // CHARGER au démarrage
    public async Task LoadData()
    {
        if (FirebaseManager.Instance == null ||
            FirebaseManager.Instance.currentUser == null) return;

        PlayerData loaded = await FirebaseManager.Instance.LoadPlayerData();

        if (loaded != null)
        {
            currentData = loaded;
            Debug.Log("Données chargées : " + currentData.playerName);
        }
    }

    // SAUVEGARDER
    public async Task SaveData()
    {
        if (FirebaseManager.Instance == null ||
            FirebaseManager.Instance.currentUser == null) return;

        await FirebaseManager.Instance.SavePlayerData(currentData);
        Debug.Log("Données sauvegardées !");
    }

    // SAUVEGARDER automatiquement
    public async void AutoSave()
    {
        await SaveData();
    }

    // Mettre à jour le score
    public void UpdateScore(int newScore)
    {
        currentData.scoreTotal += newScore;

        if (newScore > currentData.bestScore)
            currentData.bestScore = newScore;

        // Gagner de l'XP
        currentData.xpCurrent += newScore / 10;

        // Monter de niveau
        if (currentData.xpCurrent >= currentData.xpMax)
        {
            currentData.xpCurrent = 0;
            currentData.level++;
            currentData.xpMax = currentData.xpMax + 5000;
            Debug.Log("Niveau supérieur ! Niveau " + currentData.level);
        }

        AutoSave();
    }

    // Perdre une vie
    public void LoseLife()
    {
        if (currentData.lives > 0)
        {
            currentData.lives--;
            AutoSave();
        }
    }

    // Réinitialiser les vies
    public void ResetLives()
    {
        currentData.lives = 3;
        AutoSave();
    }

    // Incrémenter parties jouées
    public void AddGame()
    {
        currentData.gamesPlayed++;
        AutoSave();
    }
    // Recevoir les données depuis le script de votre camarade
    public void SetPlayerDataFromLogin(string name, string email, string userId)
    {
        currentData.playerName = name;
        currentData.email = email;
        currentData.userId = userId;
        Debug.Log("Données reçues : " + name);
    }

    // Vérifier si un joueur est connecté
    public bool IsLoggedIn()
    {
        if (FirebaseManager.Instance == null) return false;
        return FirebaseManager.Instance.currentUser != null;
    }
}