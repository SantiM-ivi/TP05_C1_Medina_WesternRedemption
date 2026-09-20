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

/*
 * DECISIONES DE DISEÑO
 *
 * UN POOL POR PREFAB
 * Cada tipo de obstáculo tiene su propio ObjectPool. Esto permite que
 * los objetos vuelvan al pool correcto sin lógica adicional. El índice
 * idx se captura por closure porque el loop crea lambdas asíncronas.
 *
 * INTERVALO MÍNIMO BASADO EN FullJumpAirTime
 * El intervalo mínimo entre spawns es el tiempo que el jugador pasa en
 * el aire durante un salto completo más extraGap. Así se garantiza que
 * siempre hay espacio suficiente para saltar entre obstáculos y nunca
 * se genera una situación imposible de resolver.
 * Si player es null (referencia no asignada) se usa 1s como fallback.
 *
 * RANDOM ENTRE TIPOS DE PREFAB
 * Se elige un pool al azar cada spawn. Con múltiples prefabs (obstáculo
 * bajo, obstáculo alto) esto da variedad sin lógica adicional. El peso
 * de cada tipo es uniforme; si se quiere sesgar, reemplazar
 * Random.Range por un sistema de pesos.
 *
 * spawnY FIJO EN EL INSPECTOR
 * Los obstáculos aparecen siempre en la misma Y (el suelo). Si se
 * quieren obstáculos aéreos, se pueden agregar más prefabs con un
 * spawnY diferente o convertir el campo en un array de posiciones.
 *
 * SETUP EN ESCENA
 * - Crear prefabs de obstáculo con SpriteRenderer, BoxCollider2D
 *   (isTrigger ON) y el script Obstacle.
 * - Asignar los prefabs al array obstaclePrefabs.
 * - Asignar la referencia al PlayerController para el cálculo del gap.
 * - spawnX debe estar fuera del borde derecho de la cámara.
 */
