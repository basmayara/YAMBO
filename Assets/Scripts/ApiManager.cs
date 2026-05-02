using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance;

    private string gateway = "http://192.168.3.94:5000";

    public string Token { get; private set; }
    public string UserId { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // INSCRIPTION
    public IEnumerator Register(string email, string password, string username,
                                System.Action<bool, string> callback)
    {
        var json = $"{{\"email\":\"{email}\"," +
                   $"\"password\":\"{password}\"," +
                   $"\"username\":\"{username}\"}}";
        yield return StartCoroutine(Post(
            $"{gateway}/api/joueur/inscription", json, (ok, res) =>
            {
                if (ok)
                {
                    var r = JsonUtility.FromJson<ApiResponse>(res);
                    callback(r.success, r.message);
                }
                else callback(false, "Erreur réseau");
            }));
    }

    // LOGIN
    public IEnumerator Login(string email, string password,
                             System.Action<bool, string> callback)
    {
        var json = $"{{\"email\":\"{email}\"," +
                   $"\"password\":\"{password}\"}}";
        yield return StartCoroutine(Post(
            $"{gateway}/api/auth/login", json, (ok, res) =>
            {
                if (ok)
                {
                    var r = JsonUtility.FromJson<LoginResponse>(res);
                    if (r.success)
                    {
                        Token = r.token;
                        UserId = r.userId;
                        Username = r.username;
                        Email = email;
                        if (SaveManager.Instance != null)
                            SaveManager.Instance.SetPlayerDataFromLogin(
                                r.username, email, r.userId);
                    }
                    callback(r.success, r.message);
                }
                else callback(false, "Erreur réseau");
            }));
    }

    // GET PROFIL
    public IEnumerator GetProfil(System.Action<string> callback)
    {
        using var req = UnityWebRequest.Get(
            $"{gateway}/api/menu/profil/{UserId}");
        req.SetRequestHeader("Authorization", $"Bearer {Token}");
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.Success)
            callback(req.downloadHandler.text);
        else
        {
            Debug.LogError("GetProfil error: " + req.error);
            callback(null);
        }
    }

    // GET PROFIL BY ID
    public IEnumerator GetProfilById(string userId, System.Action<string> callback)
    {
        using var req = UnityWebRequest.Get(
            $"{gateway}/api/menu/profil/{userId}");
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.Success)
            callback(req.downloadHandler.text);
        else
        {
            Debug.LogError("GetProfilById error: " + req.error);
            callback(null);
        }
    }

    // UPDATE SCORE
    public IEnumerator UpdateScore(int score, System.Action<bool> callback)
    {
        var json = $"{{\"userId\":\"{UserId}\",\"score\":{score}}}";
        yield return StartCoroutine(Post(
            $"{gateway}/api/menu/score", json, (ok, _) => callback(ok)));
    }

    // SIGNALEMENT
    public IEnumerator PostSignalement(string joueurSignale, string raison,
                                        string description,
                                        System.Action<bool> callback)
    {
        var json = $"{{\"signalePar\":\"{Username}\"," +
                   $"\"joueurSignale\":\"{joueurSignale}\"," +
                   $"\"raison\":\"{raison}\"," +
                   $"\"description\":\"{description}\"}}";
        yield return StartCoroutine(Post(
            $"{gateway}/api/menu/signalement", json,
            (ok, _) => callback(ok)));
    }

    // HELPERS
    IEnumerator Post(string url, string json,
                     System.Action<bool, string> callback)
    {
        using var req = new UnityWebRequest(url, "POST");
        req.uploadHandler = new UploadHandlerRaw(
            Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();
        bool success = req.result == UnityWebRequest.Result.Success;
        callback(success, req.downloadHandler.text);
    }

    // MODELS
    [System.Serializable]
    public class ApiResponse
    {
        public bool success;
        public string message;
        public string userId;
        public string username;
    }

    [System.Serializable]
    public class LoginResponse
    {
        public bool success;
        public string token;
        public string userId;
        public string username;
        public string message;
    }
}