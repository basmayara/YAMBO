using System;
using System.Collections.Generic;

namespace YAMBO.API
{
    // AUTH

    [Serializable]
    public class RegisterRequest
    {
        public string username;
        public string email;
        public string password;
    }

    [Serializable]
    public class LoginRequest
    {
        public string username;
        public string password;
    }

    [Serializable]
    public class AuthResponse
    {
        public bool   success;
        public string message;
        public string playerId;
        public string username;
        public string token;
        public string refreshToken;
        public int    expiresIn;
    }

    // QUIZ

    [Serializable]
    public class QuizQuestion
    {
        public int      quiz_id;
        public string   category;
        public string   difficulty;
        public string   question;
        public string   correct_answer;
        public string[] wrong_answers;
        public int      reward_currency;
        public int      time_limit;

        public string[] GetShuffledAnswers()
        {
            var all = new List<string>(wrong_answers) { correct_answer };
            var rng = new Random();
            for (int i = all.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                string tmp = all[i]; all[i] = all[j]; all[j] = tmp;
            }
            return all.ToArray();
        }
    }

    [Serializable]
    public class QuizSubmitRequest
    {
        public int    quiz_id;
        public string player_answer;
        public int    time_taken;
    }

    [Serializable]
    public class QuizSubmitResponse
    {
        public bool   is_correct;
        public string correct_answer;
        public int    currency_earned;
        public int    new_balance;
        public string message;
        public string explanation;
    }

    // SHOP

    [Serializable]
    public class ShopItem
    {
        public int    item_id;
        public string item_name;
        public string item_type;
        public int    price;
        public string description;
        public string sprite_url;
        public bool   is_available;
        public bool   is_limited;
        public int    stock;

        public bool CanBePurchased => is_available && (stock == -1 || stock > 0);
    }

    [Serializable]
    public class ShopItemsResponse
    {
        public List<ShopItem> items;
        public int            total_count;
    }

    [Serializable]
    public class PurchaseRequest
    {
        public int item_id;
    }

    [Serializable]
    public class ItemAcquired
    {
        public int    id;
        public string name;
        public string item_type;
    }

    [Serializable]
    public class PurchaseResponse
    {
        public bool         success;
        public string       message;
        public int          new_balance;
        public ItemAcquired item_acquired;
    }

    // INVENTORY

    [Serializable]
    public class InventoryItem
    {
        public int    inventory_id;
        public int    item_id;
        public string item_name;
        public string item_type;
        public int    quantity;
        public string purchased_at;

        public DateTime PurchasedAt =>
            DateTime.TryParse(purchased_at, out var d) ? d : DateTime.MinValue;
    }

    [Serializable]
    public class InventoryResponse
    {
        public List<InventoryItem> items;
        public int                 total_items;
    }

    // BALANCE

    [Serializable]
    public class BalanceResponse
    {
        public int balance;
        public int total_earned;
        public int total_spent;

        public float SpendRatio => total_earned > 0 ? (float)total_spent / total_earned : 0f;
    }

    // PROGRESSION

    [Serializable]
    public class ProgressionResponse
    {
        public int    level_reached;
        public string current_checkpoint;
        public int    total_score;
        public int    quizzes_solved;
        public int    quizzes_total;
        public float  completion_rate;

        public string CompletionPercent => $"{(int)(completion_rate * 100)}%";
    }

    // LEADERBOARD

    [Serializable]
    public class LeaderboardEntry
    {
        public int    rank;
        public string username;
        public int    total_score;
        public int    level_reached;
    }

    [Serializable]
    public class LeaderboardResponse
    {
        public List<LeaderboardEntry> entries;
        public int                    player_rank;
    }

    // ERRORS

    [Serializable]
    public class ErrorResponse
    {
        public string error;
        public string message;
        public int    status_code;
    }
}
