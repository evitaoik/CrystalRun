using UnityEngine;

public class PlayerHitObstacle : MonoBehaviour
{
    private GameManager gm;
    private float lastHitTime = -999f;
    public float hitCooldown = 1.0f;

    void Start()
    {
        gm = FindFirstObjectByType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Obstacle")) return;

        // Prevent multiple hits instantly
        if (Time.time - lastHitTime < hitCooldown) return;
        lastHitTime = Time.time;

        // Lose 1 life
        gm?.TakeHit(1);

        SoundManager.instance?.PlayHit();

        // Remove the obstacle you hit
        Destroy(other.gameObject);
    }
}
