using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Si tu utilises TextMeshPro
using YAMBO.API;

namespace YAMBO.Auth
{
    public class LoginManager : MonoBehaviour
    {
        [Header("UI References")]
        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;
        public Button loginButton;
        public Button registerButton;
        public TextMeshProUGUI errorText;
        public GameObject loadingPanel;

        void Start()
        {
            // Cacher le texte d'erreur au départ
            errorText.gameObject.SetActive(false);
            loadingPanel.SetActive(false);

            // Attacher les événements aux boutons
            loginButton.onClick.AddListener(OnLoginButtonClicked);
            registerButton.onClick.AddListener(OnRegisterButtonClicked);
        }

        void OnLoginButtonClicked()
        {
            string username = usernameInput.text.Trim();
            string password = passwordInput.text;

            // Validation
            if (string.IsNullOrEmpty(username))
            {
                ShowError("Veuillez entrer un nom d'utilisateur");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Veuillez entrer un mot de passe");
                return;
            }

            // Désactiver le bouton pendant la requête
            loginButton.interactable = false;
            loadingPanel.SetActive(true);
            errorText.gameObject.SetActive(false);

            // Appeler l'API
            StartCoroutine(APIClient.Instance.Login(
                username,
                password,
                OnLoginSuccess,
                OnLoginError
            ));
        }

        void OnLoginSuccess(AuthResponse response)
        {
            Debug.Log($"🎉 Bienvenue {response.username} !");

            // Cacher le loading
            loadingPanel.SetActive(false);

            // Charger la scène principale du jeu
            SceneManager.LoadScene("GameScene"); // Change avec le nom de ta scène
        }

        void OnLoginError(string errorMessage)
        {
            // Réactiver le bouton
            loginButton.interactable = true;
            loadingPanel.SetActive(false);

            // Afficher l'erreur
            ShowError(errorMessage);
        }

        void OnRegisterButtonClicked()
        {
            // Charger la scène d'inscription
            SceneManager.LoadScene("RegisterScene");
        }

        void ShowError(string message)
        {
            errorText.text = message;
            errorText.gameObject.SetActive(true);
        }
    }
}