using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LoginAPI : MonoBehaviour
{
    private string url = "https://localhost:44362/api/Utilisateurs/register";

    public void Register(string nom, string email, string password, string role)
    {
        StartCoroutine(RegisterRequest(nom, email, password, role));
    }

    IEnumerator RegisterRequest(string nom, string email, string password, string role)
    {
        UserData user = new UserData(nom, email, password, role);
        string json = JsonUtility.ToJson(user);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        request.certificateHandler = new BypassCertificate();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Success: User Registered with Role: " + role);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
}

[System.Serializable]
public class UserData
{
    public string Nom;
    public string Email;
    public string MotDePasse;
    public string Role;

    public UserData(string nom, string email, string password, string role)
    {
        Nom = nom;
        Email = email;
        MotDePasse = password;
        Role = role;
    }
}