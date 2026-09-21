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

    private void Update()
    {
        if (!running) return;
        currentSpeed = Mathf.Min(currentSpeed + accelerationRate * Time.deltaTime, maxSpeed);
    }

    public void StopScrolling() => running = false;
    public void ResetSpeed() => currentSpeed = startSpeed;
}

/*
 * DECISIONES DE DISEÑO
 *
 * SINGLETON CON PROPIEDAD ESTÁTICA Speed
 * Los obstáculos y el suelo leen Speed cada frame sin necesitar
 * una referencia serializada. Cualquier script accede con
 * WorldScroller.Speed sin buscar el componente.
 *
 * ACELERACIÓN GRADUAL EN Update
 * La velocidad sube a ritmo constante (accelerationRate unidades/s²)
 * hasta maxSpeed. La rampa hace que el juego sea manejable al inicio
 * y se vuelva difícil con el tiempo.
 *
 * SIN DEPENDENCIA DE PlayerData
 * startSpeed es un parámetro del mundo, no del jugador. Cada sistema
 * tiene sus propios datos serializados para evitar acoplamiento.
 *
 * StopScrolling / ResetSpeed
 * GameManager llama StopScrolling en game over. ResetSpeed sirve
 * si se reinicia sin recargar la escena.
 */

