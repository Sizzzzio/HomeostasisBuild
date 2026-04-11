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
    public Transform respawnPoint;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isInvincible = false;
    public float invincibilityDuration = 0.5f;

    private SpriteRenderer sp;
    private MeleeAttack meleeAttack;
    private SpawnManager spawnManager;
    private ItemManager itemManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        meleeAttack = GetComponent<MeleeAttack>();
        spawnManager = FindAnyObjectByType<SpawnManager>();
        itemManager = FindAnyObjectByType<ItemManager>();
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (Input.GetKeyDown(KeyCode.F))
            meleeAttack?.TryAttack();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Damage"))
        {
            TakeDamage(25);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryTakeDamageFromEnemy(other.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryTakeDamageFromEnemy(other.gameObject);
    }

    private void TryTakeDamageFromEnemy(GameObject other)
    {
        cog_behavior cog = other.GetComponent<cog_behavior>();
        if (cog != null)
            TakeDamage(cog.damage);
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        health -= amount;
        health = Mathf.Max(health, 0);

        Debug.Log($"Player took {amount} damage. HP: {health}/{maxHealth}");

        StartCoroutine(InvincibilityFrames());

        if (health <= 0)
            Die();
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
        // Reset health
        health = maxHealth;

        // Move to respawn point
        if (respawnPoint != null)
            transform.position = respawnPoint.position;
        else
            Debug.LogWarning("No respawn point assigned on Player!");

        // Stop momentum
        rb.linearVelocity = Vector2.zero;

        // Reset all enemies
        if (spawnManager != null)
            spawnManager.ResetAll();

        // Reset all items — pickups reappear, abilities stripped
        if (itemManager != null)
            itemManager.ResetAll();

        // Strip all abilities from player
        ResetAbilities();

        // Brief invincibility after respawn
        StartCoroutine(InvincibilityFrames());

        Debug.Log("Player died — all entities reset.");
    }

    private void ResetAbilities()
    {
        // Disable sawblade launcher
        SawbladeLauncher launcher = GetComponent<SawbladeLauncher>();
        if (launcher != null)
            launcher.enabled = false;

        // Hide sawblade visual
        Transform sawbladeVisual = transform.Find("SawbladeVisual");
        if (sawbladeVisual != null)
            sawbladeVisual.gameObject.SetActive(false);

        // Add more ability resets here as you build them
    }
}