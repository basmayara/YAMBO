using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace YAMBO.Shop
{
    /// <summary>
    /// Ajoute des animations aux ItemCards
    /// - Fade-in à l'apparition
    /// - Scale au hover
    /// - Pulse du bouton acheter
    /// VERSION CORRIGÉE - Trouve automatiquement toutes les références
    /// </summary>
    public class ItemCardAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private CanvasGroup canvasGroup;
        private Button buyButton;
        private Vector3 originalScale;
        private Coroutine hoverCoroutine;
        private Coroutine pulseCoroutine;

        // Paramètres d'animation (modifiables dans l'Inspector)
        [Header("Animation Settings")]
        public float fadeInDuration = 0.4f;
        public float hoverScale = 1.05f;
        public float hoverDuration = 0.2f;

        void Awake()
        {
            // Ajouter un CanvasGroup
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            // Trouver le bouton automatiquement
            Transform btnTransform = transform.Find("BuyButton");
            if (btnTransform != null)
            {
                buyButton = btnTransform.GetComponent<Button>();
            }

            originalScale = transform.localScale;
        }

        void Start()
        {
            // Calculer le délai basé sur la position dans la grille
            float delay = transform.GetSiblingIndex() * 0.05f;

            // Lancer les animations après le délai
            StartCoroutine(StartAnimationsWithDelay(delay));
        }

        IEnumerator StartAnimationsWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Fade-in
            StartCoroutine(FadeIn());

            // Pulse du bouton (seulement si interactable)
            if (buyButton != null && buyButton.interactable)
            {
                pulseCoroutine = StartCoroutine(PulseBuyButton());
            }
        }

        void OnDisable()
        {
            if (pulseCoroutine != null)
            {
                StopCoroutine(pulseCoroutine);
            }
        }

        IEnumerator FadeIn()
        {
            canvasGroup.alpha = 0f;
            transform.localScale = originalScale * 0.8f;

            float elapsed = 0f;

            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / fadeInDuration;

                // Ease-out cubic
                float easeProgress = 1f - Mathf.Pow(1f - progress, 3f);

                canvasGroup.alpha = easeProgress;
                transform.localScale = Vector3.Lerp(originalScale * 0.8f, originalScale, easeProgress);

                yield return null;
            }

            canvasGroup.alpha = 1f;
            transform.localScale = originalScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hoverCoroutine != null)
                StopCoroutine(hoverCoroutine);

            hoverCoroutine = StartCoroutine(ScaleTo(originalScale * hoverScale));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (hoverCoroutine != null)
                StopCoroutine(hoverCoroutine);

            hoverCoroutine = StartCoroutine(ScaleTo(originalScale));
        }

        IEnumerator ScaleTo(Vector3 targetScale)
        {
            Vector3 startScale = transform.localScale;
            float elapsed = 0f;

            while (elapsed < hoverDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / hoverDuration;

                transform.localScale = Vector3.Lerp(startScale, targetScale, progress);

                yield return null;
            }

            transform.localScale = targetScale;
        }

        IEnumerator PulseBuyButton()
        {
            if (buyButton == null) yield break;

            Vector3 originalButtonScale = buyButton.transform.localScale;
            float pulseScale = 1.08f;
            float pulseDuration = 0.6f;

            // Attendre un peu avant de commencer
            yield return new WaitForSeconds(1f);

            while (true)
            {
                // Pulse up et down
                float elapsed = 0f;
                while (elapsed < pulseDuration)
                {
                    elapsed += Time.deltaTime;
                    float progress = elapsed / pulseDuration;

                    // Sine wave pour un pulse smooth
                    float scale = 1f + (pulseScale - 1f) * Mathf.Sin(progress * Mathf.PI);
                    buyButton.transform.localScale = originalButtonScale * scale;

                    yield return null;
                }

                buyButton.transform.localScale = originalButtonScale;

                // Pause entre les pulses
                yield return new WaitForSeconds(2f);
            }
        }
    }
}