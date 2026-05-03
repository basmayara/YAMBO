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
    public string apiUrl = "http://192.168.1.8:8080/api/auth/login";
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
        string json = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\"}";

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("ngrok-skip-browser-warning", "true");
        request.certificateHandler = new BypassCertificate();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            statusText.text = "Cannot connect to server.";
        }
        else if (request.responseCode == 401)
        {
            statusText.text = "Incorrect email or password.";
        }
        else if (request.responseCode == 400)
        {
            statusText.text = "Invalid request.";
        }
        else if (request.responseCode == 200)
        {
            statusText.text = "Login successful!";
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            statusText.text = "Error: " + request.responseCode;
        }
    }

    private class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData) => true;
    }
}