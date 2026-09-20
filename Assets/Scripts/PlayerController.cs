using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerData data;

    [Header("Ground check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new(0.5f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    public event Action Jumped;
    public event Action Landed;

    public bool IsGrounded => grounded;

    // Cantidad total de saltos (1 = salto simple). Los power ups pueden modificarla.
    public int MaxJumps
    {
        get => maxJumps;
        set => maxJumps = Mathf.Max(1, value);
    }

    // Tiempo aproximado en el aire de un salto completo. Sirve para calcular el hueco minimo del spawner.
    public float FullJumpAirTime
    {
        get
        {
            float g = Mathf.Abs(Physics2D.gravity.y) * data.gravityScale;
            float up = data.jumpForce / g;
            float apex = data.jumpForce * data.jumpForce / (2f * g);
            float down = Mathf.Sqrt(2f * apex / (g * data.fallGravityMultiplier));
            return up + down;
        }
    }

    private Rigidbody2D rb;
    private int maxJumps;
    private int airJumpsLeft;
    private float coyoteCounter;
    private float bufferCounter;
    private bool grounded;
    private bool wasGrounded;
    private bool jumpHeld;

#if UNITY_6000_0_OR_NEWER
    private float VelY
    {
        get => rb.linearVelocity.y;
        set => rb.linearVelocity = new Vector2(0f, value);
    }
#else
    private float VelY
    {
        get => rb.velocity.y;
        set => rb.velocity = new Vector2(0f, value);
    }
#endif

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Copia local: no se modifica el ScriptableObject en runtime.
        maxJumps = data.maxJumps;
        rb.gravityScale = data.gravityScale;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (JumpPressed()) bufferCounter = data.jumpBuffer;
        jumpHeld = JumpHeld();
        bufferCounter -= Time.deltaTime;

        wasGrounded = grounded;
        grounded = VelY <= 0.01f &&
                   Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        if (grounded)
        {
            coyoteCounter = data.coyoteTime;
            airJumpsLeft = maxJumps - 1;
            if (!wasGrounded) Landed?.Invoke();
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (bufferCounter > 0f)
        {
            if (coyoteCounter > 0f)
            {
                DoJump();
                coyoteCounter = 0f;
            }
            else if (airJumpsLeft > 0)
            {
                airJumpsLeft--;
                DoJump();
            }
        }
    }

    private void FixedUpdate()
    {
        float vy = VelY;
        float mult = 1f;

        if (vy < 0f) mult = data.fallGravityMultiplier;
        else if (vy > 0f && !jumpHeld) mult = data.lowJumpMultiplier;

        rb.gravityScale = data.gravityScale * mult;
    }

    private void DoJump()
    {
        bufferCounter = 0f;
        VelY = data.jumpForce;
        grounded = false;
        Jumped?.Invoke();
    }

    private static bool JumpPressed()
    {
#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        var ms = Mouse.current;
        var ts = Touchscreen.current;
        return (kb != null && (kb.spaceKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame))
            || (ms != null && ms.leftButton.wasPressedThisFrame)
            || (ts != null && ts.primaryTouch.press.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)
            || Input.GetMouseButtonDown(0);
#endif
    }

    private static bool JumpHeld()
    {
#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        var ms = Mouse.current;
        var ts = Touchscreen.current;
        return (kb != null && (kb.spaceKey.isPressed || kb.upArrowKey.isPressed))
            || (ms != null && ms.leftButton.isPressed)
            || (ts != null && ts.primaryTouch.press.isPressed);
#else
        return Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)
            || Input.GetMouseButton(0);
#endif
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
