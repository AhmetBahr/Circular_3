using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        public Transform pointTransform;
        public CoinDirection direction;
        public Vector2 xRange;
        public Vector2 yRange;
    }

    [SerializeField] private GameManager gameManager;
    [SerializeField] private SpawnPoint[] spawnPoints;
    [SerializeField] private float spawnInterval = 2f;

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
            coinsSpawned();
        }
    }

    private void coinsSpawned()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && CoinPool.Instance.CanSpawn())
        {
            spawnTimer = 0f;

            // Rastgele bir spawn noktası seç
            int index = Random.Range(0, spawnPoints.Length);
            SpawnPoint sp = spawnPoints[index];

            // Spawn pozisyonunu hesapla
            Vector3 spawnPos = new Vector3(
                Random.Range(sp.xRange.x, sp.xRange.y),
                Random.Range(sp.yRange.x, sp.yRange.y),
                0f
            );

            spawnPos += sp.pointTransform.position;

            GameObject coin = CoinPool.Instance.GetCoin();
            coin.transform.position = spawnPos;
            coin.transform.rotation = Quaternion.identity;

            var coinManager = coin.GetComponent<CoinController>();
            coinManager.coinDirection = sp.direction;

            coin.SetActive(true);
        }
    }

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
}
