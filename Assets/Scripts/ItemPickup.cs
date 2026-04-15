using UnityEngine;

public enum ItemType
{
    SawbladeLauncher,
    AirDash,
}

public class ItemPickup : MonoBehaviour
{
    [Header("Item Settings")]
    public ItemType itemType;

    [Header("Visuals")]
    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;

    private Vector3 startPosition;
    private bool initialized = false;   // Prevent Start() resetting position on re-enable

    void Start()
    {
        if (!initialized)
        {
            startPosition = transform.position;
            initialized = true;
        }
    }

    void OnEnable()
    {
        // When re-enabled by ItemManager, restore original position
        if (initialized)
            transform.position = startPosition;
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        ApplyItemToPlayer(other.gameObject);
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
                else
                    Debug.LogWarning("SawbladeLauncher component not found on Player!");
                break;

            case ItemType.AirDash:
                AirDash airDash = player.GetComponent<AirDash>();
                if (airDash != null)
                {
                    airDash.enabled = true;
                    Debug.Log("Picked up: Air Dash");
                }
                else
                    Debug.LogWarning("AirDash component not found on Player!");
                break;
        }
    }
}