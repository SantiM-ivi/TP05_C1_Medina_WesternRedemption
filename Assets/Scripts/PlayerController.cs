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
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    public event Action Jumped;
    public event Action Landed;

    public bool IsGrounded => grounded;

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
    private int maxJumps;
    private int airJumpsLeft;
    private float coyoteCounter;
    private float bufferCounter;
    private bool grounded;
    private bool wasGrounded;
    private bool jumpHeld;

    private float VelY
    {
        get => rb.linearVelocity.y;
        set => rb.linearVelocity = new Vector2(0f, value);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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

/*
 * DECISIONES DE DISEÑO
 *
 * MUNDO FIJO, JUGADOR FIJO EN X
 * El jugador no se mueve horizontalmente. El suelo y los obstáculos
 * avanzan hacia la izquierda.
 
 * SALTO VARIABLE (lowJumpMultiplier / fallGravityMultiplier)
 * En FixedUpdate se cambia el gravityScale según el estado vertical.
 * Al subir sin mantener el botón se aplica lowJumpMultiplier, cortando
 * el salto antes. Al caer se aplica fallGravityMultiplier para que el
 * descenso sea más rápido que el ascenso y la curva se sienta pesada
 * en lugar de floaty. Esto es preferible a cambiar velocity.y a cero
 * porque respeta la física y se integra bien con el doble salto.
 *
 * COYOTE TIME
 * Si el jugador se cae del borde sin saltar, coyoteCounter le da una
 * ventana de 0.1 s para hacerlo igual. Muchos platformers lo usan.
 *
 * JUMP BUFFER
 * Si el jugador presiona salto 0.12 s antes de tocar el suelo,
 * el salto se ejecuta igual al aterrizar. Elimina la frustración de
 * presionar "demasiado temprano" y el juego no responder.
 *
 * DATOS EN SCRIPTABLEOBJECT
 * Los valores de jumpForce, gravityScale, etc. viven en PlayerData.
 * En Awake se copian maxJumps a una variable local para que los power
 * ups puedan subirlo/bajarlo en runtime sin pisar el asset original.
 * gravityScale del Rigidbody se sobreescribe cada FixedUpdate, así
 * que no importa lo que diga el Inspector del Rigidbody.
 *
 * DOBLE SALTO / POWER UP
 * airJumpsLeft se calcula como maxJumps - 1 cada vez que se toca el
 * suelo. Un power up solo necesita cambiar MaxJumps para habilitar
 * o deshabilitar el doble salto sin ningún otro cambio en este script.
 *
 * EVENTOS Jumped / Landed
 * Se exponen como eventos para que AudioManager y el sistema de
 * partículas se suscriban sin que este script conozca su existencia.
 * Mantiene el acoplamiento bajo entre sistemas.
 *
 * GROUND CHECK CON OverlapBox
 * Se usa un box chico en los pies en lugar de un Raycast porque con
 * un Raycast en el centro del collider se pierden los bordes de
 * plataformas angostas. El box cubre todo el ancho del pie.
 * El GroundCheck debe estar posicionado en el borde inferior exacto
 * del BoxCollider2D del jugador para que no haya gap visual ni
 * detección errónea.
 *
 * COMPATIBILIDAD DE INPUT
 * Se detecta en compilación si el Input System nuevo está activo.
 * Si no está, usa el Input clásico. Así el script funciona sin
 * cambiar nada independientemente de la configuración del proyecto.
 *
 * COMPATIBILIDAD DE VERSIÓN UNITY
 * linearVelocity es la API de Unity 6+. Si el proyecto es anterior,
 * se puede reemplazar por velocity. El compilador lo detecta con el
 * símbolo UNITY_6000_0_OR_NEWER (que en este script se usa siempre
 * linearVelocity; ajustar si la versión es menor).
 */