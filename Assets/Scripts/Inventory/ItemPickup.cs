using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData item;

    private bool playerInRange = false;
    private Inventory playerInventory;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            playerInventory.AddItem(item);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerInventory = other.GetComponent<Inventory>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerInventory = null;
        }
    }
}