using System;
using System.Collections;
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
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    [Header("Invencibilidad")]
    [SerializeField] private float flashInterval = 0.1f;

    public event Action Jumped;
    public event Action Landed;
    public event Action Died;

    public bool IsGrounded => grounded;
    public bool IsAlive => alive;
    public bool IsInvincible => invincible;

    public int MaxJumps
    {
        get => maxJumps;
        set => maxJumps = Mathf.Max(1, value);
    }

    public float FullJumpAirTime
    {
        get
        {
            float g = Mathf.Abs(Physics2D.gravity.y) * data.gravityScale;
            float timeUp = data.jumpForce / g;
            float apex = (data.jumpForce * data.jumpForce) / (2f * g);
            float timeDown = Mathf.Sqrt(2f * apex / (g * data.fallGravityMultiplier));
            return timeUp + timeDown;
        }
    }

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private int maxJumps;
    private int airJumpsLeft;
    private float coyoteCounter;
    private float bufferCounter;
    private bool grounded;
    private bool wasGrounded;
    private bool jumpHeld;
    private bool alive = true;
    private bool invincible;

    private float VelY
    {
        get => rb.linearVelocity.y;
        set => rb.linearVelocity = new Vector2(0f, value);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        maxJumps = data.maxJumps;
        rb.gravityScale = data.gravityScale;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (!alive) return;

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
        if (!alive) return;

        float vy = VelY;
        float mult = 1f;

        if (vy < 0f) mult = data.fallGravityMultiplier;
        else if (vy > 0f && !jumpHeld) mult = data.lowJumpMultiplier;

        rb.gravityScale = data.gravityScale * mult;
    }

    public void Die()
    {
        alive = false;
        rb.gravityScale = data.gravityScale;
        Died?.Invoke();
    }

    public void SetInvincible(float duration)
    {
        StopCoroutine(nameof(InvincibilityRoutine));
        StartCoroutine(InvincibilityRoutine(duration));
    }

    private IEnumerator InvincibilityRoutine(float duration)
    {
        invincible = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (sr != null) sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        if (sr != null) sr.enabled = true;
        invincible = false;
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