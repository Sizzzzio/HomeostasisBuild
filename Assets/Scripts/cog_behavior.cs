using UnityEngine;

public class cog_behavior : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public float followRange = 5f;
    public int damage = 25;             // Read by Player.cs TryTakeDamageFromEnemy
    public float damageCooldown = 1f;   // Handled by Player's invincibility frames

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= followRange)
            FollowPlayer();
    }

    void FollowPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }
}