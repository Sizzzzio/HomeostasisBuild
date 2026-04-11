using System.Collections;
using UnityEngine;

public class SawbladeLauncher : MonoBehaviour
{
    [Header("Sawblade Settings")]
    public GameObject sawbladePrefab;
    public float launchSpeed = 10f;
    public float returnSpeed = 12f;
    public int damage = 50;
    public int maxHits = 3;
    public float cooldown = 1f;

    private GameObject activeSawblade;
    private bool isOnCooldown = false;
    private Player player;

    void Start()
    {
        player = GetComponent<Player>();
        Debug.Log($"SawbladeLauncher Start — enabled: {enabled}, prefab: {sawbladePrefab}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log($"Q pressed — enabled: {enabled}, onCooldown: {isOnCooldown}, activeSawblade: {activeSawblade != null}");

            if (!isOnCooldown && activeSawblade == null)
                Launch();
        }
    }

    void Launch()
    {
        if (sawbladePrefab == null)
        {
            Debug.LogError("SawbladeLauncher: sawbladePrefab is null!");
            return;
        }

        float direction = transform.localScale.x > 0 ? 1f : -1f;
        Debug.Log($"Launching sawblade in direction: {direction}");

        activeSawblade = Instantiate(sawbladePrefab, transform.position, Quaternion.identity);

        SawbladeProjectile projectile = activeSawblade.GetComponent<SawbladeProjectile>();
        if (projectile != null)
            projectile.Init(direction, launchSpeed, returnSpeed, damage, maxHits, transform, OnSawbladeReturned);
        else
            Debug.LogError("SawbladeLauncher: SawbladeProjectile component missing from prefab!");
    }

    void OnSawbladeReturned()
    {
        activeSawblade = null;
        StartCoroutine(CooldownTimer());
    }

    IEnumerator CooldownTimer()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }
}
