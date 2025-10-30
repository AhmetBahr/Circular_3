using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Serializable]
    public class SpawnPoint
    {
        public Transform pointTransform;
        public EnemyDirection direction;
        public Vector2 xRange;
        public Vector2 yRange;

        [Tooltip("Bu listede bir şey varsa prefab burada yazılanlardan rastgele seçilir. Boşsa global listeden seçilir.")]
        public GameObject[] allowedPrefabs;
    }

    [Header("Spawn Points")]
    [SerializeField] private SpawnPoint[] spawnPoints;

    [Header("Global Prefabs (SpawnPoint boşsa buradan)")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Tempo")]
    [SerializeField] private float spawnInterval = 3f;

    private Coroutine loop;
    private bool isRunning;

    // Dış API
    public void StartSpawning()
    {
        if (isRunning) return;
        isRunning = true;
        loop = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning(bool reset = true)
    {
        isRunning = false;
        if (loop != null)
        {
            StopCoroutine(loop);
            loop = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        var wait = new WaitForSeconds(spawnInterval);

        while (isRunning)
        {
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                int index = Random.Range(0, spawnPoints.Length);
                var sp = spawnPoints[index];

                var prefab = ChoosePrefab(sp);
                if (prefab != null && EnemyPool.Instance != null)
                {
                    if (EnemyPool.Instance.GetTotalActiveCount() < int.MaxValue) // genel sınır içeride de kontrol ediliyor
                    {
                        var enemy = EnemyPool.Instance.GetEnemy(prefab);
                        if (enemy != null)
                        {
                            var spawnPos = new Vector3(
                                Random.Range(sp.xRange.x, sp.xRange.y),
                                Random.Range(sp.yRange.x, sp.yRange.y),
                                0f
                            ) + (sp.pointTransform ? sp.pointTransform.position : Vector3.zero);

                            enemy.transform.position = spawnPos;
                            enemy.transform.rotation = Quaternion.identity;

                            var controller = enemy.GetComponent<EnemyController>();
                            if (controller != null)
                                controller.enemyDirection = sp.direction;

                            enemy.SetActive(true);
                        }
                    }
                }
            }

            // spawnInterval her frame değiştirilebilsin istiyorsan wait'i burada tekrar oluştur
            yield return wait;
        }
    }

    private GameObject ChoosePrefab(SpawnPoint sp)
    {
        // Yerel liste varsa ondan seç
        if (sp.allowedPrefabs != null && sp.allowedPrefabs.Length > 0)
        {
            return sp.allowedPrefabs[Random.Range(0, sp.allowedPrefabs.Length)];
        }

        // Global liste
        if (enemyPrefabs != null && enemyPrefabs.Length > 0)
        {
            return enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        }

        return null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (spawnPoints == null) return;

        foreach (var sp in spawnPoints)
        {
            if (sp.pointTransform != null)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(sp.pointTransform.position, 0.1f);

                Vector3 min = new Vector3(sp.xRange.x, sp.yRange.x, 0) + sp.pointTransform.position;
                Vector3 max = new Vector3(sp.xRange.y, sp.yRange.y, 0) + sp.pointTransform.position;

                Gizmos.color = Color.red;
                Gizmos.DrawLine(new Vector3(min.x, min.y, 0), new Vector3(max.x, min.y, 0));
                Gizmos.DrawLine(new Vector3(max.x, min.y, 0), new Vector3(max.x, max.y, 0));
                Gizmos.DrawLine(new Vector3(max.x, max.y, 0), new Vector3(min.x, max.y, 0));
                Gizmos.DrawLine(new Vector3(min.x, max.y, 0), new Vector3(min.x, min.y, 0));
            }
        }
    }
#endif
}
