using UnityEngine;

public class WorldScroller : MonoBehaviour
{
    public static WorldScroller Instance { get; private set; }
    public static float Speed => Instance != null ? Instance.currentSpeed : 0f;
    public static bool Running => Instance != null && Instance.running;

    [SerializeField] private float startSpeed = 6f;
    [SerializeField] private float accelerationRate = 0.3f;
    [SerializeField] private float maxSpeed = 22f;

    private float currentSpeed;
    private bool running;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        currentSpeed = startSpeed;
        running = true;
    }

    public void StopScrolling() => running = false;
    public void ResetSpeed() => currentSpeed = startSpeed;
}

/*
 * DECISIONES DE DISEÑO
 *
 * SIN DEPENDENCIA DE PlayerData
 * startSpeed es un parámetro del mundo, no del jugador. Ponerlo en
 * PlayerData obligaba a WorldScroller a conocer ese ScriptableObject
 * sin una razón de diseño válida. Ahora cada sistema tiene sus propios
 * datos serializados.
 *
 * SINGLETON CON PROPIEDAD ESTÁTICA Speed
 * Los obstáculos y el suelo leen Speed sin necesitar una referencia
 * serializada. Cualquier script accede con WorldScroller.Speed.
 *
 * ACELERACIÓN GRADUAL
 * La velocidad sube a ritmo constante hasta maxSpeed. La rampa hace
 * que el juego sea manejable al inicio y se vuelva difícil con el tiempo.
 *
 * StopScrolling / ResetSpeed
 * Game Over llama StopScrolling. ResetSpeed sirve si se reinicia sin
 * recargar la escena.
 */