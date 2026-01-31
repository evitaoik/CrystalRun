using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources")]
    [Tooltip("Looping background music source")]
    public AudioSource musicSource;

    [Tooltip("One-shot SFX source (coin/hit/win/lose/collect)")]
    public AudioSource sfxSource;

    [Tooltip("Looping warning source for power-up appearing")]
    public AudioSource warningSource;

    [Header("Clips")]
    public AudioClip backgroundMusic;

    public AudioClip coinSound;
    public AudioClip hitSound;
    public AudioClip winSound;
    public AudioClip loseSound;

    public AudioClip powerUpWarning;
    public AudioClip powerUpCollect;

    void Awake()
    {
        // Singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // keeps audio alive on restart / scene reload
    }

    void Start()
    {
        // Auto-play background music if assigned
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;

            if (!musicSource.isPlaying)
                musicSource.Play();
        }
    }

    // ---------------- Basic SFX ----------------
    public void PlayCoin() => PlayOneShotSafe(coinSound);
    public void PlayHit() => PlayOneShotSafe(hitSound);
    public void PlayWin() => PlayOneShotSafe(winSound);
    public void PlayLose() => PlayOneShotSafe(loseSound);
    public void PlayPowerUpCollect() => PlayOneShotSafe(powerUpCollect);

    private void PlayOneShotSafe(AudioClip clip)
    {
        if (sfxSource == null) return;
        if (clip == null) return;

        sfxSource.PlayOneShot(clip);
    }

    // ---------------- Power-up Warning Loop ----------------
    public void PlayPowerUpWarning()
    {
        if (warningSource == null) return;
        if (powerUpWarning == null) return;

        // If already playing, don't restart
        if (warningSource.isPlaying && warningSource.clip == powerUpWarning)
            return;

        warningSource.Stop();
        warningSource.clip = powerUpWarning;
        warningSource.loop = true;
        warningSource.Play();
    }

    public void StopPowerUpWarning()
    {
        if (warningSource == null) return;

        // Unity "destroyed object" safe check
        if (!warningSource) return;

        if (warningSource.isPlaying)
            warningSource.Stop();
    }

    // Optional convenience
    public void SetMusicVolume(float v)
    {
        if (musicSource == null) return;
        musicSource.volume = Mathf.Clamp01(v);
    }

    public void SetSfxVolume(float v)
    {
        if (sfxSource == null) return;
        sfxSource.volume = Mathf.Clamp01(v);
        if (warningSource != null) warningSource.volume = Mathf.Clamp01(v);
    }
}
