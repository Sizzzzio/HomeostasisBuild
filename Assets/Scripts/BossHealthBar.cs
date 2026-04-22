using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    public GameObject healthBarPanel;
    public Image healthFill;
    public TMP_Text bossNameText;
    public string bossName = "Boss";

    void Start()
    {
        if (healthBarPanel != null)
            healthBarPanel.SetActive(false);
    }

    public void SetBoss(BossBase boss)
    {
        Debug.Log($"SetBoss — healthBarPanel null: {healthBarPanel == null}");

        if (healthBarPanel != null)
        {
            healthBarPanel.SetActive(true);
            Debug.Log($"healthBarPanel active: {healthBarPanel.activeSelf}");
        }

        if (bossNameText != null)
            bossNameText.text = bossName;

        boss.onHealthChanged += UpdateBar;
        UpdateBar(boss.currentHealth, boss.maxHealth);
    }

    void UpdateBar(int current, int max)
    {
        if (healthFill != null)
            healthFill.fillAmount = max > 0 ? (float)current / max : 0f;

        if (current <= 0 && healthBarPanel != null)
            healthBarPanel.SetActive(false);
    }
}