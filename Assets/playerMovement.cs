using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;

    [Header("Forward Speed")]
    public float baseForwardSpeed = 10f;
    public float speedIncreaseStep = 2f;   // how much speed increases
    public float speedIncreaseInterval = 15f; // every X seconds

    [Header("Side Movement")]
    public float sideSpeed = 6f;

    private float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        timer = 0f;
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        // How many intervals have passed
        int level = Mathf.FloorToInt(timer / speedIncreaseInterval);

        // Calculate current forward speed
        float currentForwardSpeed = baseForwardSpeed + level * speedIncreaseStep;

        // Left / Right input
        float h = Input.GetAxis("Horizontal");

        rb.linearVelocity = new Vector3(
            h * sideSpeed,
            rb.linearVelocity.y,
            currentForwardSpeed
        );
    }
}
