using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinItemUI : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] private TMP_Text itemNameText;   // T_ItemName
    [SerializeField] private Image iconImage;         // küçük görsel

    [Header("Select (owned)")]
    [SerializeField] private Button selectButton;     // B_SelectButton
    [SerializeField] private TMP_Text selectBtnText;  // B_SelectButton/Text (TMP)
    [SerializeField] private Image selectBtnBg;       // B_SelectButton Image
    [SerializeField] private Color normalBg = new(0.85f,0.85f,0.85f,1);
    [SerializeField] private Color selectedBg = new(0.20f,0.70f,0.60f,1);

    [Header("Buy (not owned)")]
    [SerializeField] private Button buyButton;        // B_OpenButton
    [SerializeField] private TMP_Text buyPriceText;   // B_OpenButton/Text (TMP)

    [Header("Localization Keys (UI)")]
    [Tooltip("JSON'daki 'Seç' metninin key'i (ör. 21)")]
    [SerializeField] private string selectKey = "21";
    [Tooltip("JSON'daki 'Seçili' metninin key'i (ör. 22)")]
    [SerializeField] private string selectedKey = "22";

    // state & callbacks
    private PlayerSkinSO _data;
    private bool _owned;
    private bool _selected;
    private Func<PlayerSkinSO,bool> _onPurchase;
    private Action<PlayerSkinSO> _onSelect;

    private void OnEnable()
    {
        LanguageManager.OnLanguageChanged += HandleLanguageChanged;
    }

    private void OnDisable()
    {
        LanguageManager.OnLanguageChanged -= HandleLanguageChanged;
    }

    private void HandleLanguageChanged()
    {
        Refresh(); // dil değişince tüm metinleri tazele
    }

    public void Setup(PlayerSkinSO data, bool owned, bool selected,
                      Func<PlayerSkinSO,bool> onPurchase,
                      Action<PlayerSkinSO> onSelect)
    {
        _data = data; _owned = owned; _selected = selected;
        _onPurchase = onPurchase; _onSelect = onSelect;

        if (iconImage) iconImage.sprite = data.icon;

        if (buyButton)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnClickBuy);
        }

        if (selectButton)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnClickSelect);
        }

        Refresh();
    }

    public void MarkSelected(bool selected)
    {
        _selected = selected;
        Refresh();
    }

    void OnClickBuy()
    {
        if (_owned || _data == null) return;
        if (_onPurchase != null && _onPurchase(_data))
        {
            _owned = true;            // hemen sahip oldu
            Refresh();                // buy gizlenir, select aktif olur
        }
    }

    void OnClickSelect()
    {
        if (!_owned || _data == null) return;
        _onSelect?.Invoke(_data);     // anında uygula
        _selected = true;             // kartını seçili boya (tekilliği manager ayarlar)
        Refresh();
    }

    void Refresh()
    {
        // Fiyat ve buton görünürlükleri
        if (buyButton)      buyButton.gameObject.SetActive(!_owned);
        if (buyPriceText)   buyPriceText.text = _data ? _data.price.ToString() : "";

        if (selectButton)   selectButton.interactable = _owned;
        if (selectBtnBg)    selectBtnBg.color = _selected ? selectedBg : normalBg;

        // === Localization ===
        // 1) İsim: SO içindeki displayNameKey'ten
        if (itemNameText)
        {
            string nameText = "";
            if (_data != null && !string.IsNullOrEmpty(_data.displayNameKey))
            {
                nameText = LanguageManager.Instance
                    ? LanguageManager.Instance.GetLocalizedValue(_data.displayNameKey)
                    : _data.displayNameKey; // LM yoksa key göster
            }
            itemNameText.text = nameText;
        }

        // 2) Seç / Seçili: UI içindeki key'lerden
        if (selectBtnText)
        {
            var key = _selected ? selectedKey : selectKey;
            string localized = LanguageManager.Instance
                ? LanguageManager.Instance.GetLocalizedValue(key)
                : key; // LM yoksa key göster
            selectBtnText.text = localized;
        }
    }
}
