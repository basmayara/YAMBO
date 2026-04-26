using UnityEngine;
using YAMBO.API;

public class GameFlowExample : MonoBehaviour
{
    void Start()
    {
        // 1. Connexion
        StartCoroutine(APIClient.Instance.Login(
            "shadow_gamer",
            "motdepasse",
            OnLoginSuccess,
            error => Debug.LogError(error)
        ));
    }

    void OnLoginSuccess(AuthResponse auth)
    {
        Debug.Log($"Bienvenue {auth.username} !");

        // 2. Charger le solde
        StartCoroutine(APIClient.Instance.GetBalance(
            balance => Debug.Log($"Vous avez {balance.balance} C#"),
            error => Debug.LogError(error)
        ));

        // 3. Charger un quiz
        StartCoroutine(APIClient.Instance.GetRandomQuiz(
            quiz => Debug.Log($"Question : {quiz.question}"),
            error => Debug.LogError(error)
        ));

        // 4. Charger la boutique
        StartCoroutine(APIClient.Instance.GetShopItems(
            items => Debug.Log($"{items.items.Count} items disponibles"),
            error => Debug.LogError(error)
        ));
    }
}