using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject obstaclePrefab;     // Obstacle prefab (needs BoxCollider)
    public Transform player;              // Player transform
    public BoxCollider roadCollider;      // Drag FLOOR BoxCollider here

    [Header("Lanes (3 positions)")]
    public float[] lanesX = new float[] { -2.0f, 0f, 2.0f }; // safe for road width ~8

    [Header("Spawn Settings")]
    public float spawnDistanceAhead = 30f;
    public float spawnY = 1.3f;           // make obstacles a little higher

    [Header("Run Duration (difficulty ramp)")]
    public float runDuration = 60f;

    [Header("1) Frequency (seconds between spawns)")]
    public float intervalEarly = 0.9f;
    public float intervalLate = 0.45f;

    [Header("2) Z Gap (min distance between obstacles)")]
    public float gapEarly = 20f;
    public float gapLate = 10f;

    [Header("3) Size (X width grows with time)")]
    public float sizeEarly = 0.8f;        // smaller than prefab at start
    public float sizeLate = 2.2f;         // safe for floor scale X=8
    public float sizeStartAfter = 8f;     // keep small for first seconds

    [Header("Materials (cycle one-by-one)")]
    public Material[] obstacleMaterials;  // put your 3 materials here

    [Header("Cleanup (performance)")]
    public bool destroyBehindPlayer = true;
    public float destroyDistanceBehind = 18f;

    private float elapsed;
    private float timer;
    private float lastSpawnZ = -99999f;
    private int lastLane = -999;

    // material cycle: 0 -> 1 -> 2 -> 0 ...
    private int materialIndex = 0;

    void Start()
    {
        // spawn early
        timer = intervalEarly;
    }

    void Update()
    {
        if (obstaclePrefab == null || player == null || roadCollider == null) return;

        elapsed += Time.deltaTime;
        timer += Time.deltaTime;

        // difficulty factor 0..1
        float t = Mathf.Clamp01(elapsed / runDuration);

        // frequency & gap ramp
        float currentInterval = Mathf.Lerp(intervalEarly, intervalLate, t);
        float currentGap = Mathf.Lerp(gapEarly, gapLate, t);

        float spawnZ = player.position.z + spawnDistanceAhead;

        bool timeOk = timer >= currentInterval;
        bool gapOk = (spawnZ - lastSpawnZ) >= currentGap;

        if (timeOk && gapOk)
        {
            timer = 0f;

            int lane = PickLaneDifferentFromLast();
            lastLane = lane;

            Vector3 pos = new Vector3(lanesX[lane], spawnY, spawnZ);
            GameObject obs = Instantiate(obstaclePrefab, pos, Quaternion.identity);

            // Make sure tag exists and is assigned to prefab too (safe)
            obs.tag = "Obstacle";

            // Size ramp (start small -> grow)
            float sizeT = Mathf.Clamp01((elapsed - sizeStartAfter) / Mathf.Max(0.01f, (runDuration - sizeStartAfter)));
            float sizeX = Mathf.Lerp(sizeEarly, sizeLate, sizeT);

            // Keep Y/Z stable for fairness
            obs.transform.localScale = new Vector3(sizeX, 2f, 2f);

            // Clamp after scaling so it NEVER goes out of the road
            ClampObstacleInsideRoad(obs);

            // Cycle materials one-by-one
            ApplyMaterialCycle(obs);

            // Optional cleanup behind the player
            if (destroyBehindPlayer)
            {
                DestroyBehind db = obs.AddComponent<DestroyBehind>();
                db.Init(player, destroyDistanceBehind);
            }

            lastSpawnZ = spawnZ;
        }
    }

    int PickLaneDifferentFromLast()
    {
        int lane = Random.Range(0, lanesX.Length);

        if (lane == lastLane)
            lane = (lane + Random.Range(1, lanesX.Length)) % lanesX.Length;

        return lane;
    }

    void ClampObstacleInsideRoad(GameObject obstacle)
    {
        BoxCollider obsCol = obstacle.GetComponent<BoxCollider>();
        if (obsCol == null) return;

        float roadMinX = roadCollider.bounds.min.x;
        float roadMaxX = roadCollider.bounds.max.x;

        float obsHalfX = obsCol.bounds.extents.x; // real half width (scaled)

        Vector3 p = obstacle.transform.position;

        // If obstacle too wide, force it centered
        float roadWidth = roadMaxX - roadMinX;
        if (obsHalfX * 2f > roadWidth)
        {
            p.x = (roadMinX + roadMaxX) * 0.5f;
            obstacle.transform.position = p;
            return;
        }

        p.x = Mathf.Clamp(p.x, roadMinX + obsHalfX, roadMaxX - obsHalfX);
        obstacle.transform.position = p;
    }

    void ApplyMaterialCycle(GameObject obs)
    {
        Renderer r = obs.GetComponent<Renderer>();
        if (r == null || obstacleMaterials == null || obstacleMaterials.Length == 0) return;

        r.material = obstacleMaterials[materialIndex];

        materialIndex++;
        if (materialIndex >= obstacleMaterials.Length)
            materialIndex = 0;
    }
}

// Destroys obstacles that go behind the player (keeps scene clean)
public class DestroyBehind : MonoBehaviour
{
    private Transform player;
    private float behindDistance;

    public void Init(Transform playerTransform, float distanceBehind)
    {
        player = playerTransform;
        behindDistance = distanceBehind;
    }

    void Update()
    {
        if (player == null) return;

        if (transform.position.z < player.position.z - behindDistance)
            Destroy(gameObject);
    }
}
