using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Shop/Player Skin (Blend Index)")]
public class PlayerSkinSO : ScriptableObject
{
    [Header("Store/Display")]
    public string skinId;                 // benzersiz id (örn: "skin_default_knight")

    [FormerlySerializedAs("displayName")]
    public string displayNameKey;

    public Sprite icon;

    [Tooltip("Satın alınabilir fiyat. Başlangıçtan açık ise bu yok sayılır.")]
    public int price = 100;

    [Header("Unlocking")]
    [Tooltip("Eğer true ise oyun ilk açıldığında bu skin sahip (owned) kabul edilir ve fiyat yok sayılır.")]
    public bool unlockedByDefault = false;

    /// <summary>UI/Shop için pratik yardımcı: Bu skin satın alma gerektiriyor mu?</summary>
    public bool RequiresPurchase => !unlockedByDefault && price > 0;

    [Header("Animator Blend")]
    [Range(0, 20)] public int blendIndex = 0;

    [Header("Color Override")]
    [Tooltip("Bu skin seçildiğinde player rengini değiştirmek istiyorsan işaretle.")]
    public bool overridePlayerColor = false;

    [Tooltip("Inspector'dan seçebileceğin renk (hex boşsa bu kullanılır).")]
    public Color playerColor = Color.white;

    [Tooltip("İstersen #RRGGBB veya #RRGGBBAA şeklinde hex yaz. Doluysa bu kullanılır.")]
    public string playerColorHex = "";

    /// <summary>Hex varsa onu, yoksa inspector renk değerini döndürür.</summary>
    public Color GetResolvedColor()
    {
        if (!string.IsNullOrWhiteSpace(playerColorHex) &&
            ColorUtility.TryParseHtmlString(playerColorHex, out var hexCol))
        {
            return hexCol;
        }
        return playerColor;
    }
}