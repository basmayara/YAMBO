namespace YAMBO.API
{
    public static class APIEndpoints
    {
#if UNITY_EDITOR
        public const string BASE_URL = "http://localhost:3000/api";
#else
        public const string BASE_URL = "https://api.yambo.game/api";
#endif

        public const int TIMEOUT_SECONDS = 10;
        public const int MAX_RETRIES     = 3;

        public static class Auth
        {
            public const string Register = "/auth/register";
            public const string Login    = "/auth/login";
            public const string Logout   = "/auth/logout";
            public const string Refresh  = "/auth/refresh";
        }

        public static class Quiz
        {
            public const string Random  = "/quiz/random";
            public const string Submit  = "/quiz/submit";
            public const string History = "/quiz/history";

            public static string ByCategory(string category)   => $"/quiz/random?category={category}";
            public static string ByDifficulty(string difficulty) => $"/quiz/random?difficulty={difficulty}";
        }

        public static class Shop
        {
            public const string Items    = "/shop/items";
            public const string Purchase = "/shop/purchase";

            public static string ItemsByType(string type) => $"/shop/items?type={type}";
        }

        public static class Player
        {
            public const string Balance     = "/player/balance";
            public const string Inventory   = "/player/inventory";
            public const string Progression = "/player/progression";
            public const string Profile     = "/player/profile";
        }

        public static class Leaderboard
        {
            public const string Global = "/leaderboard/global";
            public const string Weekly = "/leaderboard/weekly";
        }
    }
}
