using UnityEngine;
using UnityEngine.Pool;

public class ScoreItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private float spawnX = 12f;
    [SerializeField] private float[] spawnHeights = { 1.5f, 3f };
    [SerializeField] private Vector2 intervalRange = new Vector2(4f, 8f);

    private IObjectPool<ScoreItem> pool;
    private float timer;

    private void Awake()
    {
        pool = new ObjectPool<ScoreItem>(
            createFunc: () =>
            {
                var go = Instantiate(itemPrefab);
                var item = go.GetComponent<ScoreItem>();
                item.Pool = pool;
                return item;
            },
            actionOnGet:     i => i.gameObject.SetActive(true),
            actionOnRelease: i => i.gameObject.SetActive(false),
            actionOnDestroy: i => Destroy(i.gameObject),
            defaultCapacity: 3,
            maxSize: 6
        );

        ResetTimer();
    }

    private void Update()
    {
        if (!WorldScroller.Running) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        var item = pool.Get();
        float y = spawnHeights[Random.Range(0, spawnHeights.Length)];
        item.transform.position = new Vector3(spawnX, y, 0f);
        ResetTimer();
    }

    private void ResetTimer() => timer = Random.Range(intervalRange.x, intervalRange.y);
}
