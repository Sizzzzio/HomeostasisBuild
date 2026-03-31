using System.Collections;
using UnityEngine;

// Unity Script Reference
public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int health = 100;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;

    private SpriteRenderer sp;

    private Inventory inventory;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();

        inventory = GetComponent<Inventory>();
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
            UseSelectedItem();
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Damage")
        {
            health -= 25;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            StartCoroutine(BlinkBlack());

            if(health <= 0)
            {
                Die();
            }
        }
    }

    private IEnumerator BlinkBlack()
    {
        sp.color = Color.black;
        yield return new WaitForSeconds(0.1f);
        sp.color = Color.white;
    }

    private void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    void UseSelectedItem()
    {
        if (inventory == null) return;

        if (inventory.selectedHotbarIndex < inventory.slots.Count)
        {
            var slot = inventory.slots[inventory.selectedHotbarIndex];

            Debug.Log("Using: " + slot.item.itemName);

            // Example: consume item
            slot.quantity--;

            if (slot.quantity <= 0)
            {
                inventory.slots.RemoveAt(inventory.selectedHotbarIndex);
            }
        }
    }
}