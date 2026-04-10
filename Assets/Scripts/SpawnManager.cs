using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
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
            Debug.LogWarning("SpawnManager: no spawn points found!");
            return;
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("SpawnManager: Enemy Prefab is not assigned!");
            return;
        }

        SpawnAll();
    }

    void SpawnAll()
    {
        Player player = FindAnyObjectByType<Player>();

        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            activeEnemies.Add(newEnemy);
            Debug.Log($"Spawned {newEnemy.name} at {spawnPoint.name}");

            if (player != null)
            {
                cog_behavior cog = newEnemy.GetComponent<cog_behavior>();
                if (cog != null)
                    cog.player = player.transform;
            }
        }
    }

    // Called by Player.cs when the player dies
    public void ResetAll()
    {
        // Destroy all existing enemies
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        activeEnemies.Clear();

        // Respawn fresh enemies at all spawn points
        SpawnAll();
    }
}