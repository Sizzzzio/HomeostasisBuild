using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;

    private Transform[] spawnPoints;

    void Start()
    {
        Debug.Log($"SpawnManager Start — direct child count: {transform.childCount}");

        spawnPoints = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            spawnPoints[i] = transform.GetChild(i);
            Debug.Log($"Spawn point {i}: {spawnPoints[i].name} at {spawnPoints[i].position}");
        }

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

        SpawnOne();
    }

    void SpawnOne()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Debug.Log($"Spawning at {spawnPoint.name} ({spawnPoint.position})");

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log($"Spawned: {newEnemy.name}");

        Player player = FindAnyObjectByType<Player>();
        if (player != null)
        {
            cog_behavior cog = newEnemy.GetComponent<cog_behavior>();
            if (cog != null)
                cog.player = player.transform;
        }
    }
}