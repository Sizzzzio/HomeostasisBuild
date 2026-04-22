using System.Collections;
using UnityEngine;

public class TreeBoss : BossBase
{
    [Header("Root Attack")]
    public GameObject rootPrefab;
    public float rootInterval = 3f;
    public float rootDuration = 2f;
    public float rootSpread = 2f;
    public int rootDamage = 20;
    public LayerMask groundLayer;

    [Header("Projectile Attack")]
    public GameObject projectilePrefab;
    public float projectileInterval = 5f;
    public float projectileForce = 8f;
    public int projectileDamage = 15;

    private Transform player;

    protected override void Start()
    {
        base.Start();

        Player p = FindAnyObjectByType<Player>();
        if (p != null)
            player = p.transform;

        StartCoroutine(AttackLoop());
    }

    IEnumerator AttackLoop()
    {
        StartCoroutine(RootLoop());
        StartCoroutine(ProjectileLoop());
        yield break;
    }

    IEnumerator RootLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(rootInterval);
            if (player != null)
                yield return StartCoroutine(DoRootAttack());
        }
    }

    IEnumerator ProjectileLoop()
    {
        yield return new WaitForSeconds(2f);
        while (true)
        {
            yield return new WaitForSeconds(projectileInterval);
            if (player != null)
                DoProjectileAttack();
        }
    }

    IEnumerator DoRootAttack()
    {
        if (rootPrefab == null) yield break;

        // Spawn 3 roots at fixed offsets from player's X, always on ground
        float playerX = player.position.x;

        for (int i = -1; i <= 1; i++)
        {
            float x = playerX + i * rootSpread;

            // Cast straight down from a high point to find ground
            Vector2 castOrigin = new Vector2(x, player.position.y + 15f);
            RaycastHit2D hit = Physics2D.Raycast(castOrigin, Vector2.down, 30f, groundLayer);

            if (hit.collider != null)
            {
                Debug.Log($"Ground hit at {hit.point} for root {i}");
                Vector3 rootPos = new Vector3(hit.point.x, hit.point.y, 0f);
                StartCoroutine(ShowRoot(rootPos));
            }
            else
            {
                Debug.LogWarning($"No ground found for root {i} casting from {castOrigin}");
            }
        }

        yield return new WaitForSeconds(rootDuration);
    }

    IEnumerator ShowRoot(Vector3 pos)
    {
        Debug.Log($"ShowRoot at {pos}");
        GameObject root = Instantiate(rootPrefab, pos, Quaternion.identity);

        // Short delay so player can see and react
        yield return new WaitForSeconds(0.25f);

        // Check for player overlap
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, 0.5f);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Player p = hit.GetComponent<Player>();
                if (p != null) p.TakeDamage(rootDamage);
            }
        }

        yield return new WaitForSeconds(rootDuration - 0.25f);

        if (root != null)
            Destroy(root);
    }

    void DoProjectileAttack()
    {
        if (projectilePrefab == null || player == null) return;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();

        if (projRb != null)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            projRb.linearVelocity = new Vector2(dir.x * projectileForce, projectileForce * 0.8f);
        }

        TreeBossProjectile projScript = proj.GetComponent<TreeBossProjectile>();
        if (projScript != null)
            projScript.damage = projectileDamage;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}