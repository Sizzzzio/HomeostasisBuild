using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [Header("Boss Prefabs")]
    public GameObject treeBossPrefab;       // < 10 kills
    public GameObject turretBossPrefab;     // >= 10 kills
    public int killThreshold = 10;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    [Header("Boss Health Bar UI")]
    public BossHealthBar bossHealthBar;

    private bool bossSpawned = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (bossSpawned) return;

        SpawnBoss();
    }

    void SpawnBoss()
    {
        bossSpawned = true;

        int kills = EnemyKillTracker.Instance != null ? EnemyKillTracker.Instance.killCount : 0;
        GameObject prefab = kills < killThreshold ? treeBossPrefab : turretBossPrefab;

        if (prefab == null)
        {
            Debug.LogError("BossSpawner: prefab not assigned!");
            return;
        }

        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject boss = Instantiate(prefab, pos, Quaternion.identity);

        Debug.Log($"Spawned {prefab.name} (kills: {kills})");

        // Hook up health bar
        Debug.Log($"bossHealthBar null: {bossHealthBar == null}");
        if (bossHealthBar != null)
        {
            BossBase bossBase = boss.GetComponent<BossBase>();
            Debug.Log($"bossBase null: {bossBase == null}");
            if (bossBase != null)
            {
                bossHealthBar.SetBoss(bossBase);
                Debug.Log("SetBoss called successfully");
            }
        }
    }

    public void ResetSpawner()
    {
        bossSpawned = false;
    }
}