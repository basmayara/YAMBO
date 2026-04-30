using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System.Collections;

public class ProfilPanel : MonoBehaviour
{
    [Header("=== TEXTES ===")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI scoreTotalText;
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI gamesPlayedText;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI avatarInitialsText;

    [Header("=== BARRE XP ===")]
    public Slider xpSlider;

    [Header("=== BOUTONS ===")]
    public Button playButton;
    public Button settingsButton;
    public Button logoutButton;
    public Button closeButton;

    [Header("=== PANELS ===")]
    public GameObject menuPanel;
    public GameObject settingsPanel;
    public GameObject loginPanel;

    // ← Mettez ici l'ID Firebase de votre joueur test
    private string testUserId = "64KMRLfqVOm2Fs8oQytp";

    void Start()
    {
        playButton.onClick.AddListener(OnPlayClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        logoutButton.onClick.AddListener(OnLogoutClicked);
        closeButton.onClick.AddListener(OnCloseClicked);
    }

    void OnEnable()
    {
        Debug.Log("ProfilPanel ENABLED");
        Debug.Log("ApiManager existe : " + (ApiManager.Instance != null));
        Debug.Log("UserId : " + (ApiManager.Instance?.UserId ?? "VIDE"));
        Debug.Log("testUserId : " + testUserId);
        if (ApiManager.Instance != null &&
            !string.IsNullOrEmpty(ApiManager.Instance.UserId))
        {
            // Joueur connecté via Gateway
            StartCoroutine(ChargerDepuisGateway(ApiManager.Instance.UserId));
        }
        else if (ApiManager.Instance != null)
        {
            // Test avec ID Firebase direct
            StartCoroutine(ChargerDepuisGateway(testUserId));
        }
        else
        {
            // Fallback Firebase direct
            _ = LoadAndRefresh();
        }
    }

    IEnumerator ChargerDepuisGateway(string userId)
    {
        Debug.Log("Chargement profil pour userId : " + userId);
        yield return StartCoroutine(
            ApiManager.Instance.GetProfilById(userId, profilJson =>
            {
                if (profilJson != null)
                {
                    Debug.Log("Profil reçu : " + profilJson);
                    var data = JsonUtility.FromJson<PlayerApiData>(profilJson);
                    if (data != null && SaveManager.Instance != null)
                    {
                        SaveManager.Instance.currentData.playerName = data.playerName ?? "Joueur";
                        SaveManager.Instance.currentData.scoreTotal = data.scoreTotal;
                        SaveManager.Instance.currentData.bestScore = data.bestScore;
                        SaveManager.Instance.currentData.level = data.level > 0 ? data.level : 1;
                        SaveManager.Instance.currentData.lives = data.lives > 0 ? data.lives : 3;
                        SaveManager.Instance.currentData.gamesPlayed = data.gamesPlayed;
                        SaveManager.Instance.currentData.xpCurrent = data.xpCurrent;
                        SaveManager.Instance.currentData.xpMax = data.xpMax > 0 ? data.xpMax : 10000;
                    }
                }
                RefreshUI();
            }));
    }

    async Task LoadAndRefresh()
    {
        if (SaveManager.Instance == null) return;
        await SaveManager.Instance.LoadData();
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (SaveManager.Instance == null) return;

        PlayerData data = SaveManager.Instance.currentData;

        if (playerNameText) playerNameText.text = data.playerName;
        if (levelText) levelText.text = "Level " + data.level;
        if (scoreTotalText) scoreTotalText.text = FormatNumber(data.scoreTotal);
        if (bestScoreText) bestScoreText.text = FormatNumber(data.bestScore);
        if (livesText) livesText.text = data.lives.ToString();
        if (gamesPlayedText) gamesPlayedText.text = data.gamesPlayed.ToString();
        if (xpText) xpText.text = FormatNumber(data.xpCurrent)
                                                       + " / " + FormatNumber(data.xpMax) + " XP";
        if (avatarInitialsText)
            avatarInitialsText.text = data.playerName.Length >= 2
                ? data.playerName.Substring(0, 2).ToUpper()
                : data.playerName.ToUpper();

        if (xpSlider)
        {
            xpSlider.minValue = 0;
            xpSlider.maxValue = data.xpMax;
            xpSlider.value = data.xpCurrent;
        }
    }

    void OnPlayClicked()
    {
        gameObject.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    void OnSettingsClicked()
    {
        gameObject.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(true);
    }

    void OnLogoutClicked()
    {
        if (FirebaseManager.Instance != null)
            FirebaseManager.Instance.Logout();
        gameObject.SetActive(false);
        if (loginPanel) loginPanel.SetActive(true);
    }

    void OnCloseClicked()
    {
        gameObject.SetActive(false);
        if (menuPanel) menuPanel.SetActive(true);
    }

    string FormatNumber(int n)
    {
        return n.ToString("N0",
            System.Globalization.CultureInfo.GetCultureInfo("fr-FR"));
    }

    [System.Serializable]
    public class PlayerApiData
    {
        public string playerName;
        public int scoreTotal;
        public int bestScore;
        public int level;
        public int lives;
        public int gamesPlayed;
        public int xpCurrent;
        public int xpMax;
    }
}