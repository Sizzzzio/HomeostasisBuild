using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> slots = new List<InventorySlot>();

    public int maxSlots = 10;

    // HOTBAR SETTINGS
    public int hotbarSize = 5;
    public int selectedHotbarIndex = 0;

    public void AddItem(ItemData item)
    {
        if (item.isStackable)
        {
            foreach (var slot in slots)
            {
                if (slot.item == item)
                {
                    slot.Add(1);
                    return;
                }
            }
        }

        if (slots.Count < maxSlots)
        {
            InventorySlot newSlot = new InventorySlot();
            newSlot.item = item;
            newSlot.quantity = 1;
            slots.Add(newSlot);
        }
        else
        {
            Debug.Log("Inventory Full");
        }
    }
}