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
        // Önceden iki kere instantiate ediyordu. Sadeleştirdim.
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
