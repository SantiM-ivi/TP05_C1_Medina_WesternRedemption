using UnityEngine;
using UnityEngine.Pool;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private GameObject invincibilityPrefab;
    [SerializeField] private GameObject extraLifePrefab;
    [SerializeField] private float spawnX = 12f;
    [SerializeField] private float[] spawnHeights = { 1.5f, 3f };
    [SerializeField] private Vector2 intervalRange = new Vector2(10f, 20f);

    private IObjectPool<PowerUpItem> invincibilityPool;
    private IObjectPool<PowerUpItem> extraLifePool;
    private float timer;

    private void Awake()
    {
        invincibilityPool = CreatePool(invincibilityPrefab);
        extraLifePool     = CreatePool(extraLifePrefab);
        ResetTimer();
    }

    private IObjectPool<PowerUpItem> CreatePool(GameObject prefab)
    {
        IObjectPool<PowerUpItem> pool = null;
        pool = new ObjectPool<PowerUpItem>(
            createFunc: () =>
            {
                var go   = Instantiate(prefab);
                var item = go.GetComponent<PowerUpItem>();
                item.Pool = pool;
                return item;
            },
            actionOnGet:     i => i.gameObject.SetActive(true),
            actionOnRelease: i => i.gameObject.SetActive(false),
            actionOnDestroy: i => Destroy(i.gameObject),
            defaultCapacity: 2,
            maxSize: 4
        );
        return pool;
    }

    private void Update()
    {
        if (!WorldScroller.Running) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        Spawn();
        ResetTimer();
    }

    private void Spawn()
    {
        var pool = Random.value > 0.5f ? invincibilityPool : extraLifePool;
        var item = pool.Get();
        float y  = spawnHeights[Random.Range(0, spawnHeights.Length)];
        item.transform.position = new Vector3(spawnX, y, 0f);
    }

    private void ResetTimer() => timer = Random.Range(intervalRange.x, intervalRange.y);
}
