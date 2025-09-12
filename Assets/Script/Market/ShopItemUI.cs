using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ShopItemUI : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text priceText;

    [Header("Buy (only when not owned)")]
    [SerializeField] private GameObject buyButton;   // B_OpenButton gibi
    [SerializeField] private Button selectButton;    // Seç butonu

    [Header("Select visuals")]
    [SerializeField] private TMP_Text selectButtonText; // "Seç / Seçili" yazan TMP
    [SerializeField] private Image selectButtonBg;      // Seç butonunun arka plan Image'ı
    [SerializeField] private Color normalBg = new(0.85f, 0.85f, 0.85f, 1f);
    [SerializeField] private Color selectedBg = new(0.20f, 0.70f, 0.60f, 1f);

    [Header("ID Namespace (çapraz market karışmasın)")]
    [SerializeField] private string idPrefix = "bg";   // bu liste için benzersiz: "bg"

    private ShopItemSO currentItem;
    private bool isItemBought;
    private bool isSelected;

    // Bu sınıf kendi içinde tüm kartlara "seçim değişti"yi duyuruyor
    public static event Action<string> OnBackgroundSelectedChanged;

    string Key(string raw) => string.IsNullOrEmpty(idPrefix) ? raw : $"{idPrefix}:{raw}";

    void OnEnable()
    {
        OnBackgroundSelectedChanged += HandleSelectedChanged;
        // selectButton listener (inspector'a eklemene gerek yok)
        if (selectButton)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnClickSelect);
        }
    }

    void OnDisable()
    {
        OnBackgroundSelectedChanged -= HandleSelectedChanged;
    }

    public void Setup(ShopItemSO itemData)
    {
        currentItem = itemData;

        if (itemImage) itemImage.sprite = itemData.itemIcon;
        if (priceText) priceText.text = itemData.itemPrice.ToString();

        var key = Key(currentItem.itemID);
        isItemBought = ProgressManager.IsItemBought(key);
        isSelected   = ProgressManager.GetSelectedBackgroundId() == key;

        if (buyButton) buyButton.SetActive(!isItemBought);

        RefreshSelectVisuals();
    }

    // Inspector'daki Buy butonunun OnClick'ine bunu bağlayabilirsin (ya da bir Button ekleyip burada çağır)
    public void OnClickBuy()
    {
        if (currentItem == null || isItemBought) return;

        // Coin düşür
        if (!ProgressManager.SpendCoins(currentItem.itemPrice))
        {
            Debug.Log("Yetersiz coin ya da satın alma başarısız.");
            return;
        }

        // Sahiplik kaydı
        ProgressManager.MarkItemAsBought(Key(currentItem.itemID));
        isItemBought = true;

        // UI güncelle
        if (buyButton) buyButton.SetActive(false);

        // Coin HUD (olursa)
        var cm = GameManager.Instance ? GameManager.Instance.canvasManager : null;
        if (cm) cm.UpdateCoinUI(ProgressManager.GetPlayerCoin());
    }

    public void OnClickSelect()
    {
        if (currentItem == null || !isItemBought) return;

        // Arka planları uygula (mevcut mantığın)
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

        // Persist + tüm kartlara duyur
        string key = Key(currentItem.itemID);
        ProgressManager.SetSelectedBackgroundId(key);
        OnBackgroundSelectedChanged?.Invoke(key);
    }

    void HandleSelectedChanged(string selectedKey)
    {
        if (currentItem == null) return;
        isSelected = (selectedKey == Key(currentItem.itemID));
        RefreshSelectVisuals();
    }

    void RefreshSelectVisuals()
    {
        if (selectButton)      selectButton.interactable = isItemBought;
        if (selectButtonText)  selectButtonText.text = isSelected ? "Seçili" : "Seç";
        if (selectButtonBg)    selectButtonBg.color = isSelected ? selectedBg : normalBg;
    }
}
