using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int health = 100;
    public int maxHealth = 100;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isInvincible = false;
    public float invincibilityDuration = 0.5f;

    private SpriteRenderer sp;
    private MeleeAttack meleeAttack;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        meleeAttack = GetComponent<MeleeAttack>();
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            //UseSelectedItem();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            meleeAttack?.TryAttack();
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        health -= amount;
        health = Mathf.Max(health, 0);

        Debug.Log($"Player took {amount} damage. HP: {health}/{maxHealth}");

        StartCoroutine(InvincibilityFrames());

        if (health <= 0)
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Damage"))
        {
            TakeDamage(25);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            sp.color = sp.color == Color.white ? Color.black : Color.white;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        sp.color = Color.white;
        isInvincible = false;
    }

    private void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}