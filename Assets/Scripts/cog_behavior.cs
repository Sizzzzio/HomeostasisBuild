using UnityEngine;

public class cog_behavior : MonoBehaviour
{
    public Transform player;        // Assign in Inspector
    public float speed = 3f;        // Movement speed
    public float followRange = 5f;  // Detection range
    public int damage = 25;         // Damage dealt on contact
    public float damageCooldown = 1f; // Seconds between hits

    private SpriteRenderer sr;
    private float lastDamageTime = -Mathf.Infinity;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= followRange)
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamagePlayer(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryDamagePlayer(collision.gameObject);
    }

    private void TryDamagePlayer(GameObject other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time - lastDamageTime < damageCooldown) return;

        lastDamageTime = Time.time;

        Player playerScript = other.GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(damage);
        }
    }
}