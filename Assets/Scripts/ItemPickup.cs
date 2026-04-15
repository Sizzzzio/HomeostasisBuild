using UnityEngine;

public enum ItemType
{
    SawbladeLauncher,
    AirDash,
    MeatyMush,
}

public class ItemPickup : MonoBehaviour
{
    [Header("Item Settings")]
    public ItemType itemType;

    [Header("Description")]
    public string itemName = "Item";
    [TextArea(2, 4)]
    public string itemDescription = "A mysterious item.";

    [Header("Meaty Mush Stats")]
    public float speedBoost = 1f;
    public int healthBoost = 25;
    public int attackBoost = 10;

    [Header("Visuals")]
    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;

    private Vector3 startPosition;
    private bool initialized = false;

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
        if (initialized)
            transform.position = startPosition;
    }

    void OnDisable()
    {
        ItemDescriptionUI.Instance?.Hide();
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
                break;

            case ItemType.AirDash:
                AirDash airDash = player.GetComponent<AirDash>();
                if (airDash != null)
                {
                    airDash.enabled = true;
                    Debug.Log("Picked up: Air Dash");
                }
                break;

            case ItemType.MeatyMush:
                Player playerScript = player.GetComponent<Player>();
                MeleeAttack meleeAttack = player.GetComponent<MeleeAttack>();

                if (playerScript != null)
                {
                    playerScript.moveSpeed += speedBoost;
                    playerScript.maxHealth += healthBoost;
                    playerScript.health = Mathf.Min(playerScript.health + healthBoost, playerScript.maxHealth);
                    Debug.Log($"Meaty Mush: speed +{speedBoost}, health +{healthBoost}");
                }

                if (meleeAttack != null)
                {
                    meleeAttack.damage += attackBoost;
                    Debug.Log($"Meaty Mush: attack +{attackBoost}");
                }
                break;
        }
    }
}