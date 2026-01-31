using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public float gameDuration = 60f;
    public int livesStart = 3;
    public int maxLives = 3;

    [Header("UI Text")]
    public TMP_Text timeText;
    public TMP_Text coinsText;
    public TMP_Text livesText;

    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel;

    private float timeLeft;
    private int coins;
    private int lives;
    private bool ended;

    void Start()
    {
        timeLeft = gameDuration;
        coins = 0;
        lives = livesStart;
        ended = false;

        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);

        Time.timeScale = 1f;
        UpdateUI();
    }

    void Update()
    {
        if (ended) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            Win();
        }

        UpdateUI();
    }

    // ---------- Coins ----------
    public void AddCoin(int amount = 1)
    {
        if (ended) return;
        coins += amount;
        UpdateUI();
    }

    public bool TrySpendCoins(int amount)
    {
        if (ended) return false;
        if (coins < amount) return false;

        coins -= amount;
        UpdateUI();
        return true;
    }

    public int GetCoins() => coins;

    // ---------- Lives ----------
    public int GetLives() => lives;

    public bool CanGainLife() => lives < maxLives;

    public void GainLife(int amount = 1)
    {
        if (ended) return;
        lives = Mathf.Min(maxLives, lives + amount);
        UpdateUI();
    }

    public void TakeHit(int damage = 1)
    {
        if (ended) return;

        lives -= damage;
        if (lives < 0) lives = 0;

        UpdateUI();

        if (lives <= 0)
            Lose();
    }

    // ---------- End states ----------
    public void Lose()
    {
        if (ended) return;

        ended = true;
        if (losePanel) losePanel.SetActive(true);
        SoundManager.instance?.PlayLose();
        Time.timeScale = 0f;
    }

    void Win()
    {
        if (ended) return;

        ended = true;
        if (winPanel) winPanel.SetActive(true);
        SoundManager.instance?.PlayWin();
        Time.timeScale = 0f;
    }

    void UpdateUI()
    {
        if (timeText)
            timeText.text = "Time: " + Mathf.CeilToInt(timeLeft);

        if (coinsText)
            coinsText.text = "Coins: " + coins;

        if (livesText)
            livesText.text = "Lives: " + lives;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
