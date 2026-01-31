using UnityEngine;

public class LifePowerUpPickup : MonoBehaviour
{
    public int costCoins = 100;

    private bool consumed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (consumed) return;

        // Reliable player detection (works even if tags are wrong)
        if (other.attachedRigidbody == null) return;

        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm == null) return;

        if (!gm.CanGainLife())
        {
            consumed = true;
            SoundManager.instance?.StopPowerUpWarning();
            Destroy(gameObject);
            return;
        }

        if (gm.TrySpendCoins(costCoins))
        {
            consumed = true;
            gm.GainLife(1);

            SoundManager.instance?.StopPowerUpWarning();
            SoundManager.instance?.PlayPowerUpCollect();

            Destroy(gameObject);
        }
        // else: not enough coins -> do nothing (it stays)
    }

    private void OnDestroy()
    {
        // If it disappears (timeout/missed), warning must stop.
        // Safe even if already stopped.
        SoundManager.instance?.StopPowerUpWarning();
    }
}
