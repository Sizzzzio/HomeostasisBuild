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
            if (i < inventory.slots.Count)
            {
                slotImages[i].sprite = inventory.slots[i].item.icon;
                slotImages[i].enabled = true;
            }
            else
            {
                slotImages[i].enabled = false;
            }

            // Highlight selected slot
            selectionHighlights[i].enabled = (i == inventory.selectedHotbarIndex);
        }
    }
}