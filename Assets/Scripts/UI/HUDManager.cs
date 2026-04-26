using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YAMBO.API;

namespace YAMBO.UI
{
    /// <summary>
    /// Global HUD that stays on screen during gameplay.
    /// Shows: balance, username, level, and quick panel toggles.
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        public static HUDManager Instance { get; private set; }

        [Header("HUD Elements")]
        public TextMeshProUGUI balanceText;
        public TextMeshProUGUI usernameText;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI quizCountText;

        [Header("Panels (toggled by buttons)")]
        public GameObject shopPanel;
        public GameObject inventoryPanel;
        public GameObject progressionPanel;

        [Header("Toggle Buttons")]
        public Button shopButton;
        public Button inventoryButton;
        public Button progressionButton;
        public Button logoutButton;

        [Header("Notification")]
        public GameObject       notificationBanner;
        public TextMeshProUGUI  notificationText;

        private int _currentBalance = 0;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }
        }

        void Start()
        {
            // Display username immediately from saved prefs
            if (usernameText != null)
                usernameText.text = APIClient.Instance?.GetUsername() ?? "Player";

            // Wire buttons
            shopButton?       .onClick.AddListener(() => TogglePanel(shopPanel));
            inventoryButton?  .onClick.AddListener(() => TogglePanel(inventoryPanel));
            progressionButton?.onClick.AddListener(() => TogglePanel(progressionPanel));
            logoutButton?     .onClick.AddListener(OnLogout);

            // Hide all panels at start
            shopPanel?        .SetActive(false);
            inventoryPanel?   .SetActive(false);
            progressionPanel? .SetActive(false);
            notificationBanner?.SetActive(false);

            // Load balance & progression
            RefreshBalance();
            RefreshProgression();
        }

        // ---- Public API ----

        public void RefreshBalance()
        {
            if (APIClient.Instance == null) return;
            StartCoroutine(APIClient.Instance.GetBalance(
                r =>
                {
                    _currentBalance = r.balance;
                    UpdateBalanceDisplay();
                },
                e => Debug.LogWarning("[HUD] Balance error: " + e)));
        }

        public void RefreshProgression()
        {
            if (APIClient.Instance == null) return;
            StartCoroutine(APIClient.Instance.GetProgression(
                r =>
                {
                    if (levelText     != null) levelText.text     = "Lvl " + r.level_reached;
                    if (quizCountText != null) quizCountText.text = r.quizzes_solved + "/" + r.quizzes_total;
                },
                e => Debug.LogWarning("[HUD] Progression error: " + e)));
        }

        /// <summary>Call this after any purchase to sync balance.</summary>
        public void OnPurchaseCompleted(int newBalance)
        {
            _currentBalance = newBalance;
            UpdateBalanceDisplay();
            ShowNotification("Purchase successful!");
        }

        /// <summary>Call this after a correct quiz answer.</summary>
        public void OnCurrencyEarned(int earned, int newBalance)
        {
            _currentBalance = newBalance;
            UpdateBalanceDisplay();
            ShowNotification("+" + earned + " C# earned!");
        }

        public void ShowNotification(string message, float duration = 2.5f)
        {
            if (notificationBanner == null) return;
            if (notificationText != null) notificationText.text = message;
            notificationBanner.SetActive(true);
            CancelInvoke("HideNotification");
            Invoke("HideNotification", duration);
        }

        // ---- Private ----

        private void UpdateBalanceDisplay()
        {
            if (balanceText != null) balanceText.text = _currentBalance + " C#";
        }

        private void TogglePanel(GameObject panel)
        {
            if (panel == null) return;
            bool open = !panel.activeSelf;
            // Close all first
            shopPanel?      .SetActive(false);
            inventoryPanel? .SetActive(false);
            progressionPanel?.SetActive(false);
            // Open the requested one
            panel.SetActive(open);
        }

        private void OnLogout()
        {
            APIClient.Instance?.Logout();
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
        }

        private void HideNotification()
        {
            notificationBanner?.SetActive(false);
        }
    }
}