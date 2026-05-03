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
    public float invincibilityDuration = 0.5f;
    public float enemyKnockbackForce = 8f;

    [Header("Managers — assign in Inspector")]
    public SpawnManager spawnManager;
    public MeleeSpawnManager meleeSpawnManager;
    public ItemManager itemManager;

    [Header("Item Visuals — assign in Inspector")]
    public GameObject sawbladeVisual;
    public GameObject laserImplantVisual;
    public GameObject druidHeartVisual;
    public GameObject mushVisual;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool wasGrounded = false;
    private bool isInvincible = false;
    private bool isDead = false;

    private SpriteRenderer sp;
    private MeleeAttack meleeAttack;
    private AirDash airDash;
    private BossSpawner bossSpawner;        // ← added

    private float baseMoveSpeed;
    private float baseJumpForce;
    private float baseGravityScale;
    private int baseMaxHealth;
    private int baseMeleeAttackDamage;
    private Vector3 lastGroundedPosition;

    private GameObject activeBushBum;
    private float facingDirection = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        meleeAttack = GetComponent<MeleeAttack>();
        airDash = GetComponent<AirDash>();

        if (spawnManager == null)
            spawnManager = FindAnyObjectByType<SpawnManager>();
        if (meleeSpawnManager == null)
            meleeSpawnManager = FindAnyObjectByType<MeleeSpawnManager>();
        if (itemManager == null)
            itemManager = FindAnyObjectByType<ItemManager>();
        bossSpawner = FindAnyObjectByType<BossSpawner>();

        baseMoveSpeed = moveSpeed;
        baseJumpForce = jumpForce;
        baseGravityScale = rb.gravityScale;
        baseMaxHealth = maxHealth;
        baseMeleeAttackDamage = meleeAttack != null ? meleeAttack.damage : 0;

        if (respawnPoint != null)
            lastGroundedPosition = respawnPoint.position;
        else
            lastGroundedPosition = transform.position;

        if (sawbladeVisual != null) sawbladeVisual.SetActive(false);
        if (laserImplantVisual != null) laserImplantVisual.SetActive(false);
        if (druidHeartVisual != null) druidHeartVisual.SetActive(false);
        if (mushVisual != null) mushVisual.SetActive(false);
    }

    public void SetBushBum(GameObject bushBum)
    {
        activeBushBum = bushBum;
    }

    void Update()
    {
        if (isDead) return;

        float moveInput = Input.GetAxis("Horizontal");

        bool isDashing = airDash != null && airDash.IsDashing();
        if (!isDashing)
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0)
        {
            sp.flipX = false;
            facingDirection = 1f;
            FlipAttackPoint(1f);
        }
        else if (moveInput < 0)
        {
            sp.flipX = true;
            facingDirection = -1f;
            FlipAttackPoint(-1f);
        }

        if (isGrounded && !wasGrounded)
            airDash?.ResetDash();
        wasGrounded = isGrounded;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (Input.GetKeyDown(KeyCode.F))
        {
            FlipAttackPoint(facingDirection);
            meleeAttack?.TryAttack();
        }

        if (isGrounded && !IsNearHazard())
            lastGroundedPosition = transform.position;
    }

    private bool IsNearHazard()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.6f);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Damage"))
                return true;
        }
        return false;
    }

    void FlipAttackPoint(float direction)
    {
        if (meleeAttack == null || meleeAttack.attackPoint == null) return;

        Vector3 localPos = meleeAttack.attackPoint.localPosition;
        localPos.x = Mathf.Abs(localPos.x) * direction;
        meleeAttack.attackPoint.localPosition = localPos;

        if (meleeAttack.attackVisual != null)
        {
            SpriteRenderer slashSr = meleeAttack.attackVisual.GetComponent<SpriteRenderer>();
            if (slashSr != null)
                slashSr.flipX = direction < 0;
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("Damage"))
        {
            TakeDamage(25);
            if (health > 0)
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
        if (other == null || isDead) return;

        // Bosses handle their own contact damage — skip them here
        if (other.GetComponent<BossBase>() != null) return;

        cog_behavior cog = other.GetComponent<cog_behavior>();
        if (cog != null)
        {
            TakeDamage(cog.damage);
            if (health > 0)
            {
                Vector2 knockDir = (transform.position - other.transform.position).normalized;
                rb.linearVelocity = new Vector2(knockDir.x * enemyKnockbackForce, jumpForce);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible || isDead) return;

        health -= amount;
        health = Mathf.Max(health, 0);

        Debug.Log($"Player took {amount} damage. HP: {health}/{maxHealth}");

        AudioManager.Instance?.Play(AudioManager.Instance.playerHit);
        StartCoroutine(InvincibilityFrames());

        if (health <= 0)
            Die();
    }

    public void FallDeath(int damage)
    {
        if (isInvincible || isDead) return;

        health -= damage;
        health = Mathf.Max(health, 0);

        transform.position = lastGroundedPosition;
        rb.linearVelocity = Vector2.zero;

        AudioManager.Instance?.Play(AudioManager.Instance.playerHit);
        StartCoroutine(InvincibilityFrames());

        if (health <= 0)
            Die();
    }

    public void UpdateCheckpoint(Vector3 position)
    {
        lastGroundedPosition = position;
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
        isDead = true;

        StopAllCoroutines();
        sp.color = Color.white;
        isInvincible = false;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = baseGravityScale;

        moveSpeed = baseMoveSpeed;
        jumpForce = baseJumpForce;
        maxHealth = baseMaxHealth;
        health = baseMaxHealth;

        if (meleeAttack != null)
            meleeAttack.damage = baseMeleeAttackDamage;

        if (respawnPoint != null)
            transform.position = respawnPoint.position;
        else
            Debug.LogWarning("No respawn point assigned on Player!");

        ResetAbilities();

        if (spawnManager != null)
            spawnManager.ResetAll();
        if (meleeSpawnManager != null)
            meleeSpawnManager.ResetAll();
        if (itemManager != null)
            itemManager.ResetAll();

        BossBase activeBoss = FindAnyObjectByType<BossBase>();
        if (activeBoss != null)
            Destroy(activeBoss.gameObject);
        if (bossSpawner != null)
            bossSpawner.ResetSpawner();
        if (EnemyKillTracker.Instance != null)
            EnemyKillTracker.Instance.Reset();

        NPCCode[] npcs = FindObjectsByType<NPCCode>(FindObjectsSortMode.None);
        foreach (NPCCode npc in npcs)
            npc.ResetDialogue();

        isDead = false;
        StartCoroutine(InvincibilityFrames());

        Debug.Log("Player died — all entities reset.");
    }

    private void ResetAbilities()
    {
        SawbladeLauncher launcher = GetComponent<SawbladeLauncher>();
        if (launcher != null)
            launcher.enabled = false;
        if (sawbladeVisual != null)
            sawbladeVisual.SetActive(false);

        if (airDash != null)
            airDash.enabled = false;

        if (activeBushBum != null)
        {
            Destroy(activeBushBum);
            activeBushBum = null;
        }

        LaserImplant laser = GetComponent<LaserImplant>();
        if (laser != null)
            laser.enabled = false;
        if (laserImplantVisual != null)
            laserImplantVisual.SetActive(false);

        DruidHeart druid = GetComponent<DruidHeart>();
        if (druid != null)
            druid.enabled = false;
        if (druidHeartVisual != null)
            druidHeartVisual.SetActive(false);

        if (mushVisual != null)
            mushVisual.SetActive(false);

        rb.gravityScale = baseGravityScale;
    }
}