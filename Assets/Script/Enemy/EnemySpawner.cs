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
    }

    [SerializeField] private SpawnPoint[] spawnPoints;
    [SerializeField] private float spawnInterval = 3f;

    private Coroutine loop;
    private bool isRunning;

    // Dışarıdan kontrol edilecek API
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
        // reset=true ise bir sonraki Start'ta beklemeden başlamak için ekstra bir şey yapmaya gerek yok
    }

    private IEnumerator SpawnLoop()
    {
        while (isRunning)
        {
            // Spawn
            if (EnemyPool.Instance.CanSpawn() && spawnPoints != null && spawnPoints.Length > 0)
            {
                int index = Random.Range(0, spawnPoints.Length);
                var sp = spawnPoints[index];

                Vector3 spawnPos = new Vector3(
                    Random.Range(sp.xRange.x, sp.xRange.y),
                    Random.Range(sp.yRange.x, sp.yRange.y),
                    0f
                ) + sp.pointTransform.position;

                GameObject enemy = EnemyPool.Instance.GetEnemy();
                enemy.transform.position = spawnPos;
                enemy.transform.rotation = Quaternion.identity;

                var controller = enemy.GetComponent<EnemyController>();
                controller.enemyDirection = sp.direction;

                enemy.SetActive(true);
            }

            // spawnInterval değişirse etkilesin diye her seferinde yeni WaitForSeconds
            yield return new WaitForSeconds(spawnInterval);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (spawnPoints == null) return;

        Gizmos.color = Color.black;

        foreach (var sp in spawnPoints)
        {
            if (sp.pointTransform != null)
            {
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
