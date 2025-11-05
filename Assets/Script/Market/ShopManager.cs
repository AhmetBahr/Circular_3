using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject shopItemPrefab;
    [SerializeField] private ShopItemSO[] allItems;

    [Header("Background selection restore")]
    [SerializeField] private bool applySavedBackgroundOnStart = true;
    [SerializeField] private string idPrefix = "bg"; // ShopItemUI ile aynı prefix

    private string Key(string raw) => string.IsNullOrEmpty(idPrefix) ? raw : $"{idPrefix}:{raw}";

    private void Start()
    {
        SeedDefaultsForBackgrounds();

        LoadShop();

        if (applySavedBackgroundOnStart)
            ApplySavedBackgroundIfAny();
    }

    private void LoadShop()
    {
        // Content'i temizle
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // Tüm itemleri oluştur
        foreach (var itemData in allItems)
        {
            if (!itemData) continue;
            GameObject obj = Instantiate(shopItemPrefab, contentParent);
            ShopItemUI itemUI = obj.GetComponent<ShopItemUI>() ?? obj.GetComponentInChildren<ShopItemUI>(true);
            if (!itemUI) { Debug.LogError("Prefab'ta ShopItemUI yok.", obj); continue; }
            itemUI.Setup(itemData);
        }
    }
    
    private void SeedDefaultsForBackgrounds()
    {
        if (allItems == null) return;

        foreach (var item in allItems)
        {
            if (item == null) continue;

            if (item.unlockedByDefault)
            {
                // ProgressManager BG sahipliğini nasıl tutuyorsa ona göre id ver:
                // Eğer satın alma/owned tarafında prefix'li ID kullanıyorsan (örn: "bg:forest"):
                // var purchaseId = Key(item.itemID);

                // Eğer raw id kullanıyorsan (örn: "forest"):
                var purchaseId = item.itemID;

                if (!ProgressManager.IsItemBought(purchaseId))
                    ProgressManager.MarkItemAsBought(purchaseId);
            }
        }
    }

    private void ApplySavedBackgroundIfAny()
    {
        string selectedKey = ProgressManager.GetSelectedBackgroundId();
        if (string.IsNullOrEmpty(selectedKey)) return;

        // listedeki karşılığı bul
        var item = System.Array.Find(allItems, it => it && Key(it.itemID) == selectedKey);
        if (item == null) return;

        // sahneye uygula
        if (BackgroundManager.Instance != null)
        {
            Sprite[] newBackgrounds =
            {
                item.backGround_1,
                item.backGround_2,
                item.backGround_3,
                item.backGround_4
            };
            BackgroundManager.Instance.SetBackgrounds(newBackgrounds);
        }

        // kartların "Seçili" görsellerini yenile:
        ShopItemUI.NotifyBackgroundSelected(selectedKey);
    }
}
