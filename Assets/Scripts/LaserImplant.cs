using System.Collections;
using UnityEngine;

public class LaserImplant : MonoBehaviour
{
    [Header("Laser Settings")]
    public float fireRate = 3f;             // Seconds between shots
    public int damage = 5;
    public float laserSpeed = 15f;
    public float laserLifetime = 3f;        // Max time before disappearing
    public GameObject laserPrefab;          // Assign laser bolt prefab

    [Header("Visual")]
    public GameObject implantVisual;        // Sprite attached to player

    private float nextFireTime = 0f;
    private SpriteRenderer sp;

    void Start()
    {
        sp = GetComponent<SpriteRenderer>();

        if (implantVisual != null)
            implantVisual.SetActive(true);
    }

    void OnDisable()
    {
        if (implantVisual != null)
            implantVisual.SetActive(false);
    }

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            FireAtNearestEnemy();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FireAtNearestEnemy()
    {
        // Find nearest IDamageable enemy
        GameObject nearest = FindNearestEnemy();
        if (nearest == null)
        {
            Debug.Log("LaserImplant: no enemy in scene to target");
            return;
        }

        if (laserPrefab == null)
        {
            Debug.LogError("LaserImplant: laserPrefab not assigned!");
            return;
        }

        // Aim laser toward enemy
        Vector2 direction = (nearest.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.Euler(0f, 0f, angle));

        LaserBolt bolt = laser.GetComponent<LaserBolt>();
        if (bolt != null)
            bolt.Init(direction, laserSpeed, damage, laserLifetime);

        Debug.Log($"LaserImplant: fired at {nearest.name}");
    }

    GameObject FindNearestEnemy()
    {
        float closestDist = Mathf.Infinity;
        GameObject closest = null;

        // Find all IDamageable objects that aren't the player
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 20f);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player")) continue;
            if (hit.CompareTag("BushBum")) continue;

            IDamageable target = hit.GetComponent<IDamageable>();
            if (target != null)
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = hit.gameObject;
                }
            }
        }

        return closest;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 20f);
    }
}
