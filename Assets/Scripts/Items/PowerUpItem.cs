using System.Collections;
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
    [SerializeField] private float collectDuration = 0.3f;
    [SerializeField] private float launchForce = 8f;

    private bool collected;

    private void Awake()
    {
        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    private void OnEnable()
    {
        collected = false;
        transform.localScale = Vector3.one;
    }

    private void Update()
    {
        if (collected) return;
        transform.Translate(Vector3.left * WorldScroller.Speed * Time.deltaTime, Space.World);
        if (transform.position.x < despawnX) ReturnToPool();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance.IsGameOver) return;

        collected = true;
        AudioManager.Instance?.PlaySFX(pickupClip);
        GameManager.Instance.ApplyPowerUp(type);
        other.GetComponent<PlayerAnimator>()?.TriggerPickup();
        StartCoroutine(CollectTween());
    }

    private IEnumerator CollectTween()
    {
        float angle = Random.Range(30f, 150f);
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsed < collectDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / collectDuration;
            transform.position += (Vector3)(direction * launchForce * Time.deltaTime);
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (Pool != null) Pool.Release(this);
        else gameObject.SetActive(false);
    }
}