using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class ForgotPasswordManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject forgotPasswordPanel;
    public GameObject resetPasswordPanel;
    public GameObject successPanel;

    [Header("Forgot Password UI")]
    public TMP_InputField forgotEmailInput;
    public TMP_Text forgotStatusText;

    [Header("Reset Password UI")]
    public TMP_Text sentToText;
    public TMP_InputField tokenInput;
    public TMP_InputField newPasswordInput;
    public TMP_InputField confirmPasswordInput;
    public TMP_Text resetStatusText;

    [Header("API Config")]
    public string apiUrl = "https://localhost:7220/api/auth";

    private string userEmail;

    void Start()
    {
        loginPanel.SetActive(true);
        forgotPasswordPanel.SetActive(false);
        resetPasswordPanel.SetActive(false);
        successPanel.SetActive(false);
    }

    public void ShowForgotPassword()
    {
        loginPanel.SetActive(false);
        forgotPasswordPanel.SetActive(true);
        resetPasswordPanel.SetActive(false);
        successPanel.SetActive(false);
    }

    public void ShowLogin()
    {
        loginPanel.SetActive(true);
        forgotPasswordPanel.SetActive(false);
        resetPasswordPanel.SetActive(false);
        successPanel.SetActive(false);
    }

    public void ShowResetPassword()
    {
        loginPanel.SetActive(false);
        forgotPasswordPanel.SetActive(false);
        resetPasswordPanel.SetActive(true);
        successPanel.SetActive(false);
    }

    public void OnSendResetLinkClicked()
    {
        string email = forgotEmailInput.text.Trim();
        if (string.IsNullOrEmpty(email) || !email.Contains("@"))
        {
            forgotStatusText.text = "Please enter a valid email!";
            return;
        }
        userEmail = email;
        StartCoroutine(SendResetLink(email));
    }

    private IEnumerator SendResetLink(string email)
    {
        forgotStatusText.text = "Sending...";
        string json = "{\"Email\":\"" + email + "\"}";

        UnityWebRequest request = new UnityWebRequest(apiUrl + "/forgot-password", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.certificateHandler = new BypassCertificate();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("Connection error: " + request.error);
            forgotStatusText.text = "Cannot connect to server.";
        }
        else if (request.responseCode == 404 || request.responseCode == 400)
        {
            forgotStatusText.text = "No account found with that email.";
        }
        else if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;

            // ✅ Vérifie success avant de changer de panel
            if (responseText.Contains("\"success\":true"))
            {
                forgotStatusText.text = "Code sent! Check your email.";
                if (sentToText != null)
                    sentToText.text = "We sent a reset code to " + userEmail;
                yield return new WaitForSeconds(2f);
                ShowResetPassword();
            }
            else
            {
                // ❌ Email not found — reste sur ForgotPanel
                forgotStatusText.text = "No account found with that email.";
            }
        }
        else
        {
            Debug.LogError("Unexpected: " + request.error);
            forgotStatusText.text = "Something went wrong. Please try again.";
        }
    }

    public void OnResetPasswordClicked()
    {
        string token = tokenInput.text.Trim();
        string newPassword = newPasswordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        if (string.IsNullOrEmpty(token))
        { resetStatusText.text = "Please enter the reset code!"; return; }
        if (string.IsNullOrEmpty(newPassword))
        { resetStatusText.text = "Please enter a new password!"; return; }
        if (newPassword.Length < 6)
        { resetStatusText.text = "Password must be at least 6 characters."; return; }
        if (newPassword != confirmPassword)
        { resetStatusText.text = "Passwords do not match!"; return; }

        StartCoroutine(ResetPassword(token, newPassword));
    }

    private IEnumerator ResetPassword(string token, string newPassword)
    {
        resetStatusText.text = "Resetting password...";
        string json = "{\"Token\":\"" + token + "\",\"NewPassword\":\"" + newPassword + "\"}";

        UnityWebRequest request = new UnityWebRequest(apiUrl + "/reset-password", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.certificateHandler = new BypassCertificate();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("Connection error: " + request.error);
            resetStatusText.text = "Cannot connect to server.";
        }
        else if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;

            if (responseText.Contains("\"success\":true"))
            {
                loginPanel.SetActive(false);
                forgotPasswordPanel.SetActive(false);
                resetPasswordPanel.SetActive(false);
                successPanel.SetActive(true);
            }
            else if (responseText.Contains("expired"))
            { resetStatusText.text = "Reset code has expired. Request a new one."; }
            else if (responseText.Contains("Invalid"))
            { resetStatusText.text = "Invalid reset code. Please check and try again."; }
            else
            { resetStatusText.text = "Could not reset password. Please try again."; }
        }
        else
        {
            Debug.LogError("Unexpected: " + request.error);
            resetStatusText.text = "Something went wrong. Please try again.";
        }
    }

    private class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData) => true;
    }
}