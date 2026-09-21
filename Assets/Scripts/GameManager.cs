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

/*
 * DECISIONES DE DISEÑO
 *
 * Time.timeScale = 0 PARA PAUSAR
 * Poner timeScale a 0 detiene Update, FixedUpdate y las animaciones
 * en todos los objetos de la escena sin necesidad de pausar cada
 * sistema individualmente. Al reanudar se restaura a 1. Es la forma
 * estándar de pausa en Unity.
 *
 * GUARDIA if (IsPaused) EN Update
 * Con timeScale 0 Update sigue ejecutándose (corre en tiempo real).
 * La guardia evita que el puntaje siga sumando mientras está pausado,
 * ya que Time.deltaTime con timeScale 0 devuelve 0 de todas formas,
 * pero la claridad de intención vale la línea extra.
 *
 * if (IsGameOver) ANTES DE PausePressed
 * Si el juego terminó no tiene sentido poder pausar. La guardia al
 * inicio de Update lo impide sin lógica adicional.
 *
 * TogglePause EN TriggerGameOver
 * Si el jugador muere mientras está pausado, se reanuda el timeScale
 * antes de procesar el game over. Sin esto la escena quedaría
 * congelada con el panel de game over visible pero sin poder
 * interactuar correctamente con la UI.
 *
 * Time.timeScale = 1f EN Restart
 * LoadScene no resetea timeScale. Si se reinicia desde la pausa o
 * desde game over, sin este reset la nueva escena arranca congelada.
 *
 * SETUP DE UI
 * - Crear un Panel "PausePanel" desactivado dentro del Canvas con
 *   un Text (TMP) que diga "PAUSE" y un Button "Reanudar" que llame
 *   GameManager.Instance.TogglePause().
 * - Asignar pausePanel en el Inspector de GameManager.
 * - El panel arranca desactivado en el editor igual que GameOverPanel.
 */