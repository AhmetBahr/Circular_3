using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinItemUI : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private Image iconImage;

    [Header("Select (owned)")]
    [SerializeField] private Button selectButton;
    [SerializeField] private TMP_Text selectBtnText;
    [SerializeField] private Image selectBtnBg;
    [SerializeField] private Color normalBg = new(0.85f,0.85f,0.85f,1);
    [SerializeField] private Color selectedBg = new(0.20f,0.70f,0.60f,1);

    [Header("Buy (not owned)")]
    [SerializeField] private Button buyButton;
    [SerializeField] private TMP_Text buyPriceText;

    [Header("Localization Keys (UI)")]
    [SerializeField] private string selectKey = "21";
    [SerializeField] private string selectedKey = "22";

    private PlayerSkinSO _data;
    private bool _owned;
    private bool _selected;
    private Func<PlayerSkinSO,bool> _onPurchase;
    private Action<PlayerSkinSO> _onSelect;

    private void OnEnable()
    {
        LanguageManager.OnLanguageChanged += HandleLanguageChanged;
        Refresh();
    }

    private void OnDisable()
    {
        LanguageManager.OnLanguageChanged -= HandleLanguageChanged;
    }

    private void HandleLanguageChanged()
    {
        Refresh();
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

        // Başlangıçtan açık skin ise owned/selected yap
        if (_data != null && _data.unlockedByDefault)
        {
            _owned = true;
            _selected = true;
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
            _owned = true;
            Refresh();
        }
    }

    void OnClickSelect()
    {
        if (!_owned || _data == null) return;
        _onSelect?.Invoke(_data);
        _selected = true;
        Refresh();
    }

    void Refresh()
    {
        // --- butonlar ---
        if (buyButton)    buyButton.gameObject.SetActive(!_owned);
        if (buyPriceText) buyPriceText.text = _data ? _data.price.ToString() : "";
        if (selectButton) selectButton.interactable = _owned;
        if (selectBtnBg)  selectBtnBg.color = _selected ? selectedBg : normalBg;

        // --- Localization ---
        if (itemNameText)
        {
            string nameText = "";
            if (_data != null && !string.IsNullOrEmpty(_data.displayNameKey))
            {
                // ESKİ: LanguageManager.Instance.GetLocalizedValue(_data.displayNameKey)
                nameText = Texts_Script.Get(_data.displayNameKey);
            }
            itemNameText.text = nameText;
        }

        if (selectBtnText)
        {
            var key = _selected ? selectedKey : selectKey;
            // ESKİ: LanguageManager.Instance.GetLocalizedValue(key)
            string localized = Texts_Script.Get(key);
            selectBtnText.text = localized;
        }
    }
}
