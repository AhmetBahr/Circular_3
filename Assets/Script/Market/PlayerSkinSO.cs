using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Player Skin (Blend Index)")]
public class PlayerSkinSO : ScriptableObject
{
    [Header("Store/Display")]
    public string skinId;          // benzersiz olmalı
    public string displayName;
    public Sprite icon;
    public int price = 100;

    [Header("Animator Blend")]
    [Range(0, 10)] public int blendIndex = 0; // 0..10 eşiklerine karşılık
}