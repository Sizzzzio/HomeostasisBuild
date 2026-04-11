using System.Collections;
using UnityEngine;

public class SawbladeProjectile : MonoBehaviour
{
    private float launchSpeed;
    private float returnSpeed;
    private int damage;
    private int maxHits;
    private int hitCount = 0;
    private Transform playerTransform;
    private System.Action onReturned;

    private bool returning = false;
    private Vector2 moveDirection;
    private Vector3 spawnPosition;

    public float maxDistance = 8f;      // Returns automatically after this distance

    private SpriteRenderer sr;

    public void Init(float direction, float launchSpeed, float returnSpeed, int damage, int maxHits, Transform playerTransform, System.Action onReturned)
    {
        this.launchSpeed = launchSpeed;
        this.returnSpeed = returnSpeed;
        this.damage = damage;
        this.maxHits = maxHits;
        this.playerTransform = playerTransform;
        this.onReturned = onReturned;

        moveDirection = new Vector2(direction, 0f);
        spawnPosition = transform.position;

        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!returning)
        {
            transform.position += (Vector3)(moveDirection * launchSpeed * Time.deltaTime);
            transform.Rotate(0f, 0f, -600f * Time.deltaTime);

            // Return automatically if it travels too far
            if (Vector3.Distance(transform.position, spawnPosition) >= maxDistance)
            {
                Debug.Log("Sawblade reached max distance, returning");
                returning = true;
            }
        }
        else
        {
            Vector3 dir = (playerTransform.position - transform.position).normalized;
            transform.position += dir * returnSpeed * Time.deltaTime;
            transform.Rotate(0f, 0f, -600f * Time.deltaTime);

            if (Vector3.Distance(transform.position, playerTransform.position) < 0.3f)
            {
                onReturned?.Invoke();
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;

        Debug.Log($"Sawblade trigger hit: {other.gameObject.name}");

        IDamageable target = other.GetComponent<IDamageable>();
        if (target == null)
            target = other.GetComponentInParent<IDamageable>();

        if (target != null)
        {
            target.TakeDamage(damage);
            hitCount++;
            Debug.Log($"Sawblade hit {other.gameObject.name}, hitCount: {hitCount}/{maxHits}");

            if (sr != null)
                StartCoroutine(HitFlash());

            if (hitCount >= maxHits)
                returning = true;
        }
    }

    // Also check with overlap since two triggers won't collide
    void FixedUpdate()
    {
        if (returning) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.4f);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player")) continue;
            if (hit.gameObject == gameObject) continue;

            IDamageable target = hit.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage);
                hitCount++;
                Debug.Log($"Sawblade overlap hit {hit.gameObject.name}, hitCount: {hitCount}/{maxHits}");

                if (sr != null)
                    StartCoroutine(HitFlash());

                if (hitCount >= maxHits)
                    returning = true;

                break;
            }
        }
    }

    IEnumerator HitFlash()
    {
        sr.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        sr.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        sr.color = Color.white;
    }
}
