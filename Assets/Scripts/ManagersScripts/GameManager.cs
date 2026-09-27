using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Jugador")]
    [SerializeField] private PlayerController player;
    [SerializeField] private float invincibilityDuration = 5f;
    [SerializeField] private int startingLives = 1;

    [Header("UI - HUD")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI invincibilityTimerText;

    [Header("UI - Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Header("UI - Pausa")]
    [SerializeField] private GameObject pausePanel;

    [Header("Audio")]
    [SerializeField] private AudioClip gameOverClip;

    public bool IsGameOver { get; private set; }
    public bool IsPaused { get; private set; }

    private float score;
    private int lives;
    private float invincibilityTimer;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        lives = startingLives;
    }

    private void Start()
    {
        AudioManager.Instance?.PlayGameplayMusic();
        UpdateLivesUI();

        if (invincibilityTimerText != null)
            invincibilityTimerText.gameObject.SetActive(false);
    }

    private void OnEnable() => Obstacle.OnPlayerHit += TriggerDeath;
    private void OnDisable() => Obstacle.OnPlayerHit -= TriggerDeath;

    private void Update()
    {
        if (IsGameOver) return;
        if (PausePressed()) TogglePause();
        if (IsPaused) return;

        score += Time.deltaTime * WorldScroller.Speed;
        if (scoreText != null)
            scoreText.text = Mathf.FloorToInt(score).ToString();

        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimerText != null)
                invincibilityTimerText.text = Mathf.CeilToInt(invincibilityTimer).ToString();

            if (invincibilityTimer <= 0f && invincibilityTimerText != null)
                invincibilityTimerText.gameObject.SetActive(false);
        }
    }

    public void AddScore(float amount)
    {
        if (IsGameOver) return;
        score += amount;
    }

    public void ApplyPowerUp(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.Invincibility:
                invincibilityTimer = invincibilityDuration;
                player.SetInvincible(invincibilityDuration);
                if (invincibilityTimerText != null)
                    invincibilityTimerText.gameObject.SetActive(true);
                break;

            case PowerUpType.ExtraLife:
                lives++;
                UpdateLivesUI();
                break;
        }
    }

    private void TriggerDeath()
    {
        if (IsGameOver) return;
        if (player.IsInvincible) return;

        if (lives > 0)
        {
            lives--;
            UpdateLivesUI();
            player.SetInvincible(invincibilityDuration);
            invincibilityTimer = invincibilityDuration;
            if (invincibilityTimerText != null)
                invincibilityTimerText.gameObject.SetActive(true);
            return;
        }

        TriggerGameOver();
    }

    private void TriggerGameOver()
    {
        IsGameOver = true;

        if (IsPaused) TogglePause();

        AudioManager.Instance?.PlaySFX(gameOverClip);
        WorldScroller.Instance.StopScrolling();
        player.Die();

        if (finalScoreText != null)
            finalScoreText.text = Mathf.FloorToInt(score).ToString();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
        if (pausePanel != null) pausePanel.SetActive(IsPaused);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = "x" + lives;
    }

    private static bool PausePressed()
    {
#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        return kb != null && kb.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }
}