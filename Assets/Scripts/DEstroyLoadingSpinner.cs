using UnityEngine;

/// <summary>
/// Script qui détruit TOUS les LoadingSpinner et LoadingPanel au démarrage
/// Attacher à ShopManagerObject
/// </summary>
public class DestroyLoadingSpinner : MonoBehaviour
{
    void Awake()
    {
        // Chercher TOUS les objets avec "Loading" dans le nom
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        int destroyedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Loading") || obj.name.Contains("Spinner"))
            {
                Debug.Log($"🔥 Destruction de : {obj.name}");
                Destroy(obj);
                destroyedCount++;
            }
        }

        Debug.Log($"✅ {destroyedCount} objets Loading/Spinner détruits !");
    }
}