using System.Collections;
using UnityEngine;

public class AirDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 18f;           // How fast the dash is
    public float dashDuration = 0.15f;      // How long the dash lasts
    public float dashCooldown = 0.3f;       // Minimum time between dashes

    private bool canDash = false;           // Has a dash available
    private bool isDashing = false;
    private float lastDashTime = -Mathf.Infinity;

    private Rigidbody2D rb;
    private Player player;
    private SpriteRenderer sr;
    private TrailRenderer trail;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        sr = GetComponent<SpriteRenderer>();
        trail = GetComponent<TrailRenderer>();

        if (trail != null)
            trail.emitting = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    // Called by Player.cs when grounded
    public void ResetDash()
    {
        canDash = true;
    }

    public bool IsDashing() => isDashing;

    private IEnumerator Dash()
    {
        if (Time.time - lastDashTime < dashCooldown) yield break;

        canDash = false;
        isDashing = true;
        lastDashTime = Time.time;

        // Get direction from current movement, fallback to facing direction
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 dashDir;
        if (horizontal == 0 && vertical == 0)
            dashDir = new Vector2(transform.localScale.x > 0 ? 1f : -1f, 0f);
        else
            dashDir = new Vector2(horizontal, vertical).normalized;

        // Disable gravity during dash
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = dashDir * dashSpeed;

        if (trail != null)
            trail.emitting = true;

        // Flash white during dash
        if (sr != null)
            sr.color = Color.cyan;

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.5f, rb.linearVelocity.y);
        isDashing = false;

        if (trail != null)
            trail.emitting = false;

        if (sr != null)
            sr.color = Color.white;
    }
}
