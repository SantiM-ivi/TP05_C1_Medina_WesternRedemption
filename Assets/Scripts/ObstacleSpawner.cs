using UnityEngine;
using UnityEngine.Pool;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private PlayerController player;
    [SerializeField] private float spawnX = 12f;
    [SerializeField] private float spawnY = 0f;
    [SerializeField] private float extraGap = 0.4f;
    [SerializeField] private float maxInterval = 3f;

    private IObjectPool<Obstacle>[] pools;
    private float timer;

    private void Awake()
    {
        pools = new IObjectPool<Obstacle>[obstaclePrefabs.Length];

        for (int i = 0; i < obstaclePrefabs.Length; i++)
        {
            int idx = i;
            pools[i] = new ObjectPool<Obstacle>(
                createFunc: () =>
                {
                    var go = Instantiate(obstaclePrefabs[idx]);
                    var obs = go.GetComponent<Obstacle>();
                    obs.Pool = pools[idx];
                    return obs;
                },
                actionOnGet:     obs => obs.gameObject.SetActive(true),
                actionOnRelease: obs => obs.gameObject.SetActive(false),
                actionOnDestroy: obs => Destroy(obs.gameObject),
                defaultCapacity: 4,
                maxSize: 10
            );
        }

        ResetTimer();
    }

    private void Update()
    {
        if (!WorldScroller.Running) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Spawn();
            ResetTimer();
        }
    }

    private void Spawn()
    {
        int idx = Random.Range(0, pools.Length);
        Obstacle obs = pools[idx].Get();
        obs.transform.position = new Vector3(spawnX, spawnY, 0f);
    }

    private void ResetTimer()
    {
        float minInterval = (player != null ? player.FullJumpAirTime : 1f) + extraGap;
        float safeMax = Mathf.Max(minInterval + 0.5f, maxInterval);
        timer = Random.Range(minInterval, safeMax);
    }
}


