using System.Collections;
using UnityEngine;

public class LaserBolt : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private int damage;
    private float lifetime;

    private SpriteRenderer sr;

    public void Init(Vector2 direction, float speed, int damage, float lifetime)
    {
        this.direction = direction;
        this.speed = speed;
        this.damage = damage;
        this.lifetime = lifetime;

        sr = GetComponent<SpriteRenderer>();

        // Destroy after max lifetime
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;
        if (other.CompareTag("BushBum")) return;

        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage);
            Debug.Log($"Laser hit {other.gameObject.name} for {damage} damage");
            Destroy(gameObject);
            return;
        }

        // Hit a wall or non-damageable collider — destroy
        if (!other.isTrigger)
            Destroy(gameObject);
    }
}
