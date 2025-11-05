using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Item")]
public class ShopItemSO : ScriptableObject
{
    // Lokalleştirme key'i
    [FormerlySerializedAs("itemName")]
    public string itemNameKey;       // ör: "BG_001_NAME"

    public string itemID;            // benzersiz id (örn: "bg_default")
    public int itemPrice = 100;

    [Tooltip("Eğer true ise oyun ilk açıldığında bu arka plan sahip (owned) kabul edilir ve fiyat yok sayılır.")]
    public bool unlockedByDefault = false;

    /// <summary>UI/Shop için pratik yardımcı.</summary>
    public bool RequiresPurchase => !unlockedByDefault && itemPrice > 0;

    public Sprite itemIcon;
    public Sprite backGround_1; 
    public Sprite backGround_2; 
    public Sprite backGround_3;
    public Sprite backGround_4;
}