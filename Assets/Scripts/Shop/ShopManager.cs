using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace YAMBO.Shop
{
    public class ShopManager : MonoBehaviour
    {
        [Header("UI References")]
        public Transform itemsContainer;
        public GameObject itemPrefab;
        public TextMeshProUGUI balanceText;
        public GameObject loadingPanel;
        public GameObject purchaseSuccessPopup;
        public TextMeshProUGUI popupMessageText;

        [Header("Category Tabs")]
        public Button skinsTabButton;
        public Button powerupsTabButton;
        public Button emojisTabButton;

        // Configuration API
        private const string API_BASE_URL = "http://localhost:5000/api/Shop";
        private const int PLAYER_ID = 1; // ID du joueur de test

        // Couleurs des tabs
        private Color activeTabColor = new Color(0.23f, 0.51f, 0.96f); // #3b82f6 bleu
        private Color inactiveTabColor = new Color(0.28f, 0.33f, 0.41f); // #475569 gris

        private string currentCategory = "skin";
        private List<ShopItem> allItems = new List<ShopItem>();
        private int currentBalance = 0;

        void Start()
        {
            // Désactiver les popups au démarrage
            if (loadingPanel != null)
                loadingPanel.SetActive(false);

            if (purchaseSuccessPopup != null)
                purchaseSuccessPopup.SetActive(false);

            // Configuration des onglets
            SetupCategoryTabs();

            // Charger les données depuis le backend
            StartCoroutine(LoadBalance());
            StartCoroutine(LoadShopItems());
        }

        void SetupCategoryTabs()
        {
            if (skinsTabButton != null)
            {
                skinsTabButton.onClick.AddListener(() => OnCategoryTabClicked("skin", skinsTabButton));
            }

            if (powerupsTabButton != null)
            {
                powerupsTabButton.onClick.AddListener(() => OnCategoryTabClicked("powerup", powerupsTabButton));
            }

            if (emojisTabButton != null)
            {
                emojisTabButton.onClick.AddListener(() => OnCategoryTabClicked("emoji", emojisTabButton));
            }

            // Définir SKINS comme actif au démarrage
            UpdateTabColors(skinsTabButton);
        }

        void OnCategoryTabClicked(string category, Button clickedButton)
        {
            currentCategory = category;
            UpdateTabColors(clickedButton);
            FilterByCategory(category);

            Debug.Log($"📂 Catégorie sélectionnée : {category}");
        }

        void UpdateTabColors(Button activeButton)
        {
            // Remettre tous les onglets en gris
            if (skinsTabButton != null)
            {
                skinsTabButton.GetComponent<Image>().color = inactiveTabColor;
            }

            if (powerupsTabButton != null)
            {
                powerupsTabButton.GetComponent<Image>().color = inactiveTabColor;
            }

            if (emojisTabButton != null)
            {
                emojisTabButton.GetComponent<Image>().color = inactiveTabColor;
            }

            // Mettre le bouton actif en bleu
            if (activeButton != null)
            {
                activeButton.GetComponent<Image>().color = activeTabColor;
            }
        }

        void FilterByCategory(string category)
        {
            Debug.Log($"🔍 Filtrage par catégorie : {category}");

            // Effacer les items actuels
            foreach (Transform child in itemsContainer)
            {
                Destroy(child.gameObject);
            }

            // Afficher seulement les items de la catégorie sélectionnée
            foreach (var item in allItems)
            {
                if (item.itemType == category)
                {
                    DisplayItem(item);
                }
            }
        }

        IEnumerator LoadBalance()
        {
            string url = $"{API_BASE_URL}/balance/{PLAYER_ID}";

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string json = request.downloadHandler.text;
                    Debug.Log($"✅ Balance reçu: {json}");

                    BalanceResponse response = JsonUtility.FromJson<BalanceResponse>(json);
                    currentBalance = response.balance;

                    if (balanceText != null)
                        balanceText.text = $"{currentBalance} C#";
                }
                else
                {
                    Debug.LogError($"❌ Erreur chargement balance: {request.error}");
                }
            }
        }

        IEnumerator LoadShopItems()
        {
            if (loadingPanel != null)
                loadingPanel.SetActive(true);

            string url = $"{API_BASE_URL}/items?playerId={PLAYER_ID}";

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (loadingPanel != null)
                    loadingPanel.SetActive(false);

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string json = request.downloadHandler.text;
                    Debug.Log($"✅ Items reçus du backend C#");

                    // Wrapper pour le JSON array
                    string wrappedJson = "{\"items\":" + json + "}";
                    ShopItemsWrapper wrapper = JsonUtility.FromJson<ShopItemsWrapper>(wrappedJson);
                    allItems = new List<ShopItem>(wrapper.items);

                    Debug.Log($"✅ {allItems.Count} items chargés depuis le backend");

                    FilterByCategory(currentCategory);
                }
                else
                {
                    Debug.LogError($"❌ Erreur chargement items: {request.error}");
                }
            }
        }

        void DisplayItem(ShopItem item)
        {
            GameObject itemCard = Instantiate(itemPrefab, itemsContainer);

            // ========== COULEURS & ANIMATIONS ==========
            ItemCardAnimator animator = itemCard.AddComponent<ItemCardAnimator>();
            ItemCardColorizer colorizer = itemCard.AddComponent<ItemCardColorizer>();

            // Trouver le Background pour la bordure colorée
            Transform backgroundTransform = itemCard.transform.Find("Background");
            if (backgroundTransform != null)
            {
                Image bgImage = backgroundTransform.GetComponent<Image>();
                if (bgImage != null)
                {
                    colorizer.borderImage = bgImage;
                }
            }

            // Définir le type d'item pour la couleur
            colorizer.SetItemType(item.itemType);

            // ========== DONNÉES ==========
            Transform itemNameTransform = itemCard.transform.Find("ItemName");
            if (itemNameTransform != null)
            {
                TextMeshProUGUI nameText = itemNameTransform.GetComponent<TextMeshProUGUI>();
                if (nameText != null)
                    nameText.text = item.itemName;
            }

            Transform descriptionTransform = itemCard.transform.Find("Description");
            if (descriptionTransform != null)
            {
                TextMeshProUGUI descText = descriptionTransform.GetComponent<TextMeshProUGUI>();
                if (descText != null)
                    descText.text = item.description;
            }

            Transform pricePanelTransform = itemCard.transform.Find("PricePanel");
            if (pricePanelTransform != null)
            {
                Transform priceTransform = pricePanelTransform.Find("Price");
                if (priceTransform != null)
                {
                    TextMeshProUGUI priceText = priceTransform.GetComponent<TextMeshProUGUI>();
                    if (priceText != null)
                        priceText.text = $"{item.price} C#";
                }
            }

            // Configurer le bouton d'achat
            Transform buyButtonTransform = itemCard.transform.Find("BuyButton");
            if (buyButtonTransform != null)
            {
                Button buyButton = buyButtonTransform.GetComponent<Button>();
                if (buyButton != null)
                {
                    int itemId = item.itemId;
                    buyButton.onClick.AddListener(() => OnPurchaseClicked(itemId));

                    // Désactiver si pas assez d'argent ou déjà possédé
                    if (!item.isAvailable || currentBalance < item.price)
                    {
                        buyButton.interactable = false;

                        TextMeshProUGUI buttonText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
                        if (buttonText != null)
                        {
                            buttonText.text = item.isAvailable ? "INSUFFISANT" : "POSSÉDÉ";
                        }
                    }
                }
            }
        }

        void OnPurchaseClicked(int itemId)
        {
            Debug.Log($"🛒 Tentative d'achat de l'item {itemId}");
            StartCoroutine(PurchaseItem(itemId));
        }

        IEnumerator PurchaseItem(int itemId)
        {
            if (loadingPanel != null)
                loadingPanel.SetActive(true);

            // Créer le JSON body
            PurchaseRequest purchaseRequest = new PurchaseRequest
            {
                playerId = PLAYER_ID,
                itemId = itemId
            };
            string jsonBody = JsonUtility.ToJson(purchaseRequest);

            using (UnityWebRequest request = new UnityWebRequest($"{API_BASE_URL}/purchase", "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (loadingPanel != null)
                    loadingPanel.SetActive(false);

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string json = request.downloadHandler.text;
                    Debug.Log($"✅ Achat réussi: {json}");

                    PurchaseResponse response = JsonUtility.FromJson<PurchaseResponse>(json);

                    // Mettre à jour le solde
                    currentBalance = response.newBalance;
                    if (balanceText != null)
                        balanceText.text = $"{currentBalance} C#";

                    // Afficher le popup de succès
                    if (purchaseSuccessPopup != null)
                    {
                        purchaseSuccessPopup.SetActive(true);
                        if (popupMessageText != null)
                            popupMessageText.text = $"✅ {response.message}\n\nNouveau solde: {response.newBalance} C#";

                        StartCoroutine(HidePopupAfterDelay(3f));
                    }

                    // Recharger les items pour mettre à jour "POSSÉDÉ"
                    StartCoroutine(LoadShopItems());
                }
                else
                {
                    Debug.LogError($"❌ Erreur achat: {request.error}");

                    if (purchaseSuccessPopup != null)
                    {
                        purchaseSuccessPopup.SetActive(true);
                        if (popupMessageText != null)
                            popupMessageText.text = $"❌ Erreur:\n{request.error}";

                        StartCoroutine(HidePopupAfterDelay(3f));
                    }
                }
            }
        }

        IEnumerator HidePopupAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (purchaseSuccessPopup != null)
                purchaseSuccessPopup.SetActive(false);
        }
    }

    // ==========================================
    // CLASSES POUR JSON
    // ==========================================

    [System.Serializable]
    public class ShopItem
    {
        public int itemId;
        public string itemName;
        public string itemType;
        public int price;
        public string description;
        public string spriteUrl;
        public string rarity;
        public bool isAvailable;
    }

    [System.Serializable]
    public class ShopItemsWrapper
    {
        public ShopItem[] items;
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
    public class PurchaseRequest
    {
        public int playerId;
        public int itemId;
    }

    [System.Serializable]
    public class PurchaseResponse
    {
        public bool success;
        public string message;
        public int newBalance;
        public string itemName;
    }
}