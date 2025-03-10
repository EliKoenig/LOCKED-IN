using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplaySceneController : MonoBehaviour
{
    public List<Transform> spawnpoints = new List<Transform>();
    public List<Transform> ammoPoints = new List<Transform>();

    public float initialSpawnRate = 5f; // Start with 5 seconds between spawns
    public float minSpawnRate = 1f; // Minimum spawn rate (1 second)
    public float spawnRateDecrease = 1f; // Decrease by 1 second
    public float decreaseInterval = 20f; // Decrease spawn time every 20 seconds

    private float currentSpawnRate;

    void Start()
    {
        currentSpawnRate = initialSpawnRate;
        StartCoroutine(SpawnEnemiesRepeatedly());
        StartCoroutine(DecreaseSpawnRateOverTime());
        StartCoroutine(SpawnAmmoCratesRepeatedly()); // Start ammo spawn coroutine
    }

    IEnumerator SpawnEnemiesRepeatedly()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(currentSpawnRate);
        }
    }

    IEnumerator DecreaseSpawnRateOverTime()
    {
        while (currentSpawnRate > minSpawnRate)
        {
            yield return new WaitForSeconds(decreaseInterval);
            currentSpawnRate = Mathf.Max(currentSpawnRate - spawnRateDecrease, minSpawnRate);
        }
    }

    IEnumerator SpawnAmmoCratesRepeatedly()
    {
        while (true)
        {
            yield return new WaitForSeconds(12f); // Spawn ammo crate every 20 seconds
            SpawnAmmoCrate();
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
        string[] enemyTypes = { "Prefabs/RangedEnemy", "Prefabs/MeleeEnemy" };
        string selectedEnemy = enemyTypes[Random.Range(0, enemyTypes.Length)];
        GameObject enemyPrefab = Resources.Load<GameObject>(selectedEnemy);

        if (enemyPrefab == null)
        {
            Debug.LogError($"Failed to load enemy prefab: {selectedEnemy}. Check the path in Resources.");
            return;
        }

        Instantiate(enemyPrefab, spawnpoint.position, spawnpoint.rotation);
    }

    void SpawnAmmoCrate()
    {
        if (ammoPoints.Count == 0)
        {
            Debug.LogError("No ammo points assigned!");
            return;
        }

        Transform ammoSpawn = ammoPoints[Random.Range(0, ammoPoints.Count)];
        GameObject ammoPrefab = Resources.Load<GameObject>("Prefabs/BulletPickup");

        if (ammoPrefab == null)
        {
            Debug.LogError("Failed to load ammo pickup prefab. Check the path in Resources.");
            return;
        }

        Instantiate(ammoPrefab, ammoSpawn.position, ammoSpawn.rotation);
    }
}

