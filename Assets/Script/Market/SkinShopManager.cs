using System.Collections.Generic;
using UnityEngine;

public class SkinShopManager : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject skinItemPrefab;
    [SerializeField] private PlayerSkinSO[] allSkins;

    [Header("Scene refs")]
    [SerializeField] private PlayerSkinApplier playerSkinApplier;

    private Dictionary<string, SkinItemUI> _items = new Dictionary<string, SkinItemUI>();
    private string _selectedId = "";

    void Start()
    {
        // Player referansı bul
        if (!playerSkinApplier)
        {
            var tagged = GameObject.FindGameObjectWithTag("Player");
            if (tagged) playerSkinApplier = tagged.GetComponent<PlayerSkinApplier>();
            if (!playerSkinApplier) playerSkinApplier = FindObjectOfType<PlayerSkinApplier>();
        }

        EnsureDefaultSkinOwnedAndSelected();

        BuildList();

        // kayıtlı seçili skin varsa sahneye uygula
        _selectedId = ProgressManager.GetSelectedSkinId();
        if (!string.IsNullOrEmpty(_selectedId))
        {
            var skin = System.Array.Find(allSkins, s => s != null && s.skinId == _selectedId);
            if (skin != null && playerSkinApplier) playerSkinApplier.ApplySkin(skin);
        }

        RefreshSelectionUI();
    }

    void EnsureDefaultSkinOwnedAndSelected()
    {
        if (allSkins == null || allSkins.Length == 0) return;

        // unlockedByDefault işaretli İLK skin’i al
        PlayerSkinSO defaultSkin = null;
        foreach (var s in allSkins)
        {
            if (s != null && s.unlockedByDefault) { defaultSkin = s; break; }
        }

        if (defaultSkin != null)
        {
            // sahiplik ver (zaten sahipse tekrar yazmanın zararı yok)
            if (!ProgressManager.IsItemBought(defaultSkin.skinId))
                ProgressManager.MarkItemAsBought(defaultSkin.skinId);

            // eğer daha önce seçilmiş geçerli bir şey yoksa, onu seçili yap
            var alreadySelected = ProgressManager.GetSelectedSkinId();
            var hasValidSelection = !string.IsNullOrEmpty(alreadySelected) &&
                                    System.Array.Exists(allSkins, s => s != null && s.skinId == alreadySelected);

            if (!hasValidSelection)
                ProgressManager.SetSelectedSkinId(defaultSkin.skinId);
        }
    }

    // --- aşağısı senin mevcut kodun ---

    void BuildList()
    {
        if (contentParent == null) { Debug.LogError("[SkinShopManager] contentParent atanmadı"); return; }
        if (skinItemPrefab == null) { Debug.LogError("[SkinShopManager] skinItemPrefab atanmadı"); return; }
        if (allSkins == null) { Debug.LogError("[SkinShopManager] allSkins null"); return; }

        foreach (Transform c in contentParent) Destroy(c.gameObject);
        _items.Clear();

        foreach (var skin in allSkins)
        {
            if (skin == null) continue;

            var go = Instantiate(skinItemPrefab, contentParent);
            var ui = go.GetComponent<SkinItemUI>() ?? go.GetComponentInChildren<SkinItemUI>(true);
            if (ui == null) { Debug.LogError("Prefab'ta SkinItemUI yok.", go); continue; }

            bool owned    = ProgressManager.IsItemBought(skin.skinId);
            bool selected = ProgressManager.GetSelectedSkinId() == skin.skinId;

            ui.Setup(skin, owned, selected, OnPurchase, OnSelect);
            _items[skin.skinId] = ui;
        }
    }

    bool OnPurchase(PlayerSkinSO skin)
    {
        if (skin == null) return false;

        // ücretsiz/başlangıç skin ise coin düşmeden sahiplik ver
        if (!skin.RequiresPurchase)
        {
            if (!ProgressManager.IsItemBought(skin.skinId))
                ProgressManager.MarkItemAsBought(skin.skinId);
            return true;
        }

        if (!ProgressManager.SpendCoins(skin.price))
            return false;

        ProgressManager.MarkItemAsBought(skin.skinId);
        return true;
    }

    void OnSelect(PlayerSkinSO skin)
    {
        if (skin == null) return;
        if (!ProgressManager.IsItemBought(skin.skinId)) return;

        if (playerSkinApplier) playerSkinApplier.ApplySkin(skin);

        _selectedId = skin.skinId;
        ProgressManager.SetSelectedSkinId(_selectedId);
        RefreshSelectionUI();
    }

    void RefreshSelectionUI()
    {
        foreach (var kv in _items)
            kv.Value.MarkSelected(kv.Key == _selectedId);
    }
}
