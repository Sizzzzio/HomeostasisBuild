using UnityEngine;

public class PlayerHotbar : MonoBehaviour
{
    public Inventory inventory;

    void Update()
    {
        for (int i = 0; i < inventory.hotbarSize; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                inventory.selectedHotbarIndex = i;
            }
        }

        // Scroll wheel support
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            inventory.selectedHotbarIndex--;
        }
        else if (scroll < 0f)
        {
            inventory.selectedHotbarIndex++;
        }

        // Wrap around
        if (inventory.selectedHotbarIndex < 0)
            inventory.selectedHotbarIndex = inventory.hotbarSize - 1;

        if (inventory.selectedHotbarIndex >= inventory.hotbarSize)
            inventory.selectedHotbarIndex = 0;
    }
}