using UnityEngine;

public class ItemManager : MonoBehaviour
{
    void Start()
    {
        // Each child of ItemManager is an item spawn point
        // The child must have an ItemPickup component on it
        foreach (Transform child in transform)
        {
            ItemPickup pickup = child.GetComponent<ItemPickup>();
            if (pickup == null)
                Debug.LogWarning($"ItemManager: {child.name} has no ItemPickup component!");
        }
    }

    // Called by Player.cs on death — respawns all pickups that were collected
    public void ResetAll()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
