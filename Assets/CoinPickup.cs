using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int points = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null)
                gm.AddCoin(10);

            SoundManager.instance?.PlayCoin();

            Destroy(gameObject);
        }
    }
}
