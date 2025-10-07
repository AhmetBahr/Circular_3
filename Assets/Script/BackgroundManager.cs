using System;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Instance;

    [Header("Sahnedeki Background Image'lar")]
    public SpriteRenderer[] backgroundImages;

    [Header("Opsiyonel: Açılışta otomatik uygula")]
    [SerializeField] private ShopItemSO[] allBackgrounds; // varsa doldur
    [SerializeField] private string idPrefix = "bg";

    private string Key(string raw) => string.IsNullOrEmpty(idPrefix) ? raw : $"{idPrefix}:{raw}";

    private void Awake()
    {
        Instance = this;

        // Eğer liste verilmişse, oyun açılır açılmaz seçili BG'yi uygula
        TryApplySavedOnAwake();
    }

    private void TryApplySavedOnAwake()
    {
        if (allBackgrounds == null || allBackgrounds.Length == 0) return;

        string selectedKey = ProgressManager.GetSelectedBackgroundId();
        if (string.IsNullOrEmpty(selectedKey)) return;

        var item = Array.Find(allBackgrounds, it => it && Key(it.itemID) == selectedKey);
        if (item == null) return;

        Sprite[] newBackgrounds =
        {
            item.backGround_1,
            item.backGround_2,
            item.backGround_3,
            item.backGround_4
        };
        SetBackgrounds(newBackgrounds);
    }

    /// <summary>
    /// Backgroundları verilen spritelerle günceller.
    /// </summary>
    public void SetBackgrounds(Sprite[] newSprites)
    {
        // Koruma: eksik sprite dizisi gelirse sessizce çık
        if (newSprites == null || newSprites.Length < 4) return;
        if (backgroundImages == null || backgroundImages.Length < 6) return;

        backgroundImages[0].sprite = newSprites[0];
        backgroundImages[1].sprite = newSprites[1];
        backgroundImages[2].sprite = newSprites[2];
        backgroundImages[3].sprite = newSprites[3];
        backgroundImages[4].sprite = newSprites[2];
        backgroundImages[5].sprite = newSprites[3];
    }
}