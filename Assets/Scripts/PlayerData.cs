using UnityEngine;

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

/*
 * DECISIONES DE DISEÑO
 *
 * SCRIPTABLEOBJECT COMO FUENTE DE VERDAD DEL JUGADOR
 * Centraliza los parámetros del jugador en un asset editable sin
 * entrar al Prefab. El TP lo requiere explícitamente.
 *
 * startSpeed REMOVIDO
 * Era un parámetro del mundo, no del jugador. Vive ahora en
 * WorldScroller donde corresponde.
 *
 * COPIA LOCAL EN RUNTIME
 * PlayerController copia maxJumps a una variable local en Awake.
 * Los power ups modifican esa copia sin tocar el asset.
 *
 * GRAVEDAD EN TRES PARTES
 * gravityScale es la base. fallGravityMultiplier y lowJumpMultiplier
 * se aplican sobre ella en FixedUpdate según el estado vertical,
 * construyendo la curva de salto sin cambiar velocity directamente.
 */