using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public Transform player;

    public float[] lanesX = new float[] { -2.0f, 0f, 2.0f };
    public float spawnDistanceAhead = 30f;
    public float spawnY = 1.2f;

    public float interval = 1.0f;     // coin spawn rate
    public float minGapZ = 8f;        // distance between coins

    private float timer;
    private float lastSpawnZ = -99999f;

    void Start()
    {
        timer = interval; // spawn early
    }

    void Update()
    {
        if (coinPrefab == null || player == null) return;

        timer += Time.deltaTime;

        float spawnZ = player.position.z + spawnDistanceAhead;

        bool timeOk = timer >= interval;
        bool gapOk = (spawnZ - lastSpawnZ) >= minGapZ;

        if (timeOk && gapOk)
        {
            timer = 0f;

            int lane = Random.Range(0, lanesX.Length);
            Vector3 pos = new Vector3(lanesX[lane], spawnY, spawnZ);

            Instantiate(coinPrefab, pos, Quaternion.identity);
            lastSpawnZ = spawnZ;
        }
    }
}
