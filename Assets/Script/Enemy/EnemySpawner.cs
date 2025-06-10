using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        public Transform pointTransform;
        public EnemyDirection direction;
        public Vector2 xRange;
        public Vector2 yRange;
    }

    [SerializeField] private GameManager gameManager;
    [SerializeField] private SpawnPoint[] spawnPoints;
    [SerializeField] private float spawnInterval = 3f;

    private float spawnTimer;

    private void Awake()
    {
        var gameManagerObject = GameObject.Find("GameManager");
        gameManager = gameManagerObject.GetComponent<GameManager>();

    }

    void Update()
    {
        if (gameManager.isGameStarted)
        {
            StartSpawingEnemys();
        }
    }

    public void StartSpawingEnemys()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && EnemyPool.Instance.CanSpawn())
        {
            spawnTimer = 0f;

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
    }

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
}
