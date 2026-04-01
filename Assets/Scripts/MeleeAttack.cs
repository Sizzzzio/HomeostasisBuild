using System.Collections;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int damage = 999;
    public float attackRange = 1.2f;
    public float attackCooldown = 0.4f;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    [Header("Attack Visual")]
    public GameObject attackVisual;         // Assign your slash/sword sprite GameObject here
    public float visualDuration = 0.12f;    // How long it stays visible (seconds)

    private float lastAttackTime;

    public void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;

        Debug.Log("Melee attack fired!");

        // Show the attack visual
        if (attackVisual != null)
            StartCoroutine(ShowAttackVisual());

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        Debug.Log($"Hits detected: {hits.Length}");

        foreach (Collider2D hit in hits)
        {
            Debug.Log($"Hit: {hit.gameObject.name}");

            IDamageable target = hit.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning($"{hit.gameObject.name} has no IDamageable component!");
            }

            Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 knockDir = (hit.transform.position - transform.position).normalized;
                enemyRb.AddForce(knockDir * 300f);
            }
        }
    }

    private IEnumerator ShowAttackVisual()
    {
        attackVisual.SetActive(true);
        yield return new WaitForSeconds(visualDuration);
        attackVisual.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
