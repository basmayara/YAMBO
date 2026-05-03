using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class LoginAPI : MonoBehaviour
{
    private string baseUrl = "http://192.168.1.8:8080/api";

    public void Register(string name, string email, string password,
        Action<string> onSuccess, Action<string> onError)
    {
        StartCoroutine(RegisterRequest(name, email, password, onSuccess, onError));
    }

    IEnumerator RegisterRequest(string name, string email, string password,
        Action<string> onSuccess, Action<string> onError)
    {
        string json = "{\"name\":\"" + name + "\",\"email\":\"" + email + "\",\"password\":\"" + password + "\"}";
        UnityWebRequest req = new UnityWebRequest(baseUrl + "/player/register", "POST");
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string id = ExtractValue(req.downloadHandler.text, "id");
            if (!string.IsNullOrEmpty(id))
                PlayerPrefs.SetInt("playerId", int.Parse(id));
            Debug.Log(" Register OK: " + req.downloadHandler.text);
            onSuccess?.Invoke(req.downloadHandler.text);
        }
        else
        {
            Debug.LogError(" Register Error");
            Debug.LogError("Response Code: " + req.responseCode);
            Debug.LogError("Error: " + req.error);
            Debug.LogError("Body: " + req.downloadHandler.text); onError?.Invoke(req.downloadHandler.text);
        }
    }



    private string ExtractValue(string json, string key)
    {
        string search = "\"" + key + "\":";
        int start = json.IndexOf(search);
        if (start == -1) return "";
        start += search.Length;
        if (json[start] == '"')
        {
            start++;
            int end = json.IndexOf("\"", start);
            return json.Substring(start, end - start);
        }
        else
        {
            int end = json.IndexOfAny(new char[] { ',', '}' }, start);
            return json.Substring(start, end - start).Trim();
        }
    }
}