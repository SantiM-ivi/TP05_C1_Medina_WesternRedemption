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

    private void Awake()
    {
        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    private void Update()
    {
        transform.Translate(Vector3.left * WorldScroller.Speed * Time.deltaTime);

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
 * transform.Translate EN UPDATE
 * El suelo usa transform.Translate en Update. Si el obstáculo usara
 * rb.MovePosition, este opera en pasos de física (FixedUpdate, 50Hz
 * por defecto) mientras Update corre a la frecuencia del display.
 * El timing diferente genera un micro-desfase visual entre suelo y
 * obstáculos aunque la velocidad sea idéntica. Usar el mismo método
 * en el mismo ciclo elimina el desfase.
 *
 * RIGIDBODY2D KINEMATIC
 * Se mantiene porque Unity 2D requiere que al menos uno de los dos
 * objetos en una colisión tenga Rigidbody2D para que OnTriggerEnter2D
 * se dispare. El jugador ya tiene uno, pero tenerlo también en el
 * obstáculo hace la detección más robusta. Con bodyType Kinematic y
 * gravityScale 0 no interfiere con el movimiento manual por transform.
 *
 * EVENTO ESTÁTICO OnPlayerHit
 * El obstáculo no sabe quién procesa el game over. Dispara el evento
 * y se libera al pool. Bajo acoplamiento entre sistemas.
 *
 * REFERENCIA AL POOL
 * El spawner asigna Pool al crear el obstáculo. ReturnToPool lo usa
 * para liberarse sin buscar al spawner. Si Pool es null, desactiva
 * el GameObject como fallback.
 */