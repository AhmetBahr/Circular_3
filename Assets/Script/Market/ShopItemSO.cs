using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Item")]
public class ShopItemSO : ScriptableObject
{
    public string itemName; //item ismi 
    public string itemID; // item ID numarıs
    public int itemPrice;// itemin fiyatı 
    
    public Sprite itemIcon;
    public Sprite backGround_1; 
    public Sprite backGround_2; 
    public Sprite backGround_3;
    public Sprite backGround_4;


}
