using UnityEngine;
using TMPro;

public class SignUpManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField nameField;
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public TMP_InputField confirmPasswordField;
    public TMP_Dropdown roleDropdown;

    public GameObject adminCodeGroup;
    public TMP_InputField adminCodeInputField;

    [Header("Error Messages")]
    public TextMeshProUGUI passwordErrorText;
    public TextMeshProUGUI adminCodeErrorText;

    [Header("Settings")]
    public string secretAdminPass = "YAMBO2026";
    public LoginAPI loginAPI;

    void Start()
    {
        if (adminCodeGroup != null)
            adminCodeGroup.SetActive(false);

        roleDropdown.onValueChanged.AddListener(delegate { OnRoleChanged(); });

        ClearErrors();
    }

    public void OnRoleChanged()
    {
        ClearErrors();
        if (roleDropdown.options[roleDropdown.value].text == "Admin")
            adminCodeGroup.SetActive(true);
        else
            adminCodeGroup.SetActive(false);
    }

    void ClearErrors()
    {
        passwordErrorText.text = "";
        adminCodeErrorText.text = "";
    }

    public void OnRegisterButtonClicked()
    {
        ClearErrors();
        bool hasError = false;
        string role = roleDropdown.options[roleDropdown.value].text;

        if (passwordField.text != confirmPasswordField.text)
        {
            passwordErrorText.text = "Les mots de passe ne correspondent pas !";
            hasError = true;
        }

        if (role == "Admin" && adminCodeInputField.text != secretAdminPass)
        {
            adminCodeErrorText.text = "Code Admin incorrect !";
            hasError = true;
        }

        if (hasError) return;

        loginAPI.Register(nameField.text, emailField.text, passwordField.text, role);
    }
}