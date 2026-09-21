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

    [Header("UI - HUD")]
    [SerializeField] private TextMeshProUGUI scoreText;

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

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

  
    private void OnEnable() => Obstacle.OnPlayerHit += TriggerGameOver;
    private void OnDisable() => Obstacle.OnPlayerHit -= TriggerGameOver;
    private void Start() => AudioManager.Instance.PlayGameplayMusic();
    private void Update()
    {
        if (IsGameOver) return;
        if (PausePressed()) TogglePause();
        if (IsPaused) return;

        score += Time.deltaTime * WorldScroller.Speed;
        if (scoreText != null)
            scoreText.text = Mathf.FloorToInt(score).ToString();
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
        if (pausePanel != null) pausePanel.SetActive(IsPaused);
    }

    private void TriggerGameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        if (IsPaused) TogglePause();

        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlaySFX(gameOverClip);

        WorldScroller.Instance.StopScrolling();
        player.Die();

        if (finalScoreText != null)
            finalScoreText.text = Mathf.FloorToInt(score).ToString();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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


