using UnityEngine;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour
{
    public Inventory inventory;
    public Image[] slotImages;
    public Image[] selectionHighlights;

    void Update()
    {
        for (int i = 0; i < slotImages.Length; i++)
{
    // Always keep slot visible
    slotImages[i].enabled = true;

    if (i < inventory.slots.Count)
    {
        slotImages[i].sprite = inventory.slots[i].item.icon;
    }
    else
    {
        slotImages[i].sprite = null; // empty slot
    }

    // Highlight selected slot
    selectionHighlights[i].enabled = (i == inventory.selectedHotbarIndex);
}
    }
}