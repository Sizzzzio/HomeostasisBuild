using UnityEngine;

public class ItemDescriptionTrigger : MonoBehaviour
{
    private ItemPickup parentPickup;

    void Start()
    {
        parentPickup = GetComponentInParent<ItemPickup>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (parentPickup == null) return;

        ItemDescriptionUI.Instance?.Show(parentPickup.itemName, parentPickup.itemDescription);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        ItemDescriptionUI.Instance?.Hide();
    }
}
