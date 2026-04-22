using System.Collections;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 50;
    public int damage = 20;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float detectionRange = 6f;
    public float attackRange = 1.5f;
    public float minimumAttackDistance = 0.6f;

    [Header("Attack Timing")]
    public float startupFrames = 0.5f;
    public float attackCooldown = 2f;
    public float lungeForce = 6f;
    public float swipeRadius = 1.2f;
    public float swipeReach = 1f;

    [Header("Visuals")]
    public Color windupColor = Color.yellow;
    public Color attackColor = Color.red;
    public GameObject attackVisual;
    public float visualDuration = 0.2f;
    public float baseVisualScaleX = 1f;
    public float maxVisualScaleX = 2.5f;

    private int currentHealth;
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private SpriteRenderer attackVisualSR;  // SpriteRenderer on the attack visual
    private Color originalColor;

    private float lastAttackTime = -Mathf.Infinity;
    private bool isAttacking = false;
    private float facingDirection = 1f;
    private float distanceAtAttack = 0f;

    private enum State { Idle, Chasing, Windup, Attacking }
    private State currentState = State.Idle;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (sr != null)
            originalColor = sr.color;

        if (attackVisual != null)
        {
            attackVisual.SetActive(false);
            attackVisualSR = attackVisual.GetComponent<SpriteRenderer>();
        }

        Player p = FindAnyObjectByType<Player>();
        if (p != null)
            player = p.transform;
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        float distance = Vector2.Distance(transform.position, player.position);

        facingDirection = player.position.x >= transform.position.x ? 1f : -1f;
        if (sr != null)
            sr.flipX = facingDirection < 0;

        bool playerInAttackZone = distance <= attackRange && distance >= minimumAttackDistance;

        if (playerInAttackZone && Time.time - lastAttackTime >= attackCooldown)
        {
            distanceAtAttack = distance;
            StartCoroutine(AttackSequence());
        }
        else if (distance <= detectionRange && distance > attackRange)
        {
            currentState = State.Chasing;
            ChasePlayer();
        }
        else if (distance > detectionRange)
        {
            currentState = State.Idle;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        currentState = State.Windup;

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        if (sr != null) sr.color = windupColor;

        yield return new WaitForSeconds(startupFrames);

        currentState = State.Attacking;
        if (sr != null) sr.color = attackColor;

        if (attackVisual != null)
            StartCoroutine(ShowAttackVisual());

        Vector2 lungeDir = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(lungeDir.x * lungeForce, rb.linearVelocity.y);

        yield return new WaitForSeconds(0.15f);

        Vector2 swipeOrigin = (Vector2)transform.position + new Vector2(facingDirection * swipeReach, 0f);
        Collider2D[] hits = Physics2D.OverlapCircleAll(swipeOrigin, swipeRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Player playerScript = hit.GetComponent<Player>();
                if (playerScript != null)
                {
                    playerScript.TakeDamage(damage);
                    Debug.Log($"{gameObject.name} swiped the player for {damage} damage");
                }
                break;
            }
        }

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        if (sr != null) sr.color = originalColor;

        lastAttackTime = Time.time;
        isAttacking = false;
        currentState = State.Idle;
    }

    private IEnumerator ShowAttackVisual()
    {
        // Scale based on distance
        float t = Mathf.InverseLerp(minimumAttackDistance, attackRange, distanceAtAttack);
        float scaledX = Mathf.Lerp(baseVisualScaleX, maxVisualScaleX, t);

        // Use flipX on the SpriteRenderer instead of scale manipulation
        // This avoids scale sign conflicts
        if (attackVisualSR != null)
            attackVisualSR.flipX = facingDirection < 0;

        // Keep scale always positive, just stretch based on distance
        Vector3 scale = attackVisual.transform.localScale;
        scale.x = scaledX;
        scale.y = Mathf.Abs(scale.y);
        attackVisual.transform.localScale = scale;

        attackVisual.SetActive(true);

        yield return new WaitForSeconds(visualDuration);

        attackVisual.SetActive(false);

        // Reset scale
        scale.x = baseVisualScaleX;
        attackVisual.transform.localScale = scale;
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
        Color prev = sr.color;
        sr.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        sr.color = prev;
    }

    void Die()
    {
        if (EnemyKillTracker.Instance != null)
            EnemyKillTracker.Instance.RegisterKill();
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.magenta;
        Vector2 swipeOrigin = (Vector2)transform.position + new Vector2(1f * swipeReach, 0f);
        Gizmos.DrawWireSphere(swipeOrigin, swipeRadius);
    }
}