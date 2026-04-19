using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 40;
    public float detectionRange = 8f;
    public float fireRate = 3f;             // Seconds between shots
    public GameObject laserPrefab;          // Reuse LaserBolt prefab
    public float laserSpeed = 10f;
    public int laserDamage = 10;

    [Header("Visuals")]
    public Color hitColor = Color.white;

    private int currentHealth;
    private SpriteRenderer sr;
    private Color originalColor;
    private float nextFireTime = 0f;
    private Transform player;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;

        Player p = FindAnyObjectByType<Player>();
        if (p != null)
            player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Flip to face player
        if (sr != null)
            sr.flipX = player.position.x < transform.position.x;

        if (distance <= detectionRange && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Fire()
    {
        if (laserPrefab == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.Euler(0f, 0f, angle));

        LaserBolt bolt = laser.GetComponent<LaserBolt>();
        if (bolt != null)
            bolt.Init(direction, laserSpeed, laserDamage, 4f);

        Debug.Log($"Turret fired at player");
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        Debug.Log($"{gameObject.name} took {dmg} damage. HP: {currentHealth}/{maxHealth}");

        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator HitFlash()
    {
        if (sr == null) yield break;
        sr.color = hitColor;
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
