using UnityEngine;

public class ItemManager : MonoBehaviour
{
    void Start()
    {
        foreach (Transform child in transform)
        {
            ItemPickup pickup = child.GetComponent<ItemPickup>();
            if (pickup == null)
                Debug.LogWarning($"ItemManager: {child.name} has no ItemPickup component!");
            else
                Debug.Log($"ItemManager: registered {child.name}");
        }
    }

    public void ResetAll()
    {
        Debug.Log($"ItemManager.ResetAll called — child count: {transform.childCount}");
        foreach (Transform child in transform)
        {
            Debug.Log($"ItemManager: re-enabling {child.name}");
            child.gameObject.SetActive(true);
        }
    }
}