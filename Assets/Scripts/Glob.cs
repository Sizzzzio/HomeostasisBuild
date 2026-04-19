using System.Collections;
using UnityEngine;

public class Glob : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 20;
    public int damage = 15;
    public float damageRadius = 0.4f;

    [Header("Bob Settings")]
    public float bobHeight = 0.3f;      // How far it bobs up and down
    public float bobSpeed = 2f;         // How fast it bobs

    [Header("Visuals")]
    public Color hitColor = Color.white;

    private int currentHealth;
    private SpriteRenderer sr;
    private Color originalColor;
    private Vector3 startPosition;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        startPosition = transform.position;

        if (sr != null)
            originalColor = sr.color;
    }

    void Update()
    {
        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

        // Deal contact damage to player
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, damageRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Player player = hit.GetComponent<Player>();
                if (player != null)
                    player.TakeDamage(damage);
            }
        }
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
