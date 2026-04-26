using UnityEngine;
using UnityEngine.UI;

namespace YAMBO.Shop
{
    /// <summary>
    /// Applique une couleur de bordure selon le type d'item
    /// Attacher à chaque ItemCard
    /// </summary>
    public class ItemCardColorizer : MonoBehaviour
    {
        [Header("Item Info")]
        public string itemType = "skin"; // skin, powerup, emoji

        [Header("Bordure")]
        public Image borderImage; // Le Background de la carte

        // Couleurs par catégorie
        private static readonly Color skinColor = new Color(0.66f, 0.33f, 0.96f); // Violet #a855f7
        private static readonly Color powerupColor = new Color(0.98f, 0.45f, 0.09f); // Orange #f97316
        private static readonly Color emojiColor = new Color(0.93f, 0.28f, 0.60f); // Rose #ec4899

        public void SetItemType(string type)
        {
            itemType = type.ToLower();
            ApplyColor();
        }

        void ApplyColor()
        {
            if (borderImage == null) return;

            Color borderColor;

            switch (itemType)
            {
                case "skin":
                    borderColor = skinColor;
                    break;
                case "powerup":
                    borderColor = powerupColor;
                    break;
                case "emoji":
                    borderColor = emojiColor;
                    break;
                default:
                    borderColor = new Color(0.2f, 0.25f, 0.33f); // Gris par défaut
                    break;
            }

            // Appliquer une bordure colorée
            // On garde le fond gris mais on ajoute une Outline colorée
            Outline outline = borderImage.GetComponent<Outline>();
            if (outline == null)
            {
                outline = borderImage.gameObject.AddComponent<Outline>();
            }

            outline.effectColor = borderColor;
            outline.effectDistance = new Vector2(4, -4);

            Debug.Log($"🎨 Couleur appliquée : {itemType} → {borderColor}");
        }
    }
}