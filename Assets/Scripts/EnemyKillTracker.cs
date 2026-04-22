using UnityEngine;

// Singleton that tracks enemy kills across the scene
public class EnemyKillTracker : MonoBehaviour
{
    public static EnemyKillTracker Instance;

    public int killCount = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterKill()
    {
        killCount++;
        Debug.Log($"Kill count: {killCount}");
    }

    public void Reset()
    {
        killCount = 0;
    }
}
