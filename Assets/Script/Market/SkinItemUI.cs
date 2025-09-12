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

    // state & callbacks
    private PlayerSkinSO _data;
    private bool _owned;
    private bool _selected;
    private Func<PlayerSkinSO,bool> _onPurchase;
    private Action<PlayerSkinSO> _onSelect;

    public void Setup(PlayerSkinSO data, bool owned, bool selected,
                      Func<PlayerSkinSO,bool> onPurchase,
                      Action<PlayerSkinSO> onSelect)
    {
        _data = data; _owned = owned; _selected = selected;
        _onPurchase = onPurchase; _onSelect = onSelect;

        if (itemNameText) itemNameText.text = data.displayName;
        if (iconImage) iconImage.sprite = data.icon;

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnClickBuy);

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnClickSelect);

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
        if (buyButton)      buyButton.gameObject.SetActive(!_owned);
        if (buyPriceText)   buyPriceText.text = _data ? _data.price.ToString() : "";

        if (selectButton)   selectButton.interactable = _owned;
        if (selectBtnText)  selectBtnText.text = _selected ? "Seçili" : "Seç";
        if (selectBtnBg)    selectBtnBg.color = _selected ? selectedBg : normalBg;
    }
}
