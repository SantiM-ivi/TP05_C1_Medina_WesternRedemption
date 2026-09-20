using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Runner/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Salto")]
    [Tooltip("Velocidad vertical inicial del salto")]
    public float jumpForce = 12f;
    [Tooltip("Gravedad base del Rigidbody2D")]
    public float gravityScale = 3f;
    [Tooltip("Multiplicador de gravedad al caer")]
    public float fallGravityMultiplier = 1.6f;
    [Tooltip("Multiplicador al subir sin mantener el boton (salto corto)")]
    public float lowJumpMultiplier = 2f;
    [Min(1)] public int maxJumps = 1;

    [Header("Tolerancias de input")]
    [Tooltip("Segundos despues de dejar el suelo en los que todavia se puede saltar")]
    public float coyoteTime = 0.1f;
    [Tooltip("Segundos que se recuerda un salto presionado antes de aterrizar")]
    public float jumpBuffer = 0.12f;

    [Header("Mundo")]
    public float startSpeed = 6f;
}
