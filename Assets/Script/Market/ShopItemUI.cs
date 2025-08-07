using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private GameObject buyButton;
    [SerializeField] private Button selectButton;

    [SerializeField] bool isItemBought = false;

    private ShopItemSO currentItem;

    private void Awake()
    {
        if (isItemBought)
            buyButton.SetActive(false);
    }

    public void Setup(ShopItemSO itemData)
    {
        currentItem = itemData;
        itemImage.sprite = itemData.itemIcon;
        priceText.text = itemData.itemPrice.ToString();
        
        isItemBought = ProgressManager.IsItemBought(currentItem.itemID);
        buyButton.SetActive(!isItemBought);
    }

    public void OnClickBuy()
    {
        int currentCoin = ProgressManager.GetPlayerCoin();

        if (currentCoin >= currentItem.itemPrice && !isItemBought)
        {
            // Coin azalt
            int newCoin = currentCoin - currentItem.itemPrice;
            ProgressManager.SetPlayerCoin(newCoin);

            // Ürünü kalıcı olarak kaydet
            ProgressManager.MarkItemAsBought(currentItem.itemID);

            // UI güncelle
            GameManager.Instance.canvasManager.UpdateCoinUI(newCoin);
            buyButton.SetActive(false);
            isItemBought = true;

            Debug.Log($"{currentItem.itemName} satın alındı! Kalan coin: {newCoin}");
        }
        else
        {
            Debug.Log("Yeterli coin yok ya da zaten satın alındı.");
        }
        Debug.Log(ProgressManager.LoadPlayerCoin());
    }


    
    public void OnClickSelect()
    {
        Debug.Log($"{currentItem.itemName} item seçildi.");

        if (BackgroundManager.Instance != null)
        {
            Sprite[] newBackgrounds = new Sprite[]
            {
                currentItem.backGround_1,
                currentItem.backGround_2,
                currentItem.backGround_3,
                currentItem.backGround_4
            };

            BackgroundManager.Instance.SetBackgrounds(newBackgrounds);
        }
    }

}