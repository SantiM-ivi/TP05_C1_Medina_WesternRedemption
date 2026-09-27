
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Runner/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Salto")]
    public float jumpForce = 12f;
    public float gravityScale = 3f;
    public float fallGravityMultiplier = 1.6f;
    public float lowJumpMultiplier = 2f;
    [Min(1)] public int maxJumps = 1;

    [Header("Tolerancias de input")]
    public float coyoteTime = 0.1f;
    public float jumpBuffer = 0.12f;
}

