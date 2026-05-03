using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 30;
    public float flashDuration = 0.15f;     // How long each flash lasts
    public int flashCount = 3;              // How many times it flashes
    public Color flashColor = Color.red;    // Flash color

    private int currentHealth;
    private SpriteRenderer sr;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. HP: {currentHealth}/{maxHealth}");

        AudioManager.Instance?.Play(AudioManager.Instance.enemyHit);  // ← add this
        if (sr != null)
            StartCoroutine(HitFlash());

        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator HitFlash()
    {
        for (int i = 0; i < flashCount; i++)
        {
            sr.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            sr.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }

    void Die()
    {
        if (EnemyKillTracker.Instance != null)
            EnemyKillTracker.Instance.RegisterKill();
        Destroy(gameObject);
    }
}