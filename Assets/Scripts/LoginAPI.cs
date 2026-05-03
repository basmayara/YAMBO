using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class LoginAPI : MonoBehaviour
{
    private string url = "http://localhost:8081/api/joueur/inscription";

    public void Register(string nom, string email, string password, Action<string> onSuccess, Action<string> onError)
    {
        StartCoroutine(RegisterRequest(nom, email, password, onSuccess, onError));
    }

    IEnumerator RegisterRequest(string nom, string email, string password, Action<string> onSuccess, Action<string> onError)
    {
        var newUser = new UserDTO
        {
            nom = nom,
            email = email,
            motDePasse = password
        };

        string json = JsonUtility.ToJson(newUser);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.certificateHandler = new BypassCertificate();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            onSuccess?.Invoke(request.downloadHandler.text);
        }
        else
        {
            string errorResponse = request.downloadHandler.text;
            try
            {
                int start = errorResponse.IndexOf("\"erreur\":\"") + 10;
                int end = errorResponse.IndexOf("\"", start);
                string erreur = errorResponse.Substring(start, end - start);
                onError?.Invoke(erreur);
            }
            catch
            {
                onError?.Invoke("Erreur de connexion au serveur");
            }
        }
    }
}

[System.Serializable]
public class UserDTO
{
    public string nom;
    public string email;
    public string motDePasse;
}