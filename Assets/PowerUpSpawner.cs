using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject powerUpPrefab;
    public Transform player;
    public BoxCollider roadCollider;

    [Header("Lanes (3 positions)")]
    public float[] lanesX = new float[] { -3f, 0f, 3f };

    [Header("Spawn")]
    public float spawnDistanceAhead = 55f;   // <-- FARTHER (time to react)
    public float spawnY = 1.2f;
    public float spawnEverySeconds = 20f;

    [Header("Power-up Lifetime")]
    public float powerUpLifetime = 10f;      // disappears if not collected

    private float timer = 0f;
    private GameObject currentPowerUp;

    void Update()
    {
        if (powerUpPrefab == null || player == null || roadCollider == null) return;

        // If a power-up exists but got destroyed, clear reference
        if (currentPowerUp == null) { /* ok */ }

        timer += Time.deltaTime;
        if (timer < spawnEverySeconds) return;

        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm == null) return;

        // Only spawn if lives < max
        if (!gm.CanGainLife()) return;

        // Only spawn one at a time
        if (currentPowerUp != null) return;

        timer = 0f;

        int lane = Random.Range(0, lanesX.Length);
        float z = player.position.z + spawnDistanceAhead;

        Vector3 pos = new Vector3(lanesX[lane], spawnY, z);
        currentPowerUp = Instantiate(powerUpPrefab, pos, Quaternion.identity);

        ClampInsideRoad(currentPowerUp);

        // Start warning loop
        SoundManager.instance?.PlayPowerUpWarning();

        // Auto-destroy if not collected (and stop warning)
        Destroy(currentPowerUp, powerUpLifetime);
        Invoke(nameof(StopWarningIfNoPowerUp), powerUpLifetime + 0.05f);
    }

    void StopWarningIfNoPowerUp()
    {
        // If it timed out (or got destroyed) and no other power-up exists, stop warning
        if (currentPowerUp == null)
            SoundManager.instance?.StopPowerUpWarning();
    }

    void ClampInsideRoad(GameObject obj)
    {
        Collider col = obj.GetComponent<Collider>();
        if (col == null) return;

        float minX = roadCollider.bounds.min.x;
        float maxX = roadCollider.bounds.max.x;
        float halfX = col.bounds.extents.x;

        Vector3 p = obj.transform.position;
        p.x = Mathf.Clamp(p.x, minX + halfX, maxX - halfX);
        obj.transform.position = p;
    }
}
