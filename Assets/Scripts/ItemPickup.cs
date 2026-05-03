using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum ItemType
{
    SawbladeLauncher,
    AirDash,
    MeatyMush,
    BushBum,
    LaserImplant,
    DruidHeart,
}

public class ItemPickup : MonoBehaviour
{
    [Header("Item Settings")]
    public ItemType itemType;

    [Header("Description UI — assign same panel for all items")]
    public GameObject descriptionPanel;
    public TMP_Text itemNameText;
    public TMP_Text itemDescText;

    [Header("This Item's Data")]
    public string itemName = "Item";
    [TextArea(2, 4)]
    public string itemDescription = "A mysterious item.";

    [Header("Meaty Mush Stats")]
    public float speedBoost = 1f;
    public int healthBoost = 25;
    public int attackBoost = 10;

    [Header("Bush Bum Settings")]
    public GameObject bushBumPrefab;

    [Header("Player Visual References")]
    public GameObject itemVisual;

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
        HideDescription();
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        float dist = Vector2.Distance(transform.position, other.transform.position);

        if (dist <= 0.8f)
        {
            // Close enough — pick up
            HideDescription();
            AudioManager.Instance?.Play(AudioManager.Instance?.itemPickup);
            ApplyItemToPlayer(other.gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            // Outer range — show description
            ShowDescription();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        HideDescription();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        float dist = Vector2.Distance(transform.position, other.transform.position);
        if (dist <= 0.8f)
        {
            HideDescription();
            AudioManager.Instance?.Play(AudioManager.Instance?.itemPickup);
            ApplyItemToPlayer(other.gameObject);
            gameObject.SetActive(false);
        }
    }

    void ShowDescription()
    {
        if (descriptionPanel != null) descriptionPanel.SetActive(true);
        if (itemNameText != null) itemNameText.text = itemName;
        if (itemDescText != null) itemDescText.text = itemDescription;
    }

    void HideDescription()
    {
        if (descriptionPanel != null) descriptionPanel.SetActive(false);
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
                    if (itemVisual != null) itemVisual.SetActive(true);
                    else { Transform v = player.transform.Find("SawbladeVisual"); if (v != null) v.gameObject.SetActive(true); }
                    Debug.Log("Picked up: Sawblade Launcher");
                }
                break;

            case ItemType.AirDash:
                AirDash airDash = player.GetComponent<AirDash>();
                if (airDash != null)
                {
                    airDash.enabled = true;
                    if (itemVisual != null) itemVisual.SetActive(true);
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
                    if (playerScript.mushVisual != null) playerScript.mushVisual.SetActive(true);
                }
                if (meleeAttack != null) meleeAttack.damage += attackBoost;
                break;

            case ItemType.BushBum:
                if (bushBumPrefab != null)
                {
                    GameObject bushBum = Instantiate(bushBumPrefab, player.transform.position + new Vector3(1f, 0f, 0f), Quaternion.identity);
                    Player p = player.GetComponent<Player>();
                    if (p != null) p.SetBushBum(bushBum);
                }
                break;

            case ItemType.LaserImplant:
                LaserImplant laser = player.GetComponent<LaserImplant>();
                if (laser != null)
                {
                    laser.enabled = true;
                    if (itemVisual != null) itemVisual.SetActive(true);
                }
                break;

            case ItemType.DruidHeart:
                DruidHeart druid = player.GetComponent<DruidHeart>();
                if (druid != null)
                {
                    druid.enabled = true;
                    if (itemVisual != null) itemVisual.SetActive(true);
                }
                break;
        }
    }
}