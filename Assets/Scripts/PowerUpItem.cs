using UnityEngine;
using UnityEngine.Pool;

public enum PowerUpType { Invincibility, ExtraLife }

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PowerUpItem : MonoBehaviour
{
    public IObjectPool<PowerUpItem> Pool { get; set; }

    [SerializeField] private PowerUpType type;
    [SerializeField] private AudioClip pickupClip;
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
        if (transform.position.x < despawnX) ReturnToPool();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance.IsGameOver) return;

        AudioManager.Instance?.PlaySFX(pickupClip);
        GameManager.Instance.ApplyPowerUp(type);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (Pool != null) Pool.Release(this);
        else gameObject.SetActive(false);
    }
}
