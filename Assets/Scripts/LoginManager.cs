using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text statusText;

    [Header("API Config")]
    public string apiUrl = "https://localhost:7220/api/auth/login";

    public void OnLoginButtonClicked()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            statusText.text = "Please enter email and password!";
            return;
        }
        StartCoroutine(LoginCoroutine(email, password));
    }

    private IEnumerator LoginCoroutine(string email, string password)
    {
        statusText.text = "Connecting...";
        string json = "{\"Email\":\"" + email + "\",\"Password\":\"" + password + "\"}";

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.certificateHandler = new BypassCertificate();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            // Developer sees real error, user sees friendly message
            Debug.LogError("Connection error: " + request.error);
            statusText.text = "Cannot connect to server. Check your connection.";
        }
        else if (request.responseCode == 401)
        {
            statusText.text = "Incorrect email or password.";
        }
        else if (request.responseCode == 400)
        {
            statusText.text = "Please fill in all fields.";
        }
        else if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("Response: " + responseText);

            if (responseText.Contains("true"))
            {
                statusText.text = "Login successful!";
                Debug.Log("Login OK!");
                // TODO: load next scene
            }
            else
            {
                statusText.text = "Incorrect email or password.";
            }
        }
        else
        {
            Debug.LogError("Unexpected error: " + request.error + " | " + request.downloadHandler.text);
            statusText.text = "Something went wrong. Please try again.";
        }
    }

    private class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData) => true;
    }
}