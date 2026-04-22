using System.Collections;
using UnityEngine;

public class TurretBoss : BossBase
{
    [Header("Laser Settings")]
    public GameObject laserBeamPrefab;
    public float fireInterval = 4f;
    public float laserDuration = 0.5f;
    public int laserDamage = 25;
    public float laserRange = 20f;
    public int splitDamage = 10;

    private Transform player;
    private int ignoreEnemyLayer;

    protected override void Start()
    {
        base.Start();

        Player p = FindAnyObjectByType<Player>();
        if (p != null)
            player = p.transform;

        Debug.Log($"TurretBoss Start — player found: {player != null}, laserBeamPrefab: {laserBeamPrefab != null}");
        // Ignore Enemy layer so laser doesn't hit itself
        ignoreEnemyLayer = ~LayerMask.GetMask("Enemy");
        StartCoroutine(LaserAttackLoop());
    }

    IEnumerator LaserAttackLoop()
    {
        Debug.Log("LaserAttackLoop started");
        yield return new WaitForSeconds(2f);
        while (true)
        {
            Debug.Log("Firing laser...");
            yield return new WaitForSeconds(fireInterval);
            if (player != null)
                StartCoroutine(FireLaser());
        }
    }

    IEnumerator FireLaser()
    {
        if (player == null) yield break;

        // Telegraph — lock on to player position and show warning beam
        Vector2 lockedPosition = player.position;
        Vector2 direction = (lockedPosition - (Vector2)transform.position).normalized;
        Vector2 origin = (Vector2)transform.position + direction * 1.5f;

        // Show a faint warning beam before firing
        if (laserBeamPrefab != null)
        {
            GameObject warningBeam = Instantiate(laserBeamPrefab, origin + direction * (laserRange / 2f), Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
            warningBeam.transform.localScale = new Vector3(laserRange, warningBeam.transform.localScale.y, 1f);
            SpriteRenderer warnSr = warningBeam.GetComponent<SpriteRenderer>();
            if (warnSr != null) warnSr.color = new Color(1f, 0.3f, 0.3f, 0.3f); // Faint red
            Destroy(warningBeam, 0.8f);
        }

        // Wait before firing — player has time to dodge
        yield return new WaitForSeconds(0.8f);

        // Fire at locked position not current player position
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, laserRange, ignoreEnemyLayer);

        Debug.Log($"Laser fired — hit: {(hit.collider != null ? hit.collider.name : "nothing")}");

        if (laserBeamPrefab != null)
        {
            float hitDist = hit.collider != null ? hit.distance : laserRange;
            Vector3 midpoint = origin + direction * (hitDist / 2f);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            GameObject beam = Instantiate(laserBeamPrefab, midpoint, Quaternion.Euler(0f, 0f, angle));
            beam.transform.localScale = new Vector3(hitDist, beam.transform.localScale.y, 1f);
            Destroy(beam, laserDuration);
        }
        else
        {
            Debug.LogError("laserBeamPrefab is not assigned!");
        }

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            Player p = hit.collider.GetComponent<Player>();
            if (p != null) p.TakeDamage(laserDamage);
        }
        else if (hit.collider != null)
        {
            FireSplitLasers(hit.point, direction);
        }

        yield return null;
    }

    void FireSplitLasers(Vector2 origin, Vector2 incomingDir)
    {
        float[] angles = { 0f, 30f, -30f };

        foreach (float angle in angles)
        {
            Vector2 splitDir = Quaternion.Euler(0f, 0f, angle) * incomingDir;
            RaycastHit2D splitHit = Physics2D.Raycast(origin, splitDir, laserRange, ignoreEnemyLayer);

            if (laserBeamPrefab != null)
            {
                float dist = splitHit.collider != null ? splitHit.distance : laserRange;
                Vector3 midpoint = (Vector3)origin + (Vector3)splitDir * (dist / 2f);
                float beamAngle = Mathf.Atan2(splitDir.y, splitDir.x) * Mathf.Rad2Deg;

                GameObject beam = Instantiate(laserBeamPrefab, midpoint, Quaternion.Euler(0f, 0f, beamAngle));
                beam.transform.localScale = new Vector3(dist, beam.transform.localScale.y, 1f);
                Destroy(beam, laserDuration);
            }

            if (splitHit.collider != null && splitHit.collider.CompareTag("Player"))
            {
                Player p = splitHit.collider.GetComponent<Player>();
                if (p != null) p.TakeDamage(splitDamage);
            }
        }
    }
}