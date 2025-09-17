using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Shop/Player Skin (Blend Index)")]
public class PlayerSkinSO : ScriptableObject
{
    [Header("Store/Display")]
    public string skinId;

    // Artık metnin kendisi değil, localization key tutuluyor.
    // Eski sahnelerde/assetlerde "displayName" alanı varsa otomatik taşımak için:
    [FormerlySerializedAs("displayName")]
    public string displayNameKey;

    public Sprite icon;
    public int price = 100;

    [Header("Animator Blend")]
    [Range(0, 20)] public int blendIndex = 0;
}