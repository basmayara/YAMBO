using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace YAMBO.Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        [Header("UI References")]
        public Transform itemsContainer;
        public GameObject itemPrefab;
        public TextMeshProUGUI balanceText;
        public TextMeshProUGUI statsText;
        public GameObject loadingPanel;

        [Header("Category Tabs")]
        public Button allTabButton;
        public Button skinsTabButton;
        public Button powerupsTabButton;
        public Button emojisTabButton;

        [Header("Sort Buttons")]
        public Button sortFavoritesButton;
        public Button sortNameButton;
        public Button sortCategoryButton;
        public Button sortPriceButton;

        [Header("Sell Confirmation Popup")]
        public GameObject sellPopup;
        public TextMeshProUGUI sellPopupItemName;
        public TextMeshProUGUI sellPopupRefundText;
        public Button sellConfirmButton;
        public Button sellCancelButton;

        // Configuration API
        private const string API_BASE_URL = "http://localhost:5000/api/Shop";
        private const int PLAYER_ID = 1;

        // Couleurs
        private Color activeTabColor = new Color(0.23f, 0.51f, 0.96f); // #3b82f6
        private Color inactiveTabColor = new Color(0.28f, 0.33f, 0.41f); // #475569
        private Color activeSortColor = new Color(0.66f, 0.33f, 0.97f); // #a855f7
        private Color inactiveSortColor = new Color(0.28f, 0.33f, 0.41f);
        private Color sellButtonColor = new Color(0.86f, 0.20f, 0.27f); // #dc2626 rouge
        private Color favoriteOnColor = new Color(0.98f, 0.75f, 0.15f); // #fbbf24 jaune
        private Color favoriteOffColor = new Color(0.28f, 0.33f, 0.41f);

        private string currentCategory = "all";
        private string currentSort = "none";
        private List<InventoryItem> allItems = new List<InventoryItem>();
        private int currentBalance = 0;

        // Popup state
        private int pendingSellItemId;
        private string pendingSellItemName;
        private int pendingSellRefund;

        void Start()
        {
            if (loadingPanel != null) loadingPanel.SetActive(false);
            if (sellPopup != null) sellPopup.SetActive(false);

            SetupCategoryTabs();
            SetupSortButtons();
            SetupSellPopup();

            StartCoroutine(LoadBalance());
            StartCoroutine(LoadInventory());
        }

        // ══════════════════════════════════════════════
        // SETUP
        // ══════════════════════════════════════════════

        void SetupCategoryTabs()
        {
            allTabButton?.onClick.AddListener(() => OnCategoryTabClicked("all", allTabButton));
            skinsTabButton?.onClick.AddListener(() => OnCategoryTabClicked("skin", skinsTabButton));
            powerupsTabButton?.onClick.AddListener(() => OnCategoryTabClicked("powerup", powerupsTabButton));
            emojisTabButton?.onClick.AddListener(() => OnCategoryTabClicked("emoji", emojisTabButton));
            UpdateTabColors(allTabButton);
        }

        void SetupSortButtons()
        {
            sortFavoritesButton?.onClick.AddListener(() => OnSortClicked("favorites", sortFavoritesButton));
            sortNameButton?.onClick.AddListener(() => OnSortClicked("name", sortNameButton));
            sortCategoryButton?.onClick.AddListener(() => OnSortClicked("category", sortCategoryButton));
            sortPriceButton?.onClick.AddListener(() => OnSortClicked("price", sortPriceButton));
            UpdateSortColors(null);
        }

        void SetupSellPopup()
        {
            sellConfirmButton?.onClick.AddListener(OnSellConfirmed);
            sellCancelButton?.onClick.AddListener(() => sellPopup?.SetActive(false));
        }

        // ══════════════════════════════════════════════
        // TABS & SORT
        // ══════════════════════════════════════════════

        void OnCategoryTabClicked(string category, Button clicked)
        {
            currentCategory = category;
            UpdateTabColors(clicked);
            RefreshDisplay();
        }

        void OnSortClicked(string sort, Button clicked)
        {
            // Toggle off si déjà actif
            if (currentSort == sort) currentSort = "none";
            else currentSort = sort;

            UpdateSortColors(currentSort == "none" ? null : clicked);
            RefreshDisplay();
        }

        void UpdateTabColors(Button active)
        {
            foreach (var btn in new[] { allTabButton, skinsTabButton, powerupsTabButton, emojisTabButton })
                if (btn != null) btn.GetComponent<Image>().color = inactiveTabColor;
            if (active != null) active.GetComponent<Image>().color = activeTabColor;
        }

        void UpdateSortColors(Button active)
        {
            foreach (var btn in new[] { sortFavoritesButton, sortNameButton, sortCategoryButton, sortPriceButton })
                if (btn != null) btn.GetComponent<Image>().color = inactiveSortColor;
            if (active != null) active.GetComponent<Image>().color = activeSortColor;
        }

        // ══════════════════════════════════════════════
        // AFFICHAGE
        // ══════════════════════════════════════════════

        void RefreshDisplay()
        {
            // 1. Filtrer par catégorie
            IEnumerable<InventoryItem> filtered = allItems;
            if (currentCategory != "all")
                filtered = filtered.Where(i => i.itemType == currentCategory);

            // 2. Trier
            filtered = currentSort switch
            {
                "favorites" => filtered.OrderByDescending(i => i.isFavorite).ThenBy(i => i.itemName),
                "name" => filtered.OrderBy(i => i.itemName),
                "category" => filtered.OrderBy(i => i.itemType).ThenBy(i => i.itemName),
                "price" => filtered.OrderByDescending(i => i.price),
                _ => filtered
            };

            var list = filtered.ToList();

            // 3. Effacer et afficher
            foreach (Transform child in itemsContainer)
                Destroy(child.gameObject);

            foreach (var item in list)
                DisplayItem(item);

            UpdateStats(list.Count);
        }

        void DisplayItem(InventoryItem item)
        {
            GameObject itemCard = Instantiate(itemPrefab, itemsContainer);

            // Animations & couleurs
            YAMBO.Shop.ItemCardAnimator animator = itemCard.AddComponent<YAMBO.Shop.ItemCardAnimator>();
            YAMBO.Shop.ItemCardColorizer colorizer = itemCard.AddComponent<YAMBO.Shop.ItemCardColorizer>();

            Transform bg = itemCard.transform.Find("Background");
            if (bg != null)
            {
                Image bgImage = bg.GetComponent<Image>();
                if (bgImage != null) colorizer.borderImage = bgImage;
            }
            colorizer.SetItemType(item.itemType);

            // Nom
            Transform nameT = itemCard.transform.Find("ItemName");
            if (nameT != null)
            {
                var txt = nameT.GetComponent<TextMeshProUGUI>();
                if (txt != null) txt.text = item.itemName;
            }

            // Description
            Transform descT = itemCard.transform.Find("Description");
            if (descT != null)
            {
                var txt = descT.GetComponent<TextMeshProUGUI>();
                if (txt != null) txt.text = item.description;
            }

            // Prix
            Transform pricePanel = itemCard.transform.Find("PricePanel");
            if (pricePanel != null)
            {
                Transform priceT = pricePanel.Find("Price");
                if (priceT != null)
                {
                    var txt = priceT.GetComponent<TextMeshProUGUI>();
                    if (txt != null) txt.text = $"{item.price} C#";
                }
            }

            // ── Bouton ÉQUIPER ──
            Transform buyT = itemCard.transform.Find("BuyButton");
            if (buyT != null)
            {
                Button btn = buyT.GetComponent<Button>();
                if (btn != null)
                {
                    var lbl = btn.GetComponentInChildren<TextMeshProUGUI>();
                    if (lbl != null) lbl.text = "ÉQUIPER";
                    var img = btn.GetComponent<Image>();
                    if (img != null) img.color = activeTabColor;
                    int id = item.itemId; string name = item.itemName;
                    btn.onClick.AddListener(() => OnEquipClicked(id, name));
                }
            }

            // ── Bouton VENDRE ──
            Transform sellT = itemCard.transform.Find("SellButton");
            if (sellT != null)
            {
                Button btn = sellT.GetComponent<Button>();
                if (btn != null)
                {
                    var img = btn.GetComponent<Image>();
                    if (img != null) img.color = sellButtonColor;
                    var lbl = btn.GetComponentInChildren<TextMeshProUGUI>();
                    if (lbl != null) lbl.text = $"VENDRE\n{item.price / 2} C#";
                    int id = item.itemId; string name = item.itemName; int refund = item.price / 2;
                    btn.onClick.AddListener(() => OpenSellPopup(id, name, refund));
                }
            }

            // ── Bouton FAVORI ⭐ ──
            Transform favT = itemCard.transform.Find("FavoriteButton");
            if (favT != null)
            {
                Button btn = favT.GetComponent<Button>();
                if (btn != null)
                {
                    Image img = btn.GetComponent<Image>();
                    UpdateFavoriteButtonVisual(btn, item.isFavorite);
                    int id = item.itemId;
                    btn.onClick.AddListener(() => OnFavoriteClicked(id, btn, item));
                }
            }
        }

        void UpdateFavoriteButtonVisual(Button btn, bool isFav)
        {
            var img = btn.GetComponent<Image>();
            if (img != null) img.color = isFav ? favoriteOnColor : favoriteOffColor;
            var lbl = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (lbl != null) lbl.text = isFav ? " " : "";
        }

        // ══════════════════════════════════════════════
        // ACTIONS
        // ══════════════════════════════════════════════

        void OnEquipClicked(int itemId, string itemName)
        {
            Debug.Log($"⚔️ Équiper : {itemName} (ID: {itemId})");
        }

        void OnFavoriteClicked(int itemId, Button btn, InventoryItem item)
        {
            item.isFavorite = !item.isFavorite;
            UpdateFavoriteButtonVisual(btn, item.isFavorite);
            StartCoroutine(SendFavoriteRequest(itemId, item.isFavorite));
        }

        void OpenSellPopup(int itemId, string itemName, int refund)
        {
            pendingSellItemId = itemId;
            pendingSellItemName = itemName;
            pendingSellRefund = refund;

            if (sellPopupItemName != null) sellPopupItemName.text = $"Vendre « {itemName} » ?";
            if (sellPopupRefundText != null) sellPopupRefundText.text = $"Remboursement : {refund} C#\n(50% du prix d'achat)";

            sellPopup?.SetActive(true);
        }

        void OnSellConfirmed()
        {
            sellPopup?.SetActive(false);
            StartCoroutine(SendSellRequest(pendingSellItemId));
        }

        // ══════════════════════════════════════════════
        // APPELS API
        // ══════════════════════════════════════════════

        IEnumerator LoadBalance()
        {
            using (UnityWebRequest req = UnityWebRequest.Get($"{API_BASE_URL}/balance/{PLAYER_ID}"))
            {
                yield return req.SendWebRequest();
                if (req.result == UnityWebRequest.Result.Success)
                {
                    var res = JsonUtility.FromJson<BalanceResponse>(req.downloadHandler.text);
                    currentBalance = res.balance;
                    if (balanceText != null) balanceText.text = $"{currentBalance} C#";
                    Debug.Log($"✅ Balance : {currentBalance} C#");
                }
                else Debug.LogError($"❌ Balance : {req.error}");
            }
        }

        IEnumerator LoadInventory()
        {
            if (loadingPanel != null) loadingPanel.SetActive(true);

            using (UnityWebRequest req = UnityWebRequest.Get($"{API_BASE_URL}/inventory/{PLAYER_ID}"))
            {
                yield return req.SendWebRequest();
                if (loadingPanel != null) loadingPanel.SetActive(false);

                if (req.result == UnityWebRequest.Result.Success)
                {
                    string wrapped = "{\"items\":" + req.downloadHandler.text + "}";
                    InventoryWrapper wrapper = JsonUtility.FromJson<InventoryWrapper>(wrapped);
                    allItems = new List<InventoryItem>(wrapper.items);
                    Debug.Log($"✅ {allItems.Count} items chargés");
                    RefreshDisplay();
                }
                else Debug.LogError($"❌ Inventaire : {req.error}");
            }
        }

        IEnumerator SendSellRequest(int itemId)
        {
            string json = $"{{\"playerId\":{PLAYER_ID},\"itemId\":{itemId}}}";
            byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

            using (UnityWebRequest req = new UnityWebRequest($"{API_BASE_URL}/sell", "POST"))
            {
                req.uploadHandler = new UploadHandlerRaw(body);
                req.downloadHandler = new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type", "application/json");

                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    SellResponse res = JsonUtility.FromJson<SellResponse>(req.downloadHandler.text);
                    if (res.success)
                    {
                        currentBalance = res.newBalance;
                        if (balanceText != null) balanceText.text = $"{currentBalance} C#";

                        // Retirer l'item de la liste locale
                        allItems.RemoveAll(i => i.itemId == itemId);
                        RefreshDisplay();

                        Debug.Log($"✅ Vendu ! Remboursement : {res.refundAmount} C# | Nouveau solde : {currentBalance}");
                    }
                    else Debug.LogWarning($"⚠️ Vente échouée : {res.message}");
                }
                else Debug.LogError($"❌ Erreur vente : {req.error}");
            }
        }

        IEnumerator SendFavoriteRequest(int itemId, bool isFavorite)
        {
            string json = $"{{\"playerId\":{PLAYER_ID},\"itemId\":{itemId},\"isFavorite\":{isFavorite.ToString().ToLower()}}}";
            byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

            using (UnityWebRequest req = new UnityWebRequest($"{API_BASE_URL}/inventory/favorite", "PUT"))
            {
                req.uploadHandler = new UploadHandlerRaw(body);
                req.downloadHandler = new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type", "application/json");

                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                    Debug.Log($"✅ Favori mis à jour (item {itemId} → {isFavorite})");
                else
                    Debug.LogError($"❌ Erreur favori : {req.error}");
            }
        }

        void UpdateStats(int displayedCount)
        {
            if (statsText == null) return;
            int totalValue = allItems.Sum(i => i.price);
            int favCount = allItems.Count(i => i.isFavorite);
            statsText.text = $"Affichés : {displayedCount} • Total : {allItems.Count} • ★ {favCount} favoris • Valeur : {totalValue} C#";
        }
    }

    
    [System.Serializable]
    public class InventoryItem
    {
        public int itemId;
        public string itemName;
        public string itemType;
        public int price;
        public string description;
        public string spriteUrl;
        public string rarity;
        public bool isAvailable;
        public bool isFavorite;  
    }

    [System.Serializable]
    public class InventoryWrapper
    {
        public InventoryItem[] items;
    }

    [System.Serializable]
    public class BalanceResponse
    {
        public int playerId;
        public string username;
        public int balance;
        public int totalEarned;
        public int totalSpent;
    }

    [System.Serializable]
    public class SellResponse
    {
        public bool success;
        public string message;
        public int refundAmount;
        public int newBalance;
    }
}