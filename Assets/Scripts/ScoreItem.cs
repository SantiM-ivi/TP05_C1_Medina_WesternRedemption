using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ScoreItem : MonoBehaviour
{
    public IObjectPool<ScoreItem> Pool { get; set; }

    [SerializeField] private float scoreAmount = 100f;
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

        if (transform.position.x < despawnX)
            ReturnToPool();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance.IsGameOver) return;

        GameManager.Instance.AddScore(scoreAmount);
        AudioManager.Instance.PlaySFX(pickupClip);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (Pool != null) Pool.Release(this);
        else gameObject.SetActive(false);
    }
}
