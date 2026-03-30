using UnityEngine;

public class cog_behavior : MonoBehaviour
{
    public Transform player;        // Assign in Inspector
    public float speed = 3f;        // Movement speed
    public float followRange = 5f;  // Detection range

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
        {
            FollowPlayer();
            FlipSprite();
        }
    }

    void FollowPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    //void FlipSprite()
    //{
    //    // Flip based on player's horizontal position
    //    sr.flipX = player.position.x < transform.position.x;
    //}
}