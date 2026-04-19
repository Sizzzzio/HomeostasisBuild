using System.Collections;
using UnityEngine;

public class BushBum : MonoBehaviour
{
    [Header("Stats")]
    public float damagePerTick = 2f;
    public float tickRate = 0.5f;           // How often damage is applied
    public float attackRange = 3f;          // Range to detect and attack enemies
    public float retreatDistance = 1f;      // How close to player before stopping retreat

    [Header("Movement")]
    public float followSpeed = 3f;          // Speed when following player
    public float chaseSpeed = 4f;           // Speed when chasing enemy
    public float retreatSpeed = 4f;         // Speed when retreating to player
    public float followOffset = 1.5f;       // Distance behind player to idle at

    private Transform player;
    private Transform targetEnemy;
    private SpriteRenderer sr;

    private bool isAttacking = false;
    private bool isRetreating = false;
    private float lastDamageTime;

    private enum State { Following, Chasing, Attacking, Retreating }
    private State currentState = State.Following;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        Player p = FindAnyObjectByType<Player>();
        if (p != null)
            player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        switch (currentState)
        {
            case State.Following:
                FollowPlayer();
                FindNearestEnemy();
                break;

            case State.Chasing:
                ChaseEnemy();
                break;

            case State.Attacking:
                AttackEnemy();
                break;

            case State.Retreating:
                RetreatToPlayer();
                break;
        }

        // Flip sprite based on movement direction
        if (sr != null)
        {
            if (currentState == State.Retreating || currentState == State.Following)
                sr.flipX = player.position.x < transform.position.x;
            else if (targetEnemy != null)
                sr.flipX = targetEnemy.position.x < transform.position.x;
        }
    }

    void FollowPlayer()
    {
        // Stay slightly behind the player
        Vector3 targetPos = player.position + new Vector3(-followOffset * (player.GetComponent<SpriteRenderer>().flipX ? -1f : 1f), 0f, 0f);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, followSpeed * Time.deltaTime);
    }

    void FindNearestEnemy()
    {
        // Look for any IDamageable in range that isn't the player
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        float closestDist = Mathf.Infinity;
        Transform closest = null;

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
                    closest = hit.transform;
                }
            }
        }

        if (closest != null)
        {
            targetEnemy = closest;
            currentState = State.Chasing;
        }
    }

    void ChaseEnemy()
    {
        if (targetEnemy == null || targetEnemy.gameObject == null)
        {
            currentState = State.Retreating;
            return;
        }

        float dist = Vector2.Distance(transform.position, targetEnemy.position);

        if (dist < 0.3f)
        {
            currentState = State.Attacking;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetEnemy.position, chaseSpeed * Time.deltaTime);
        }
    }

    void AttackEnemy()
    {
        if (targetEnemy == null || targetEnemy.gameObject == null)
        {
            // Enemy died — retreat to player
            targetEnemy = null;
            currentState = State.Retreating;
            return;
        }

        // Apply damage per tick
        if (Time.time - lastDamageTime >= tickRate)
        {
            IDamageable target = targetEnemy.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(Mathf.RoundToInt(damagePerTick));
                lastDamageTime = Time.time;
                StartCoroutine(AttackFlash());
                Debug.Log($"BushBum dealt {damagePerTick} damage to {targetEnemy.name}");
            }
        }

        // Stay on top of enemy
        transform.position = Vector3.MoveTowards(transform.position, targetEnemy.position, chaseSpeed * Time.deltaTime);
    }

    void RetreatToPlayer()
    {
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer <= retreatDistance)
        {
            targetEnemy = null;
            currentState = State.Following;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, retreatSpeed * Time.deltaTime);
        }
    }

    private IEnumerator AttackFlash()
    {
        if (sr == null) yield break;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        sr.color = Color.white;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
