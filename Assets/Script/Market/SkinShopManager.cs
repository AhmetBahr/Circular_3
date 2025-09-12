using System.Collections.Generic;
using UnityEngine;

public class SkinShopManager : MonoBehaviour
{
    [SerializeField] private Transform contentParent;     // ScrollView/Content
    [SerializeField] private GameObject skinItemPrefab;   // kökte SkinItemUI olmalı
    [SerializeField] private PlayerSkinSO[] allSkins;

    [Header("Scene refs")]
    [SerializeField] private PlayerSkinApplier playerSkinApplier; // sahnedeki Player

    private Dictionary<string, SkinItemUI> _items = new Dictionary<string, SkinItemUI>();
    private string _selectedId = "";

    void Start()
    {
        // PlayerSkinApplier sahnede bulun
        if (!playerSkinApplier)
        {
            var tagged = GameObject.FindGameObjectWithTag("Player");
            if (tagged) playerSkinApplier = tagged.GetComponent<PlayerSkinApplier>();
            if (!playerSkinApplier) playerSkinApplier = FindObjectOfType<PlayerSkinApplier>();
        }

        BuildList();

        // kayıtlı seçili skin varsa SAHNEDE ANINDA uygula
        _selectedId = ProgressManager.GetSelectedSkinId();
        if (!string.IsNullOrEmpty(_selectedId))
        {
            var skin = System.Array.Find(allSkins, s => s != null && s.skinId == _selectedId);
            if (skin != null && playerSkinApplier) playerSkinApplier.ApplySkin(skin);
        }

        RefreshSelectionUI();
    }

    void BuildList()
    {
        // Guard'lar
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

        // 1) coin düş (başarısızsa false)
        if (!ProgressManager.SpendCoins(skin.price))
            return false;

        // 2) sahiplik kaydet (ANINDA)
        ProgressManager.MarkItemAsBought(skin.skinId);

        // Coin UI'si ProgressManager event'iyle güncellenecek (B bölümüne bak)
        return true;
    }

    void OnSelect(PlayerSkinSO skin)
    {
        if (skin == null) return;
        if (!ProgressManager.IsItemBought(skin.skinId)) return;

        // 1) sahnede HEMEN uygula
        if (playerSkinApplier) playerSkinApplier.ApplySkin(skin);

        // 2) kaydet ve UI'da tek seçiliyi güncelle
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
