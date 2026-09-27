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

