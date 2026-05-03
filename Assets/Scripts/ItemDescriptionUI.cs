using UnityEngine;
using TMPro;

public class ItemDescriptionUI : MonoBehaviour
{
    public static ItemDescriptionUI Instance;

    [Header("UI References")]
    public GameObject descriptionBox;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;

    void Awake()
    {
        Instance = this;

        // Auto-find if not assigned
        if (descriptionBox == null)
            descriptionBox = GameObject.Find("DescriptionBox");
        if (itemNameText == null)
        {
            var obj = GameObject.Find("ItemNameText");
            if (obj != null) itemNameText = obj.GetComponent<TMP_Text>();
        }
        if (itemDescriptionText == null)
        {
            var obj = GameObject.Find("ItemDescText");
            if (obj != null) itemDescriptionText = obj.GetComponent<TMP_Text>();
        }

        Debug.Log($"ItemDescriptionUI Awake — descriptionBox: {descriptionBox != null}, nameText: {itemNameText != null}, descText: {itemDescriptionText != null}");
    }

    void Start()
    {
        Hide();
    }

    public void Show(string itemName, string description)
    {
        if (descriptionBox == null)
        {
            Debug.LogError("ItemDescriptionUI: descriptionBox is null!");
            return;
        }

        descriptionBox.SetActive(true);

        if (itemNameText != null)
            itemNameText.text = itemName;
        if (itemDescriptionText != null)
            itemDescriptionText.text = description;
    }

    public void Hide()
    {
        if (descriptionBox != null)
            descriptionBox.SetActive(false);
    }
}