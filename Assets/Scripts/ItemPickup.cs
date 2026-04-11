using UnityEngine;

public enum ItemType
{
    SawbladeLauncher,
    // Add more item types here as you build them
    // Shield,
    // SpeedBoost,
    // DoubleJump,
}

public class ItemPickup : MonoBehaviour
{
    [Header("Item Settings")]
    public ItemType itemType;

    [Header("Visuals")]
    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        ApplyItemToPlayer(other.gameObject);

        // Disable instead of destroy so ItemManager can respawn it on player death
        gameObject.SetActive(false);
    }

    void ApplyItemToPlayer(GameObject player)
    {
        switch (itemType)
        {
            case ItemType.SawbladeLauncher:
                SawbladeLauncher launcher = player.GetComponent<SawbladeLauncher>();
                if (launcher != null)
                {
                    launcher.enabled = true;

                    Transform sawbladeVisual = player.transform.Find("SawbladeVisual");
                    if (sawbladeVisual != null)
                        sawbladeVisual.gameObject.SetActive(true);

                    Debug.Log("Picked up: Sawblade Launcher");
                }
                break;

            // Add more cases here as you add items
            // case ItemType.Shield:
            //     player.GetComponent<Shield>().enabled = true;
            //     break;
        }
    }
}
