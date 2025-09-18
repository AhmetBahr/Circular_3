using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinSpawner : MonoBehaviour
{
    [Serializable]
    public class SpawnPoint
    {
        public Transform pointTransform;
        public CoinDirection direction;
        public Vector2 xRange;
        public Vector2 yRange;
    }

    [SerializeField] private SpawnPoint[] spawnPoints;
    [SerializeField] private float spawnInterval = 2f;

    private Coroutine loop;
    private bool isRunning;

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
        while (isRunning)
        {
            if (CoinPool.Instance.CanSpawn() && spawnPoints != null && spawnPoints.Length > 0)
            {
                int index = Random.Range(0, spawnPoints.Length);
                var sp = spawnPoints[index];

                Vector3 spawnPos = new Vector3(
                    Random.Range(sp.xRange.x, sp.xRange.y),
                    Random.Range(sp.yRange.x, sp.yRange.y),
                    0f
                ) + sp.pointTransform.position;

                GameObject coin = CoinPool.Instance.GetCoin();
                coin.transform.position = spawnPos;
                coin.transform.rotation = Quaternion.identity;

                var coinManager = coin.GetComponent<CoinController>();
                coinManager.coinDirection = sp.direction;

                coin.SetActive(true);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (spawnPoints == null) return;

        Gizmos.color = Color.cyan;

        foreach (var sp in spawnPoints)
        {
            if (sp.pointTransform != null)
            {
                Gizmos.DrawSphere(sp.pointTransform.position, 0.1f);

                Vector3 min = new Vector3(sp.xRange.x, sp.yRange.x, 0) + sp.pointTransform.position;
                Vector3 max = new Vector3(sp.xRange.y, sp.yRange.y, 0) + sp.pointTransform.position;

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(new Vector3(min.x, min.y, 0), new Vector3(max.x, min.y, 0));
                Gizmos.DrawLine(new Vector3(max.x, min.y, 0), new Vector3(max.x, max.y, 0));
                Gizmos.DrawLine(new Vector3(max.x, max.y, 0), new Vector3(min.x, max.y, 0));
                Gizmos.DrawLine(new Vector3(min.x, max.y, 0), new Vector3(min.x, min.y, 0));
            }
        }
    }
#endif
}
