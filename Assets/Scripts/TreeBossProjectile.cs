using UnityEngine;

public class TreeBossProjectile : MonoBehaviour
{
    public int damage = 15;
    public GameObject breakEffect;     // Optional particle or sprite on impact

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Deal damage if hit player
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
                player.TakeDamage(damage);
        }

        // Break on any surface
        if (breakEffect != null)
            Instantiate(breakEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
