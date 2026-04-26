#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using YAMBO.API;

namespace YAMBO.Editor
{
    /// <summary>
    /// Unity Editor Window to test the YAMBO API without launching the game.
    /// Menu: YAMBO > API Tester
    /// </summary>
    public class YAMBOAPITester : EditorWindow
    {
        // ---- Tabs ----
        private int _tab = 0;
        private readonly string[] _tabs = { "Auth", "Quiz", "Shop", "Player", "About" };

        // ---- Auth fields ----
        private string _username  = "testuser";
        private string _email     = "test@yambo.game";
        private string _password  = "password123";
        private string _authLog   = "";

        // ---- Quiz fields ----
        private string _quizCategory   = "";
        private string _quizLog        = "";
        private string _submitAnswer   = "";
        private int    _submitQuizId   = 1;
        private int    _timeTaken      = 5;

        // ---- Shop fields ----
        private string _shopLog      = "";
        private string _shopType     = "";
        private int    _purchaseId   = 1;

        // ---- Player fields ----
        private string _playerLog = "";

        // ---- Scroll positions ----
        private Vector2 _scroll;

        [MenuItem("YAMBO/API Tester")]
        public static void Open()
        {
            var w = GetWindow<YAMBOAPITester>("YAMBO API Tester");
            w.minSize = new Vector2(480, 600);
            w.Show();
        }

        private void OnGUI()
        {
            DrawHeader();
            _tab = GUILayout.Toolbar(_tab, _tabs, GUILayout.Height(30));
            EditorGUILayout.Space(6);

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            switch (_tab)
            {
                case 0: DrawAuthTab();   break;
                case 1: DrawQuizTab();   break;
                case 2: DrawShopTab();   break;
                case 3: DrawPlayerTab(); break;
                case 4: DrawAboutTab();  break;
            }
            EditorGUILayout.EndScrollView();
        }

        // ===== HEADER =====

        private void DrawHeader()
        {
            var bg = new GUIStyle(EditorStyles.toolbar) { fixedHeight = 50 };
            var title = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize  = 18,
                alignment = TextAnchor.MiddleCenter
            };
            title.normal.textColor = new Color(0.4f, 0.8f, 1f);

            EditorGUILayout.BeginHorizontal(bg, GUILayout.Height(50));
            GUILayout.Label("YAMBO  API Tester", title);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4);

            bool running = Application.isPlaying;
            string status = running
                ? (APIClient.Instance != null && APIClient.Instance.IsLoggedIn()
                    ? "Connected as: " + APIClient.Instance.GetUsername()
                    : "Play mode - not logged in")
                : "Not in Play Mode (press Play to use API calls)";

            var statusStyle = new GUIStyle(EditorStyles.helpBox) { fontSize = 11 };
            EditorGUILayout.LabelField(status, statusStyle);
            EditorGUILayout.Space(4);
        }

        // ===== AUTH TAB =====

        private void DrawAuthTab()
        {
            SectionLabel("Register / Login");

            _username = EditorGUILayout.TextField("Username", _username);
            _email    = EditorGUILayout.TextField("Email",    _email);
            _password = EditorGUILayout.PasswordField("Password", _password);

            EditorGUILayout.Space(8);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Register", GUILayout.Height(30)))
            {
                if (!CheckPlayMode()) return;
                _authLog = "Sending register...";
                APIClient.Instance.StartCoroutine(
                    APIClient.Instance.Register(_username, _email, _password,
                        r => _authLog = "Registered: " + r.username + " | " + r.message,
                        e => _authLog = "ERROR: " + e));
            }

            if (GUILayout.Button("Login", GUILayout.Height(30)))
            {
                if (!CheckPlayMode()) return;
                _authLog = "Sending login...";
                APIClient.Instance.StartCoroutine(
                    APIClient.Instance.Login(_username, _password,
                        r => _authLog = "Logged in: " + r.username + " | Token: " + r.token.Substring(0, Mathf.Min(20, r.token.Length)) + "...",
                        e => _authLog = "ERROR: " + e));
            }

            if (GUILayout.Button("Logout", GUILayout.Height(30)))
            {
                if (!CheckPlayMode()) return;
                APIClient.Instance.Logout();
                _authLog = "Logged out.";
            }

            EditorGUILayout.EndHorizontal();
            DrawLog(_authLog);
        }

        // ===== QUIZ TAB =====

        private void DrawQuizTab()
        {
            SectionLabel("Get Quiz");

            _quizCategory = EditorGUILayout.TextField("Category (empty = random)", _quizCategory);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Get Random Quiz", GUILayout.Height(28)))
            {
                if (!CheckPlayMode()) return;
                _quizLog = "Loading quiz...";
                APIClient.Instance.StartCoroutine(
                    APIClient.Instance.GetRandomQuiz(
                        q => _quizLog = "Q[" + q.quiz_id + "]: " + q.question + "\n  Correct: " + q.correct_answer + "\n  Reward: " + q.reward_currency + " C#",
                        e => _quizLog = "ERROR: " + e));
            }
            if (GUILayout.Button("Get By Category", GUILayout.Height(28)))
            {
                if (!CheckPlayMode()) return;
                _quizLog = "Loading quiz (" + _quizCategory + ")...";
                APIClient.Instance.StartCoroutine(
                    APIClient.Instance.GetQuizByCategory(_quizCategory,
                        q => _quizLog = "Q[" + q.quiz_id + "]: " + q.question + "\n  Category: " + q.category,
                        e => _quizLog = "ERROR: " + e));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(8);
            SectionLabel("Submit Answer");

            _submitQuizId  = EditorGUILayout.IntField("Quiz ID",       _submitQuizId);
            _submitAnswer  = EditorGUILayout.TextField("Player Answer", _submitAnswer);
            _timeTaken     = EditorGUILayout.IntField("Time Taken (s)", _timeTaken);

            if (GUILayout.Button("Submit Answer", GUILayout.Height(28)))
            {
                if (!CheckPlayMode()) return;
                APIClient.Instance.StartCoroutine(
                    APIClient.Instance.SubmitQuizAnswer(_submitQuizId, _submitAnswer, _timeTaken,
                        r => _quizLog = (r.is_correct ? "CORRECT" : "WRONG") + " | Earned: " + r.currency_earned + " C# | Balance: " + r.new_balance,
                        e => _quizLog = "ERROR: " + e));
            }

            DrawLog(_quizLog);
        }

        // ===== SHOP TAB =====

        private void DrawShopTab()
        {
            SectionLabel("Shop Items");

            _shopType = EditorGUILayout.TextField("Filter by type (empty = all)", _shopType);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Get All Items", GUILayout.Height(28)))
            {
                if (!CheckPlayMode()) return;
                _shopLog = "Loading shop...";
                APIClient.Instance.StartCoroutine(
                    APIClient.Instance.GetShopItems(
                        r =>
                        {
                            _shopLog = r.total_count + " items:\n";
                            if (r.items != null)
                                foreach (var it in r.items)
                                    _shopLog += "  [" + it.item_id + "] " + it.item_name + " - " + it.price + " C# (" + it.item_type + ")\n";
                        },
                        e => _shopLog = "ERROR: " + e));
            }
            if (GUILayout.Button("Filter by Type", GUILayout.Height(28)))
            {
                if (!CheckPlayMode()) return;
                APIClient.Instance.StartCoroutine(
                    APIClient.Instance.GetShopItemsByType(_shopType,
                        r => _shopLog = "Items of type '" + _shopType + "': " + (r.items?.Count ?? 0),
                        e => _shopLog = "ERROR: " + e));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(8);
            SectionLabel("Purchase");

            _purchaseId = EditorGUILayout.IntField("Item ID to buy", _purchaseId);
            if (GUILayout.Button("Purchase Item", GUILayout.Height(28)))
            {
                if (!CheckPlayMode()) return;
                APIClient.Instance.StartCoroutine(
                    APIClient.Instance.PurchaseItem(_purchaseId,
                        r => _shopLog = "Purchased: " + r.item_acquired?.name + " | New balance: " + r.new_balance + " C#",
                        e => _shopLog = "ERROR: " + e));
            }

            DrawLog(_shopLog);
        }

        // ===== PLAYER TAB =====

        private void DrawPlayerTab()
        {
            SectionLabel("Player Info");

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Get Balance", GUILayout.Height(28)))
            {
                if (!CheckPlayMode()) return;
                APIClient.Instance.StartCoroutine(
                    APIClient.Instance.GetBalance(
                        r => _playerLog = "Balance: " + r.balance + " C# | Earned: " + r.total_earned + " | Spent: " + r.total_spent + " | Ratio: " + (r.SpendRatio * 100f).ToString("F0") + "%",
                        e => _playerLog = "ERROR: " + e));
            }
            if (GUILayout.Button("Get Inventory", GUILayout.Height(28)))
            {
                if (!CheckPlayMode()) return;
                APIClient.Instance.StartCoroutine(
                    APIClient.Instance.GetInventory(
                        r =>
                        {
                            _playerLog = r.total_items + " items in inventory:\n";
                            if (r.items != null)
                                foreach (var it in r.items)
                                    _playerLog += "  [" + it.item_id + "] " + it.item_name + " x" + it.quantity + "\n";
                        },
                        e => _playerLog = "ERROR: " + e));
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Get Progression", GUILayout.Height(28)))
            {
                if (!CheckPlayMode()) return;
                APIClient.Instance.StartCoroutine(
                    APIClient.Instance.GetProgression(
                        r => _playerLog = "Level: " + r.level_reached + " | Score: " + r.total_score + " | Quizzes: " + r.quizzes_solved + "/" + r.quizzes_total + " (" + r.CompletionPercent + ")",
                        e => _playerLog = "ERROR: " + e));
            }

            EditorGUILayout.Space(4);
            SectionLabel("Session Info");
            if (Application.isPlaying && APIClient.Instance != null)
            {
                EditorGUILayout.LabelField("Player ID", APIClient.Instance.GetPlayerId());
                EditorGUILayout.LabelField("Username",  APIClient.Instance.GetUsername());
                EditorGUILayout.LabelField("Logged in", APIClient.Instance.IsLoggedIn().ToString());
            }
            else
            {
                EditorGUILayout.HelpBox("Enter Play Mode to see session info.", MessageType.Info);
            }

            DrawLog(_playerLog);
        }

        // ===== ABOUT TAB =====

        private void DrawAboutTab()
        {
            EditorGUILayout.Space(10);
            var center = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter, fontSize = 14 };
            GUILayout.Label("YAMBO API Tester", center);
            EditorGUILayout.Space(6);
            EditorGUILayout.HelpBox(
                "This tool lets you test the YAMBO API directly from the Unity Editor.\n\n" +
                "You MUST be in Play Mode for API calls to work (APIClient.Instance requires a running scene).\n\n" +
                "Endpoints configured in: APIEndpoints.cs\n" +
                "Base URL (Editor):   http://localhost:3000/api\n" +
                "Base URL (Build):    https://api.yambo.game/api",
                MessageType.Info);

            EditorGUILayout.Space(8);
            SectionLabel("Keyboard Shortcuts");
            EditorGUILayout.LabelField("Open window:   YAMBO > API Tester (menu)");

            EditorGUILayout.Space(8);
            if (GUILayout.Button("Open API Endpoints file", GUILayout.Height(28)))
            {
                var path = "Assets/Scripts/API/APIEndpoints.cs";
                var obj = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (obj != null) AssetDatabase.OpenAsset(obj);
            }
        }

        // ===== HELPERS =====

        private void SectionLabel(string label)
        {
            var s = new GUIStyle(EditorStyles.boldLabel) { fontSize = 12 };
            s.normal.textColor = new Color(0.4f, 0.85f, 1f);
            EditorGUILayout.LabelField(label, s);
            Rect r = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(r, new Color(0.4f, 0.85f, 1f, 0.3f));
            EditorGUILayout.Space(4);
        }

        private void DrawLog(string log)
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Response:", EditorStyles.boldLabel);
            var logStyle = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap  = true,
                richText  = true,
                fontSize  = 11
            };
            EditorGUILayout.SelectableLabel(
                string.IsNullOrEmpty(log) ? "(no response yet)" : log,
                logStyle,
                GUILayout.MinHeight(80));
        }

        private bool CheckPlayMode()
        {
            if (!Application.isPlaying || APIClient.Instance == null)
            {
                _authLog = _quizLog = _shopLog = _playerLog =
                    "ERROR: Enter Play Mode first (APIClient.Instance is null).";
                Repaint();
                return false;
            }
            return true;
        }
    }
}
#endif