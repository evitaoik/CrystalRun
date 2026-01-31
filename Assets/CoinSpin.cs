using UnityEngine;

public class CoinSpin : MonoBehaviour
{
    public float speed = 360f;

    void Update()
    {
        transform.Rotate(speed * Time.deltaTime, 0f, 0f, Space.Self);
    }
}
