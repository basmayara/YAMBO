using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;

    [HideInInspector] public FirebaseAuth auth;
    [HideInInspector] public FirebaseFirestore db;
    [HideInInspector] public FirebaseUser currentUser;

    public bool isReady = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeFirebase();
    }

    async void InitializeFirebase()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (dependencyStatus == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseFirestore.DefaultInstance;
            isReady = true;
            Debug.Log("Firebase prêt !");
        }
        else
        {
            Debug.LogError("Firebase erreur : " + dependencyStatus);
        }
    }

    // INSCRIPTION
    public async Task<bool> Register(string email, string password, string playerName)
    {
        try
        {
            var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            currentUser = result.User;

            // Créer le profil dans Firestore
            PlayerData data = new PlayerData();
            data.playerName = playerName;
            data.email = email;
            data.userId = currentUser.UserId;

            await SavePlayerData(data);
            Debug.Log("Inscription réussie !");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur inscription : " + e.Message);
            return false;
        }
    }

    // CONNEXION
    public async Task<bool> Login(string email, string password)
    {
        try
        {
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            currentUser = result.User;
            Debug.Log("Connexion réussie !");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur connexion : " + e.Message);
            return false;
        }
    }

    // DÉCONNEXION
    public void Logout()
    {
        auth.SignOut();
        currentUser = null;
        Debug.Log("Déconnecté.");
    }

    // SAUVEGARDER LES DONNÉES
    public async Task SavePlayerData(PlayerData data)
    {
        if (currentUser == null) return;

        var docRef = db.Collection("players").Document(currentUser.UserId);

        var dict = new System.Collections.Generic.Dictionary<string, object>
        {
            { "playerName",  data.playerName  },
            { "level",       data.level       },
            { "scoreTotal",  data.scoreTotal  },
            { "bestScore",   data.bestScore   },
            { "lives",       data.lives       },
            { "gamesPlayed", data.gamesPlayed },
            { "xpCurrent",   data.xpCurrent  },
            { "xpMax",       data.xpMax       },
            { "email",       data.email       },
        };

        await docRef.SetAsync(dict);
        Debug.Log("Données sauvegardées !");
    }

    // CHARGER LES DONNÉES
    public async Task<PlayerData> LoadPlayerData()
    {
        if (currentUser == null) return null;

        var docRef = db.Collection("players").Document(currentUser.UserId);
        var snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            PlayerData data = new PlayerData();
            data.playerName = snapshot.GetValue<string>("playerName");
            data.level = snapshot.GetValue<int>("level");
            data.scoreTotal = snapshot.GetValue<int>("scoreTotal");
            data.bestScore = snapshot.GetValue<int>("bestScore");
            data.lives = snapshot.GetValue<int>("lives");
            data.gamesPlayed = snapshot.GetValue<int>("gamesPlayed");
            data.xpCurrent = snapshot.GetValue<int>("xpCurrent");
            data.xpMax = snapshot.GetValue<int>("xpMax");
            data.email = snapshot.GetValue<string>("email");
            data.userId = currentUser.UserId;
            Debug.Log("Données chargées !");
            return data;
        }

        return null;
    }
}