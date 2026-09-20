using System;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Obstacle : MonoBehaviour
{
    public static event Action OnPlayerHit;
    public IObjectPool<Obstacle> Pool { get; set; }

    [SerializeField] private float despawnX = -15f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    private void Update()
    {
        rb.MovePosition(rb.position + Vector2.left * WorldScroller.Speed * Time.deltaTime);

        if (transform.position.x < despawnX)
            ReturnToPool();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        OnPlayerHit?.Invoke();
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (Pool != null) Pool.Release(this);
        else gameObject.SetActive(false);
    }
}

/*
 * DECISIONES DE DISEÑO
 *
 * RIGIDBODY2D KINEMATIC CON MovePosition
 * Mover el transform directamente en un objeto con collider hace que
 * el motor de física no actualice la posición del collider hasta el
 * siguiente FixedUpdate, lo que causa que los triggers se detecten
 * tarde o no se detecten. Con Rigidbody2D Kinematic y MovePosition
 * la posición del collider se actualiza correctamente cada frame.
 * gravityScale = 0 para que no caiga.
 *
 * EVENTO ESTÁTICO OnPlayerHit
 * El obstáculo no sabe quién procesa el game over. Dispara el evento
 * y se desactiva. El GameManager (o quien sea) se suscribe. Bajo
 * acoplamiento entre sistemas.
 *
 * REFERENCIA AL POOL (IObjectPool<Obstacle>)
 * El spawner asigna la referencia al pool al crear el obstáculo.
 * ReturnToPool lo usa para liberarse sin buscar al spawner. Si por
 * algún motivo Pool es null (instanciado manualmente), simplemente
 * se desactiva el GameObject.
 *
 * DESPAWN POR POSICIÓN X
 * Cuando el obstáculo sale del lado izquierdo de la pantalla vuelve
 * al pool. despawnX = -15 cubre resoluciones estándar con cámara
 * ortográfica de tamaño 5-6. Ajustá según el tamaño de tu cámara.
 *
 * TAG "Player"
 * El GameObject del jugador debe tener el tag Player para que
 * OnTriggerEnter2D lo detecte. El Collider2D del obstáculo debe
 * tener isTrigger = true.
 */
