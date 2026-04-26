using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class SplashScreenController : MonoBehaviour
{
    [Header("UI References")]
    public Image logo;
    public TextMeshProUGUI loadingText;
    public Slider progressBar;

    [Header("Settings")]
    public float fadeDuration = 1f;
    public float displayDuration = 2f;

    void Start()
    {
        StartCoroutine(SplashSequence());
    }

    IEnumerator SplashSequence()
    {
        // Fondu du logo
        yield return FadeIn(logo, fadeDuration);

        // Simuler le chargement
        float timer = 0f;
        while (timer < displayDuration)
        {
            timer += Time.deltaTime;
            progressBar.value = timer / displayDuration;
            yield return null;
        }

        // Vérifier si l'utilisateur est déjà connecté
        if (YAMBO.API.APIClient.Instance != null &&
            YAMBO.API.APIClient.Instance.IsLoggedIn())
        {
            loadingText.text = "Connexion...";
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene("3_MainMenu");
        }
        else
        {
            loadingText.text = "Redirection...";
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene("2_Login");
        }
    }

    IEnumerator FadeIn(Image image, float duration)
    {
        float timer = 0f;
        Color color = image.color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, timer / duration);
            image.color = color;
            yield return null;
        }

        color.a = 1f;
        image.color = color;
    }
}