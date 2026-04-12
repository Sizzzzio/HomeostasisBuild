using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public int damage = 25;     // Health penalty for falling

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Player player = other.GetComponent<Player>();
        if (player != null)
            player.FallDeath(damage);
    }
}
