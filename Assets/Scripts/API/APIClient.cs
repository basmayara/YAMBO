using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace YAMBO.API
{
    public class APIClient : MonoBehaviour
    {
        public static APIClient Instance { get; private set; }

        private string _authToken;
        private string _refreshToken;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                _authToken    = PlayerPrefs.GetString("authToken", "");
                _refreshToken = PlayerPrefs.GetString("refreshToken", "");
                if (!string.IsNullOrEmpty(_authToken))
                    Debug.Log("[YAMBO] Session restored.");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // ---- AUTH ----

        public IEnumerator Register(string username, string email, string password,
            Action<AuthResponse> onSuccess, Action<string> onError)
        {
            var body = new RegisterRequest { username = username, email = email, password = password };
            yield return Post<RegisterRequest, AuthResponse>(
                APIEndpoints.Auth.Register, body,
                r => { Debug.Log("[YAMBO] Registered: " + r.username); onSuccess?.Invoke(r); },
                onError, requiresAuth: false);
        }

        public IEnumerator Login(string username, string password,
            Action<AuthResponse> onSuccess, Action<string> onError)
        {
            var body = new LoginRequest { username = username, password = password };
            yield return Post<LoginRequest, AuthResponse>(
                APIEndpoints.Auth.Login, body,
                r => { SaveSession(r); Debug.Log("[YAMBO] Logged in: " + r.username); onSuccess?.Invoke(r); },
                onError, requiresAuth: false);
        }

        public void Logout()
        {
            _authToken = _refreshToken = "";
            PlayerPrefs.DeleteKey("authToken");
            PlayerPrefs.DeleteKey("refreshToken");
            PlayerPrefs.DeleteKey("playerId");
            PlayerPrefs.DeleteKey("username");
            PlayerPrefs.Save();
            Debug.Log("[YAMBO] Logged out.");
        }

        // ---- QUIZ ----

        public IEnumerator GetRandomQuiz(Action<QuizQuestion> onSuccess, Action<string> onError)
        {
            yield return Get<QuizQuestion>(APIEndpoints.Quiz.Random, onSuccess, onError);
        }

        public IEnumerator GetQuizByCategory(string category,
            Action<QuizQuestion> onSuccess, Action<string> onError)
        {
            yield return Get<QuizQuestion>(APIEndpoints.Quiz.ByCategory(category), onSuccess, onError);
        }

        public IEnumerator SubmitQuizAnswer(int quizId, string playerAnswer, int timeTaken,
            Action<QuizSubmitResponse> onSuccess, Action<string> onError)
        {
            var body = new QuizSubmitRequest { quiz_id = quizId, player_answer = playerAnswer, time_taken = timeTaken };
            yield return Post<QuizSubmitRequest, QuizSubmitResponse>(
                APIEndpoints.Quiz.Submit, body,
                r =>
                {
                    Debug.Log(r.is_correct
                        ? "[YAMBO] Correct! +" + r.currency_earned + " C#"
                        : "[YAMBO] Wrong. Answer: " + r.correct_answer);
                    onSuccess?.Invoke(r);
                },
                onError);
        }

        // ---- SHOP ----

        public IEnumerator GetShopItems(Action<ShopItemsResponse> onSuccess, Action<string> onError)
        {
            yield return Get<ShopItemsResponse>(APIEndpoints.Shop.Items, onSuccess, onError);
        }

        public IEnumerator GetShopItemsByType(string type,
            Action<ShopItemsResponse> onSuccess, Action<string> onError)
        {
            yield return Get<ShopItemsResponse>(APIEndpoints.Shop.ItemsByType(type), onSuccess, onError);
        }

        public IEnumerator PurchaseItem(int itemId,
            Action<PurchaseResponse> onSuccess, Action<string> onError)
        {
            var body = new PurchaseRequest { item_id = itemId };
            yield return Post<PurchaseRequest, PurchaseResponse>(
                APIEndpoints.Shop.Purchase, body,
                r => { Debug.Log("[YAMBO] Purchased: " + r.item_acquired?.name + " | Balance: " + r.new_balance); onSuccess?.Invoke(r); },
                onError);
        }

        // ---- PLAYER ----

        public IEnumerator GetBalance(Action<BalanceResponse> onSuccess, Action<string> onError)
        {
            yield return Get<BalanceResponse>(APIEndpoints.Player.Balance, onSuccess, onError);
        }

        public IEnumerator GetInventory(Action<InventoryResponse> onSuccess, Action<string> onError)
        {
            yield return Get<InventoryResponse>(APIEndpoints.Player.Inventory, onSuccess, onError);
        }

        public IEnumerator GetProgression(Action<ProgressionResponse> onSuccess, Action<string> onError)
        {
            yield return Get<ProgressionResponse>(APIEndpoints.Player.Progression, onSuccess, onError);
        }

        // ---- UTILITIES ----

        public bool   IsLoggedIn()  => !string.IsNullOrEmpty(_authToken);
        public string GetPlayerId() => PlayerPrefs.GetString("playerId",  "");
        public string GetUsername() => PlayerPrefs.GetString("username",  "");

        // ---- PRIVATE HTTP ----

        private IEnumerator Get<TResponse>(string endpoint,
            Action<TResponse> onSuccess, Action<string> onError)
        {
            yield return SendWithRetry<TResponse>(
                () => BuildGet(APIEndpoints.BASE_URL + endpoint),
                onSuccess, onError);
        }

        private IEnumerator Post<TReq, TRes>(string endpoint, TReq data,
            Action<TRes> onSuccess, Action<string> onError, bool requiresAuth = true)
        {
            string json = JsonUtility.ToJson(data);
            yield return SendWithRetry<TRes>(
                () => BuildPost(APIEndpoints.BASE_URL + endpoint, json, requiresAuth),
                onSuccess, onError);
        }

        private IEnumerator SendWithRetry<TResponse>(
            Func<UnityWebRequest> build,
            Action<TResponse>     onSuccess,
            Action<string>        onError)
        {
            for (int attempt = 1; attempt <= APIEndpoints.MAX_RETRIES; attempt++)
            {
                using (UnityWebRequest req = build())
                {
                    req.timeout = APIEndpoints.TIMEOUT_SECONDS;
                    yield return req.SendWebRequest();

                    if (req.result == UnityWebRequest.Result.Success)
                    {
                        HandleSuccess(req, onSuccess, onError);
                        yield break;
                    }

                    if (req.responseCode == 401 && !string.IsNullOrEmpty(_refreshToken) && attempt == 1)
                    {
                        bool ok = false;
                        yield return RefreshToken(r => ok = r);
                        if (!ok) { Logout(); onError?.Invoke("Session expired."); yield break; }
                        attempt--; // retry same attempt with new token
                        continue;
                    }

                    bool networkErr = req.result == UnityWebRequest.Result.ConnectionError
                                   || req.result == UnityWebRequest.Result.DataProcessingError;

                    if (networkErr && attempt < APIEndpoints.MAX_RETRIES)
                    {
                        Debug.LogWarning("[YAMBO] Attempt " + attempt + " failed, retrying...");
                        yield return new WaitForSeconds(1f);
                    }
                    else
                    {
                        HandleError(req, onError);
                        yield break;
                    }
                }
            }
        }

        private UnityWebRequest BuildGet(string url)
        {
            var r = UnityWebRequest.Get(url);
            if (!string.IsNullOrEmpty(_authToken))
                r.SetRequestHeader("Authorization", "Bearer " + _authToken);
            return r;
        }

        private UnityWebRequest BuildPost(string url, string json, bool auth)
        {
            var r = new UnityWebRequest(url, "POST")
            {
                uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json)),
                downloadHandler = new DownloadHandlerBuffer()
            };
            r.SetRequestHeader("Content-Type", "application/json");
            if (auth && !string.IsNullOrEmpty(_authToken))
                r.SetRequestHeader("Authorization", "Bearer " + _authToken);
            return r;
        }

        private void HandleSuccess<T>(UnityWebRequest req, Action<T> onSuccess, Action<string> onError)
        {
            try   { onSuccess?.Invoke(JsonUtility.FromJson<T>(req.downloadHandler.text)); }
            catch (Exception e) { Debug.LogError("[YAMBO] Parse error: " + e.Message); onError?.Invoke("Failed to parse response."); }
        }

        private void HandleError(UnityWebRequest req, Action<string> onError)
        {
            string msg = "HTTP " + req.responseCode;
            try
            {
                var e = JsonUtility.FromJson<ErrorResponse>(req.downloadHandler.text);
                if (e != null) msg = e.message ?? e.error ?? msg;
            }
            catch { msg = req.error; }
            Debug.LogError("[YAMBO] " + msg);
            onError?.Invoke(msg);
        }

        private IEnumerator RefreshToken(Action<bool> cb)
        {
            string json = "{\"refreshToken\":\"" + _refreshToken + "\"}";
            using (var r = BuildPost(APIEndpoints.BASE_URL + APIEndpoints.Auth.Refresh, json, false))
            {
                r.timeout = APIEndpoints.TIMEOUT_SECONDS;
                yield return r.SendWebRequest();
                if (r.result == UnityWebRequest.Result.Success)
                {
                    try { SaveSession(JsonUtility.FromJson<AuthResponse>(r.downloadHandler.text)); cb(true); }
                    catch { cb(false); }
                }
                else cb(false);
            }
        }

        private void SaveSession(AuthResponse r)
        {
            _authToken    = r.token;
            _refreshToken = r.refreshToken ?? "";
            PlayerPrefs.SetString("authToken",    _authToken);
            PlayerPrefs.SetString("refreshToken", _refreshToken);
            PlayerPrefs.SetString("playerId",     r.playerId);
            PlayerPrefs.SetString("username",     r.username);
            PlayerPrefs.Save();
        }
    }
}