using UnityEngine;
using System.Collections.Generic;

public class CoinPool : MonoBehaviour
{
    public static CoinPool Instance;

    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private int maxActiveCoins = 2;

    private List<GameObject> pool = new List<GameObject>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(coinPrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetCoin()
    {
        foreach (GameObject coin in pool)
        {
            if (!coin.activeInHierarchy)
            {
                return coin;
            }
        }

        return null; 
    }

    public bool CanSpawn()
    {
        int activeCount = 0;
        foreach (GameObject coin in pool)
        {
            if (coin.activeInHierarchy)
                activeCount++;
        }

        return activeCount < maxActiveCoins;
    }
}