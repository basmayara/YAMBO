using UnityEngine;
using TMPro;

public class SignUpManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField nameField;
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public TMP_InputField confirmPasswordField;

    [Header("Error Messages")]
    public TextMeshProUGUI passwordErrorText;  
    public TextMeshProUGUI serverErrorText;    

    public LoginAPI loginAPI;

    void Start()
    {
        ClearErrors();
    }

    void ClearErrors()
    {
        passwordErrorText.text = "";
        if (serverErrorText != null)
            serverErrorText.text = "";
    }

    public void OnRegisterButtonClicked()
    {
        ClearErrors();

        if (string.IsNullOrWhiteSpace(nameField.text) ||
            string.IsNullOrWhiteSpace(emailField.text) ||
            string.IsNullOrWhiteSpace(passwordField.text) ||
            string.IsNullOrWhiteSpace(confirmPasswordField.text))
        {
            serverErrorText.text = "Veuillez remplir tous les champs !";
            return;
        }

        // Validation email
        if (!emailField.text.Contains("@") || !emailField.text.Contains("."))
        {
            serverErrorText.text = "Email invalide !";
            return;
        }

        // Validation password match
        if (passwordField.text != confirmPasswordField.text)
        {
            passwordErrorText.text = "Les mots de passe ne correspondent pas !";
            return;
        }

        // Validation longueur password
        if (passwordField.text.Length < 6)
        {
            passwordErrorText.text = "Le mot de passe doit contenir au moins 6 caractères !";
            return;
        }

        loginAPI.Register(
            nameField.text,
            emailField.text,
            passwordField.text,
            OnSuccess,
            OnError
        );
    }

    void OnSuccess(string message)
    {
        serverErrorText.text = "";
        Debug.Log("Inscription réussie !");

    }

    void OnError(string erreur)
    {
        if (erreur.Contains("déjà utilisé"))
            serverErrorText.text = "Cet email est déjà utilisé !";
        else if (erreur.Contains("6 caractères"))
            passwordErrorText.text = "Le mot de passe doit contenir au moins 6 caractères !";
        else
            serverErrorText.text = "Erreur : " + erreur;
    }
}