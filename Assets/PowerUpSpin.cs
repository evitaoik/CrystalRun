using UnityEngine;

public class PowerUpSpin : MonoBehaviour
{
    public float speed = 500f;

    void Update()
    {
        transform.Rotate(speed * Time.deltaTime, 0f, 0f, Space.Self);
    }
}
