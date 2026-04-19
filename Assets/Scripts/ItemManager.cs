using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    // Store references at start so we don't lose them when objects deactivate
    private List<GameObject> registeredPickups = new List<GameObject>();

    void Start()
    {
        foreach (Transform child in transform)
        {
            registeredPickups.Add(child.gameObject);
            ItemPickup pickup = child.GetComponent<ItemPickup>();
            if (pickup == null)
                Debug.LogWarning($"ItemManager: {child.name} has no ItemPickup component!");
            else
                Debug.Log($"ItemManager: registered {child.name}");
        }
    }

    public void ResetAll()
    {
        Debug.Log($"ItemManager.ResetAll called — registered count: {registeredPickups.Count}");
        foreach (GameObject pickup in registeredPickups)
        {
            if (pickup != null)
            {
                Debug.Log($"ItemManager: re-enabling {pickup.name}");
                pickup.SetActive(true);
            }
        }
    }
}