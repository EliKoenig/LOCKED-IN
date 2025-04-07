using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualSceneController : MonoBehaviour
{
    public List<Transform> spawnpoints = new List<Transform>();

    public float initialSpawnRate = 5f; // Start with 5 seconds between spawns
    public float minSpawnRate = 1f; // Minimum spawn rate (1 second)
    public float spawnRateDecrease = 1f; // Decrease by 1 second
    public float decreaseInterval = 20f; // Decrease spawn time every 20 seconds
    string[] enemyTypes = { "Prefabs/VRangedEnemy", "Prefabs/VMeleeEnemy" };

    private float currentSpawnRate;
    private void Awake()
    {
        Time.timeScale = 1;
    }
    void Start()
    {
        currentSpawnRate = initialSpawnRate;
        StartCoroutine(SpawnEnemiesRepeatedly());
        StartCoroutine(DecreaseSpawnRateOverTime());
    }

    IEnumerator SpawnEnemiesRepeatedly()
    {
        while (true)
        {
            SpawnEnemy();
            Debug.Log(currentSpawnRate);
            yield return new WaitForSeconds(currentSpawnRate);
        }
    }

    IEnumerator DecreaseSpawnRateOverTime()
    {
        while (currentSpawnRate > minSpawnRate)
        {
            yield return new WaitForSeconds(decreaseInterval); // Wait 20 seconds
            currentSpawnRate = Mathf.Max(currentSpawnRate - spawnRateDecrease, minSpawnRate);
        }
    }

    void SpawnEnemy()
    {
        if (spawnpoints.Count == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        Transform spawnpoint = spawnpoints[Random.Range(0, spawnpoints.Count)];

        // Randomly select an enemy type
        string[] enemyTypes = { "Prefabs/VRangedEnemy", "Prefabs/VMeleeEnemy" };
        string selectedEnemy = enemyTypes[Random.Range(0, enemyTypes.Length)];

        GameObject enemyPrefab = Resources.Load<GameObject>(selectedEnemy);

        if (enemyPrefab == null)
        {
            Debug.LogError($"Failed to load enemy prefab: {selectedEnemy}. Check the path in Resources.");
            return;
        }

        Instantiate(enemyPrefab, spawnpoint.position, spawnpoint.rotation);
    }

}
