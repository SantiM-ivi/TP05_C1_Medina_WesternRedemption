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



