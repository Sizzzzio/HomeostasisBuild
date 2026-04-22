using System.Collections;
using UnityEngine;

public class BossBase : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 200;
    public int currentHealth;

    protected SpriteRenderer sr;
    protected Color originalColor;

    public System.Action<int, int> onHealthChanged;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;

        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        currentHealth = Mathf.Max(currentHealth, 0);

        onHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"{gameObject.name} took {dmg} damage. HP: {currentHealth}/{maxHealth}");

        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} defeated!");
        Destroy(gameObject);
    }

    // Boss acts as a solid damageable barrier — no contact damage

    private IEnumerator HitFlash()
    {
        if (sr == null) yield break;
        for (int i = 0; i < 3; i++)
        {
            sr.color = Color.white;
            yield return new WaitForSeconds(0.05f);
            sr.color = originalColor;
            yield return new WaitForSeconds(0.05f);
        }
    }
}