using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject shopItemPrefab;
    [SerializeField] private ShopItemSO[] allItems;

    private void Start()
    {
        foreach (var item in allItems)
        {
            GameObject obj = Instantiate(shopItemPrefab, contentParent);
            ShopItemUI itemUI = obj.GetComponent<ShopItemUI>();
            itemUI.Setup(item);
        }
        
        LoadShop();
    }
    
    private void LoadShop()
    {
        // Content'i temizle
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Tüm itemleri oluştur
        foreach (var itemData in allItems)
        {
            GameObject obj = Instantiate(shopItemPrefab, contentParent);
            ShopItemUI itemUI = obj.GetComponent<ShopItemUI>();
            itemUI.Setup(itemData); 
        }
    }
}