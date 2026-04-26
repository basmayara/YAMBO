using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Script pour créer le Prefab ItemCard automatiquement
/// UTILISATION : Attacher ce script à un GameObject vide, puis cliquer sur "Create ItemCard Prefab" dans l'Inspector
/// </summary>
public class CreateItemCardPrefab : MonoBehaviour
{
#if UNITY_EDITOR
    [ContextMenu("Create ItemCard Prefab")]
    public void CreatePrefab()
    {
        // Créer le GameObject parent
        GameObject itemCard = new GameObject("ItemCard");
        RectTransform cardRect = itemCard.AddComponent<RectTransform>();
        cardRect.sizeDelta = new Vector2(280, 380);

        // ========== BACKGROUND ==========
        GameObject background = new GameObject("Background");
        background.transform.SetParent(itemCard.transform, false);
        RectTransform bgRect = background.AddComponent<RectTransform>();
        Image bgImage = background.AddComponent<Image>();

        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        bgImage.color = new Color(0.118f, 0.161f, 0.231f); // #1e293b

        // Ajouter une ombre
        Shadow shadow = background.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.39f);
        shadow.effectDistance = new Vector2(5, -5);

        // ========== ITEMIMAGE ==========
        GameObject itemImage = new GameObject("ItemImage");
        itemImage.transform.SetParent(itemCard.transform, false);
        RectTransform imgRect = itemImage.AddComponent<RectTransform>();
        Image img = itemImage.AddComponent<Image>();

        imgRect.anchorMin = new Vector2(0.5f, 1);
        imgRect.anchorMax = new Vector2(0.5f, 1);
        imgRect.pivot = new Vector2(0.5f, 1);
        imgRect.anchoredPosition = new Vector2(0, -30);
        imgRect.sizeDelta = new Vector2(220, 220);
        img.color = Color.white;
        img.preserveAspect = true;

        // ========== ITEMNAME ==========
        GameObject itemName = new GameObject("ItemName");
        itemName.transform.SetParent(itemCard.transform, false);
        RectTransform nameRect = itemName.AddComponent<RectTransform>();
        TextMeshProUGUI nameText = itemName.AddComponent<TextMeshProUGUI>();

        nameRect.anchorMin = new Vector2(0.5f, 1);
        nameRect.anchorMax = new Vector2(0.5f, 1);
        nameRect.pivot = new Vector2(0.5f, 1);
        nameRect.anchoredPosition = new Vector2(0, -265);
        nameRect.sizeDelta = new Vector2(260, 40);

        nameText.text = "Skin Test";
        nameText.fontSize = 20;
        nameText.fontStyle = FontStyles.Bold;
        nameText.color = Color.white;
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.overflowMode = TextOverflowModes.Ellipsis;

        // ========== DESCRIPTION ==========
        GameObject description = new GameObject("Description");
        description.transform.SetParent(itemCard.transform, false);
        RectTransform descRect = description.AddComponent<RectTransform>();
        TextMeshProUGUI descText = description.AddComponent<TextMeshProUGUI>();

        descRect.anchorMin = new Vector2(0.5f, 1);
        descRect.anchorMax = new Vector2(0.5f, 1);
        descRect.pivot = new Vector2(0.5f, 1);
        descRect.anchoredPosition = new Vector2(0, -305);
        descRect.sizeDelta = new Vector2(260, 40);

        descText.text = "Description du skin";
        descText.fontSize = 14;
        descText.color = new Color(0.58f, 0.64f, 0.72f); // #94a3b8
        descText.alignment = TextAlignmentOptions.Center;
        descText.overflowMode = TextOverflowModes.Ellipsis;

        // ========== PRICEPANEL ==========
        GameObject pricePanel = new GameObject("PricePanel");
        pricePanel.transform.SetParent(itemCard.transform, false);
        RectTransform pricePanelRect = pricePanel.AddComponent<RectTransform>();
        Image pricePanelImage = pricePanel.AddComponent<Image>();

        pricePanelRect.anchorMin = new Vector2(0.5f, 0);
        pricePanelRect.anchorMax = new Vector2(0.5f, 0);
        pricePanelRect.pivot = new Vector2(0.5f, 0);
        pricePanelRect.anchoredPosition = new Vector2(0, 70);
        pricePanelRect.sizeDelta = new Vector2(140, 40);
        pricePanelImage.color = new Color(0.2f, 0.255f, 0.333f); // #334155

        // CoinIcon
        GameObject coinIcon = new GameObject("CoinIcon");
        coinIcon.transform.SetParent(pricePanel.transform, false);
        RectTransform coinRect = coinIcon.AddComponent<RectTransform>();
        Image coinImage = coinIcon.AddComponent<Image>();

        coinRect.anchorMin = new Vector2(0, 0.5f);
        coinRect.anchorMax = new Vector2(0, 0.5f);
        coinRect.pivot = new Vector2(0, 0.5f);
        coinRect.anchoredPosition = new Vector2(20, 0);
        coinRect.sizeDelta = new Vector2(24, 24);

        // Utiliser un sprite par défaut (Knob)
        coinImage.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Knob.psd");
        coinImage.color = new Color(0.984f, 0.749f, 0.141f); // #fbbf24

        // Price
        GameObject price = new GameObject("Price");
        price.transform.SetParent(pricePanel.transform, false);
        RectTransform priceRect = price.AddComponent<RectTransform>();
        TextMeshProUGUI priceText = price.AddComponent<TextMeshProUGUI>();

        priceRect.anchorMin = new Vector2(0.5f, 0.5f);
        priceRect.anchorMax = new Vector2(0.5f, 0.5f);
        priceRect.pivot = new Vector2(0.5f, 0.5f);
        priceRect.anchoredPosition = new Vector2(15, 0);
        priceRect.sizeDelta = new Vector2(80, 35);

        priceText.text = "100 C#";
        priceText.fontSize = 18;
        priceText.fontStyle = FontStyles.Bold;
        priceText.color = new Color(0.984f, 0.749f, 0.141f); // #fbbf24
        priceText.alignment = TextAlignmentOptions.Center;

        // ========== BUYBUTTON ==========
        GameObject buyButton = new GameObject("BuyButton");
        buyButton.transform.SetParent(itemCard.transform, false);
        RectTransform btnRect = buyButton.AddComponent<RectTransform>();
        Image btnImage = buyButton.AddComponent<Image>();
        Button btn = buyButton.AddComponent<Button>();

        btnRect.anchorMin = new Vector2(0.5f, 0);
        btnRect.anchorMax = new Vector2(0.5f, 0);
        btnRect.pivot = new Vector2(0.5f, 0);
        btnRect.anchoredPosition = new Vector2(0, 20);
        btnRect.sizeDelta = new Vector2(240, 50);
        btnImage.color = new Color(0.133f, 0.773f, 0.369f); // #22c55e

        // Couleurs du bouton
        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0.133f, 0.773f, 0.369f);
        colors.highlightedColor = new Color(0.2f, 0.85f, 0.45f);
        colors.pressedColor = new Color(0.1f, 0.65f, 0.3f);
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f);
        btn.colors = colors;

        // Ombre du bouton
        Shadow btnShadow = buyButton.AddComponent<Shadow>();
        btnShadow.effectColor = new Color(0, 0, 0, 0.39f);
        btnShadow.effectDistance = new Vector2(3, -3);

        // Texte du bouton
        GameObject btnText = new GameObject("Text (TMP)");
        btnText.transform.SetParent(buyButton.transform, false);
        RectTransform btnTextRect = btnText.AddComponent<RectTransform>();
        TextMeshProUGUI btnTextTMP = btnText.AddComponent<TextMeshProUGUI>();

        btnTextRect.anchorMin = Vector2.zero;
        btnTextRect.anchorMax = Vector2.one;
        btnTextRect.offsetMin = Vector2.zero;
        btnTextRect.offsetMax = Vector2.zero;

        btnTextTMP.text = "ACHETER";
        btnTextTMP.fontSize = 20;
        btnTextTMP.fontStyle = FontStyles.Bold;
        btnTextTMP.color = Color.white;
        btnTextTMP.alignment = TextAlignmentOptions.Center;

        // ========== SAUVEGARDER LE PREFAB ==========
        string prefabPath = "Assets/Prefabs/Shop/ItemCard.prefab";

        // Créer les dossiers si nécessaire
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Shop"))
            AssetDatabase.CreateFolder("Assets/Prefabs", "Shop");

        // Supprimer l'ancien prefab s'il existe
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            AssetDatabase.DeleteAsset(prefabPath);
        }

        // Créer le nouveau prefab
        PrefabUtility.SaveAsPrefabAsset(itemCard, prefabPath);

        // Nettoyer
        DestroyImmediate(itemCard);

        Debug.Log($"✅ Prefab ItemCard créé avec succès à : {prefabPath}");

        // Sélectionner le prefab dans le Project
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Selection.activeObject = prefab;
        EditorGUIUtility.PingObject(prefab);
    }
#endif
}