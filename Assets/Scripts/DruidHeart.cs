using System.Collections;
using UnityEngine;

public class DruidHeart : MonoBehaviour
{
    [Header("Settings")]
    public float trapDuration = 2f;         // How long the enemy is stunned
    public float cooldown = 2f;             // Time between traps
    public float detectionRange = 10f;      // How far it looks for enemies
    public GameObject vinePrefab;           // Vine sprite that appears on trapped enemy

    private float nextTrapTime = 0f;
    private bool isActive = false;

    void Start()
    {
        isActive = true;
    }

    void OnEnable()
    {
        isActive = true;
    }

    void OnDisable()
    {
        isActive = false;
    }

    void Update()
    {
        if (!isActive) return;

        if (Time.time >= nextTrapTime)
        {
            GameObject nearest = FindNearestEnemy();
            if (nearest != null)
            {
                StartCoroutine(TrapEnemy(nearest));
                nextTrapTime = Time.time + trapDuration + cooldown;
            }
        }
    }

    GameObject FindNearestEnemy()
    {
        float closestDist = Mathf.Infinity;
        GameObject closest = null;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRange);
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

    private IEnumerator TrapEnemy(GameObject enemy)
    {
        if (enemy == null) yield break;

        Debug.Log($"DruidHeart: trapping {enemy.name}");

        // Freeze enemy movement
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        cog_behavior cog = enemy.GetComponent<cog_behavior>();
        MeleeEnemy meleeEnemy = enemy.GetComponent<MeleeEnemy>();

        Vector2 originalVelocity = Vector2.zero;
        float originalSpeed = 0f;
        float originalMeleeSpeed = 0f;

        if (enemyRb != null)
        {
            originalVelocity = enemyRb.linearVelocity;
            enemyRb.linearVelocity = Vector2.zero;
        }

        // Disable movement scripts
        if (cog != null)
        {
            originalSpeed = cog.speed;
            cog.speed = 0f;
        }

        if (meleeEnemy != null)
        {
            originalMeleeSpeed = meleeEnemy.moveSpeed;
            meleeEnemy.moveSpeed = 0f;
        }

        // Spawn vine visual on the enemy
        GameObject vine = null;
        if (vinePrefab != null)
        {
            vine = Instantiate(vinePrefab, enemy.transform.position, Quaternion.identity);
            vine.transform.SetParent(enemy.transform);
            vine.transform.localPosition = Vector3.zero;
        }

        // Tint enemy green to show trapped state
        SpriteRenderer enemySr = enemy.GetComponent<SpriteRenderer>();
        Color originalColor = Color.white;
        if (enemySr != null)
        {
            originalColor = enemySr.color;
            enemySr.color = Color.green;
        }

        yield return new WaitForSeconds(trapDuration);

        // Check enemy still exists
        if (enemy == null) yield break;

        // Restore movement
        if (enemyRb != null)
            enemyRb.linearVelocity = originalVelocity;

        if (cog != null)
            cog.speed = originalSpeed;

        if (meleeEnemy != null)
            meleeEnemy.moveSpeed = originalMeleeSpeed;

        if (enemySr != null)
            enemySr.color = originalColor;

        // Remove vine
        if (vine != null)
            Destroy(vine);

        Debug.Log($"DruidHeart: {enemy.name} released");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
