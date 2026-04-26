using UnityEngine;
using TMPro;
using YAMBO.API;

namespace YAMBO.UI
{
    /// <summary>
    /// Simple component: auto-refreshes balance display every RefreshIntervalSeconds.
    /// Attach to any TextMeshProUGUI on HUD, Shop, Quiz scenes.
    /// </summary>
    public class BalanceDisplay : MonoBehaviour
    {
        [Header("Config")]
        public TextMeshProUGUI balanceLabel;
        public float           refreshIntervalSeconds = 30f;
        public string          format = "{0} C#";

        private float _timer;

        void Start()
        {
            Refresh();
        }

        void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= refreshIntervalSeconds)
            {
                _timer = 0f;
                Refresh();
            }
        }

        public void Refresh()
        {
            if (APIClient.Instance == null || !APIClient.Instance.IsLoggedIn()) return;
            StartCoroutine(APIClient.Instance.GetBalance(
                r => UpdateDisplay(r.balance),
                e => Debug.LogWarning("[BalanceDisplay] Error: " + e)));
        }

        public void UpdateDisplay(int balance)
        {
            if (balanceLabel != null)
                balanceLabel.text = string.Format(format, balance);
        }
    }
}