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
        serverErrorText.text = "";
    }

    public void OnRegisterButtonClicked()
    {
        Debug.Log("Button clicked!"); // ← زيد هاد السطر

        ClearErrors();

        if (string.IsNullOrWhiteSpace(nameField.text) ||
            string.IsNullOrWhiteSpace(emailField.text) ||
            string.IsNullOrWhiteSpace(passwordField.text) ||
            string.IsNullOrWhiteSpace(confirmPasswordField.text))
        {
            serverErrorText.text = "Veuillez remplir tous les champs !";
            return;
        }

        if (!emailField.text.Contains("@"))
        {
            serverErrorText.text = "Email invalide !";
            return;
        }

        if (passwordField.text != confirmPasswordField.text)
        {
            passwordErrorText.text = "Passwords do not match!";
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

    void OnSuccess(string msg)
    {
        serverErrorText.text = "Account created successfully!";
        Debug.Log(msg);
    }

    void OnError(string error)
    {
        serverErrorText.text = error;
    }
}