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

    [Header("Wall Jump")]
    public Transform wallCheckLeft;         // Empty child GameObject on the left side of player
    public Transform wallCheckRight;        // Empty child GameObject on the right side of player
    public float wallCheckRadius = 0.2f;
    public float wallJumpForceX = 8f;       // How far away from the wall
    public float wallJumpForceY = 10f;      // How high up the wall jump goes
    public float wallSlideSpeed = 1f;       // How fast the player slides down a wall

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isTouchingWallLeft;
    private bool isTouchingWallRight;
    private bool isWallSliding;
    private bool isInvincible = false;
    public float invincibilityDuration = 0.5f;

    private SpriteRenderer sp;
    private MeleeAttack meleeAttack;
    private SpawnManager spawnManager;
    private ItemManager itemManager;

    private Vector3 lastGroundedPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        meleeAttack = GetComponent<MeleeAttack>();
        spawnManager = FindAnyObjectByType<SpawnManager>();
        itemManager = FindAnyObjectByType<ItemManager>();

        if (respawnPoint != null)
            lastGroundedPosition = respawnPoint.position;
        else
            lastGroundedPosition = transform.position;
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Wall slide — slow fall when pressing into a wall in the air
        isWallSliding = (isTouchingWallLeft || isTouchingWallRight) && !isGrounded && rb.linearVelocity.y < 0;
        if (isWallSliding)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);

        // Jump — normal or wall jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else if (isTouchingWallLeft)
            {
                // Push right and up off the left wall
                rb.linearVelocity = new Vector2(wallJumpForceX, wallJumpForceY);
            }
            else if (isTouchingWallRight)
            {
                // Push left and up off the right wall
                rb.linearVelocity = new Vector2(-wallJumpForceX, wallJumpForceY);
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
            meleeAttack?.TryAttack();

        if (isGrounded)
            lastGroundedPosition = transform.position;
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (wallCheckLeft != null)
            isTouchingWallLeft = Physics2D.OverlapCircle(wallCheckLeft.position, wallCheckRadius, groundLayer);
        if (wallCheckRight != null)
            isTouchingWallRight = Physics2D.OverlapCircle(wallCheckRight.position, wallCheckRadius, groundLayer);

        Debug.Log($"grounded: {isGrounded}, wallLeft: {isTouchingWallLeft}, wallRight: {isTouchingWallRight}");
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

    public void FallDeath(int damage)
    {
        if (isInvincible) return;

        health -= damage;
        health = Mathf.Max(health, 0);

        Debug.Log($"Player fell! Took {damage} damage. HP: {health}/{maxHealth}");

        transform.position = lastGroundedPosition;
        rb.linearVelocity = Vector2.zero;

        StartCoroutine(InvincibilityFrames());

        if (health <= 0)
            Die();
    }

    public void UpdateCheckpoint(Vector3 position)
    {
        lastGroundedPosition = position;
        Debug.Log($"Checkpoint saved at {position}");
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
        health = maxHealth;

        if (respawnPoint != null)
            transform.position = respawnPoint.position;
        else
            Debug.LogWarning("No respawn point assigned on Player!");

        rb.linearVelocity = Vector2.zero;

        if (spawnManager != null)
            spawnManager.ResetAll();

        if (itemManager != null)
            itemManager.ResetAll();

        ResetAbilities();

        StartCoroutine(InvincibilityFrames());

        Debug.Log("Player died — all entities reset.");
    }

    private void ResetAbilities()
    {
        SawbladeLauncher launcher = GetComponent<SawbladeLauncher>();
        if (launcher != null)
            launcher.enabled = false;

        Transform sawbladeVisual = transform.Find("SawbladeVisual");
        if (sawbladeVisual != null)
            sawbladeVisual.gameObject.SetActive(false);
    }


}