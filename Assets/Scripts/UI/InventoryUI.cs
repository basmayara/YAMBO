using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YAMBO.API;

namespace YAMBO.UI
{
    /// <summary>
    /// Displays the player's inventory fetched from the API.
    /// Attach to a panel containing a ScrollRect > Content (Grid).
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("UI References")]
        public Transform       itemsContainer;   // GridLayoutGroup parent
        public GameObject      itemPrefab;        // Prefab with: ItemName, ItemType, Quantity, PurchasedAt labels
        public TextMeshProUGUI totalItemsText;
        public GameObject      loadingPanel;
        public GameObject      emptyLabel;        // "Your inventory is empty"

        [Header("Filter Buttons (optional)")]
        public Button filterAllButton;
        public Button filterSkinButton;
        public Button filterPowerupButton;

        private List<InventoryItem> _allItems = new List<InventoryItem>();

        void OnEnable()
        {
            Refresh();

            filterAllButton?    .onClick.AddListener(() => DisplayItems(""));
            filterSkinButton?   .onClick.AddListener(() => DisplayItems("skin"));
            filterPowerupButton?.onClick.AddListener(() => DisplayItems("powerup"));
        }

        void OnDisable()
        {
            filterAllButton?    .onClick.RemoveAllListeners();
            filterSkinButton?   .onClick.RemoveAllListeners();
            filterPowerupButton?.onClick.RemoveAllListeners();
        }

        public void Refresh()
        {
            loadingPanel?.SetActive(true);
            emptyLabel?  .SetActive(false);

            StartCoroutine(APIClient.Instance.GetInventory(OnLoaded, OnError));
        }

        private void OnLoaded(InventoryResponse response)
        {
            loadingPanel?.SetActive(false);

            _allItems = response.items ?? new List<InventoryItem>();

            if (totalItemsText != null)
                totalItemsText.text = _allItems.Count + " items";

            DisplayItems("");
        }

        private void OnError(string error)
        {
            loadingPanel?.SetActive(false);
            Debug.LogError("[YAMBO] Inventory error: " + error);
        }

        private void DisplayItems(string typeFilter)
        {
            // Clear old entries
            foreach (Transform child in itemsContainer)
                Destroy(child.gameObject);

            var toShow = string.IsNullOrEmpty(typeFilter)
                ? _allItems
                : _allItems.FindAll(i => i.item_type == typeFilter);

            emptyLabel?.SetActive(toShow.Count == 0);

            foreach (var item in toShow)
            {
                if (itemPrefab == null) continue;

                GameObject go = Instantiate(itemPrefab, itemsContainer);

                SetText(go, "ItemName",   item.item_name);
                SetText(go, "ItemType",   item.item_type);
                SetText(go, "Quantity",   "x" + item.quantity);
                SetText(go, "PurchasedAt", item.PurchasedAt.ToString("dd/MM/yyyy"));
            }
        }

        private static void SetText(GameObject go, string childName, string value)
        {
            var t = go.transform.Find(childName);
            if (t == null) return;
            var tmp = t.GetComponent<TextMeshProUGUI>();
            if (tmp != null) tmp.text = value;
        }
    }
}