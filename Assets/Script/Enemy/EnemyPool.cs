using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;

    [System.Serializable]
    public class PoolEntry
    {
        public GameObject prefab;          // farklı düşman tipleri
        public int initialSize = 5;        // başlangıçta kaç tane üretilecek
        public bool canExpand = true;      // havuz dolarsa yeni üretebilsin mi?

        [HideInInspector] public List<GameObject> pool; // runtime
    }

    [Header("Prefabs & Pool")]
    [SerializeField] private PoolEntry[] entries;

    [Header("Limits")]
    [SerializeField] private int maxActiveEnemies = 6;  // toplam eşzamanlı aktif sınırı (tüm tipler)

    private Dictionary<GameObject, PoolEntry> _map;

    void Awake()
    {
        Instance = this;

        _map = new Dictionary<GameObject, PoolEntry>();
        foreach (var e in entries)
        {
            if (e.prefab == null) continue;
            if (!_map.ContainsKey(e.prefab)) _map.Add(e.prefab, e);
            e.pool = new List<GameObject>(e.initialSize);

            for (int i = 0; i < e.initialSize; i++)
            {
                var obj = Instantiate(e.prefab, transform); // hierarchy temiz kalsın
                obj.SetActive(false);
                e.pool.Add(obj);
            }
        }
    }

    public GameObject GetEnemy(GameObject prefab)
    {
        if (prefab == null || !_map.ContainsKey(prefab)) return null;
        if (GetTotalActiveCount() >= maxActiveEnemies) return null;

        var entry = _map[prefab];

        // pasif bul
        for (int i = 0; i < entry.pool.Count; i++)
        {
            if (!entry.pool[i].activeInHierarchy)
                return entry.pool[i];
        }

        // genişlet
        if (entry.canExpand)
        {
            var obj = Instantiate(entry.prefab, transform);
            obj.SetActive(false);
            entry.pool.Add(obj);
            return obj;
        }

        return null;
    }

    public int GetTotalActiveCount()
    {
        int count = 0;
        foreach (var e in entries)
        {
            if (e.pool == null) continue;
            for (int i = 0; i < e.pool.Count; i++)
                if (e.pool[i].activeInHierarchy) count++;
        }
        return count;
    }

    // (Opsiyonel) hepsini pasifleştirmek istersen
    public void DespawnAll()
    {
        foreach (var e in entries)
        {
            if (e.pool == null) continue;
            for (int i = 0; i < e.pool.Count; i++)
                e.pool[i].SetActive(false);
        }
    }
}
