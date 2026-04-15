using UnityEngine;
using System.Collections.Generic;

public class MeleeSpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;

    private Transform[] spawnPoints;
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        spawnPoints = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            spawnPoints[i] = transform.GetChild(i);

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("MeleeSpawnManager: no spawn points found!");
            return;
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("MeleeSpawnManager: Enemy Prefab is not assigned!");
            return;
        }

        SpawnAll();
    }

    void SpawnAll()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            activeEnemies.Add(newEnemy);
            Debug.Log($"Spawned {newEnemy.name} at {spawnPoint.name}");
        }
    }

    public void ResetAll()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        activeEnemies.Clear();
        SpawnAll();
    }
}
