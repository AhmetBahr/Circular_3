// PlayerSkinCardUI.cs
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkinCardUI : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private Image iconImage;

    [Header("Buttons")]
    [SerializeField] private Button selectButton;
    [SerializeField] private TMP_Text selectButtonText;
    [SerializeField] private Button buyButton;
    [SerializeField] private TMP_Text buyPriceText;

    private PlayerSkinSO _data;
    private bool _owned;
    private bool _selected;
    private Func<PlayerSkinSO, bool> _onPurchase;
    private Action<PlayerSkinSO> _onSelect;

    public void Setup(PlayerSkinSO data, bool owned, bool selected,
                      Func<PlayerSkinSO, bool> onPurchase,
                      Action<PlayerSkinSO> onSelect)
    {
        _data = data;
        _owned = owned;
        _selected = selected;
        _onPurchase = onPurchase;
        _onSelect = onSelect;

       // if (itemNameText) itemNameText.text = data.displayName;
        if (iconImage) iconImage.sprite = data.icon;

        // Butonları wire et
        if (buyButton)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() =>
            {
                if (_owned) return;
                bool ok = _onPurchase?.Invoke(_data) ?? false;
                if (ok)
                {
                    _owned = true;
                    RefreshButtons();
                }
            });
        }

        if (selectButton)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() =>
            {
                if (_owned) _onSelect?.Invoke(_data);
            });
        }

        RefreshButtons();
    }

    void RefreshButtons()
    {
        if (buyPriceText) buyPriceText.text = _data.price.ToString();

        if (buyButton) buyButton.gameObject.SetActive(!_owned);
        if (selectButton) selectButton.gameObject.SetActive(_owned);

        if (selectButtonText) selectButtonText.text = _selected ? "Seçili" : "Seç";
    }

    public void MarkSelected(bool selected)
    {
        _selected = selected;
        if (selectButtonText) selectButtonText.text = selected ? "Seçili" : "Seç";
    }
}
