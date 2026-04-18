using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

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

    void Start()
    {
        playButton.onClick.AddListener(OnPlayClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        logoutButton.onClick.AddListener(OnLogoutClicked);
        closeButton.onClick.AddListener(OnCloseClicked);
    }

    async void OnEnable()
    {
        Debug.Log("ProfilPanel ENABLED");
        Debug.Log("SaveManager = " + SaveManager.Instance);

        await LoadAndRefresh();
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
}