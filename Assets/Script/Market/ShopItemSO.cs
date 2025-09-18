using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Item")]
public class ShopItemSO : ScriptableObject
{
    // Artık isim yerine lokalleştirme key'i tutuyoruz
    [FormerlySerializedAs("itemName")]
    public string itemNameKey; // ör: "201"

    public string itemID;      // item ID numarası
    public int itemPrice;      // itemin fiyatı 

    public Sprite itemIcon;
    public Sprite backGround_1; 
    public Sprite backGround_2; 
    public Sprite backGround_3;
    public Sprite backGround_4;
}


