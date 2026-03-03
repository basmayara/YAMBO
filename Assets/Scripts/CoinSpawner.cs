using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public float spawnRate = 2f;
    private float nextSpawn = 0f;

    void Update()
    {
        // كنأكدو أن الـ Prefab موجود وما تمسحش
        if (coinPrefab != null && Time.time > nextSpawn)
        {
            nextSpawn = Time.time + spawnRate;

            // كنزيدو مسافة عشوائية باش ما يجيوش متزاحمين
            float randomX = Random.Range(10f, 20f);

            // استعملي الـ Y ديال الـ Spawner اللي ديجا حطيتيه فوق الرملة
            Vector3 spawnPos = new Vector3(transform.position.x + randomX, transform.position.y, 0);

            Instantiate(coinPrefab, spawnPos, Quaternion.identity);
        }
    }
}