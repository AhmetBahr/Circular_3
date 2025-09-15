using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Player Skin (Blend Index)")]
public class PlayerSkinSO : ScriptableObject
{
    [Header("Store/Display")]
    public string skinId;         
    public string displayName;
    public Sprite icon;
    public int price = 100;

    [Header("Animator Blend")]
    [Range(0, 20)] public int blendIndex = 0; 
}