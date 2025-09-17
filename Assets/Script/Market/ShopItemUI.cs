using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ShopItemUI : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text itemNameText; // (opsiyonel) varsa ismi buraya basar

    [Header("Buy (only when not owned)")]
    [SerializeField] private GameObject buyButton;   // B_OpenButton gibi
    [SerializeField] private Button selectButton;    // Seç butonu

    [Header("Select visuals")]
    [SerializeField] private TMP_Text selectButtonText; // "Seç / Seçili"
    [SerializeField] private Image selectButtonBg;      // Seç butonunun arka planı
    [SerializeField] private Color normalBg = new(0.85f, 0.85f, 0.85f, 1f);
    [SerializeField] private Color selectedBg = new(0.20f, 0.70f, 0.60f, 1f);

    [Header("Localization Keys (UI)")]
    [Tooltip("JSON'daki 'Seç' metninin key'i (ör. 21)")]
    [SerializeField] private string selectKey = "21";
    [Tooltip("JSON'daki 'Seçili' metninin key'i (ör. 22)")]
    [SerializeField] private string selectedKey = "22";

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

        if (selectButton)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnClickSelect);
        }

        LanguageManager.OnLanguageChanged += HandleLanguageChanged;
    }

    void OnDisable()
    {
        OnBackgroundSelectedChanged -= HandleSelectedChanged;
        LanguageManager.OnLanguageChanged -= HandleLanguageChanged;
    }

    void HandleLanguageChanged()
    {
        // Dil değişince metinleri tazele
        RefreshSelectVisuals();
        RefreshNameLocalized();
    }

    public void Setup(ShopItemSO itemData)
    {
        currentItem = itemData;

        if (itemImage) itemImage.sprite = itemData.itemIcon;
        if (priceText) priceText.text = itemData.itemPrice.ToString();

        // İsim metnini lokalleştir
        RefreshNameLocalized();

        var key = Key(currentItem.itemID);
        isItemBought = ProgressManager.IsItemBought(key);
        isSelected   = ProgressManager.GetSelectedBackgroundId() == key;

        if (buyButton) buyButton.SetActive(!isItemBought);

        RefreshSelectVisuals();
    }

    void RefreshNameLocalized()
    {
        if (!itemNameText) return;

        if (currentItem != null && !string.IsNullOrEmpty(currentItem.itemNameKey))
        {
            string loc = LanguageManager.Instance
                ? LanguageManager.Instance.GetLocalizedValue(currentItem.itemNameKey)
                : currentItem.itemNameKey; // LM yoksa key göster
            itemNameText.text = loc;
        }
        else
        {
            itemNameText.text = string.Empty;
        }
    }

    // Inspector'daki Buy butonunun OnClick'ine bunu bağlayabilirsin
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

        // Coin HUD (varsa)
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

    void HandleSelectedChanged(string selectedKeyId)
    {
        if (currentItem == null) return;
        isSelected = (selectedKeyId == Key(currentItem.itemID));
        RefreshSelectVisuals();
    }

    void RefreshSelectVisuals()
    {
        if (selectButton) selectButton.interactable = isItemBought;

        if (selectButtonText)
        {
            var key = isSelected ? selectedKey : selectKey;
            string loc = LanguageManager.Instance
                ? LanguageManager.Instance.GetLocalizedValue(key)
                : key; // LM yoksa key göster
            selectButtonText.text = loc;
        }

        if (selectButtonBg) selectButtonBg.color = isSelected ? selectedBg : normalBg;
    }
}
