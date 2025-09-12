using UnityEngine;

public class PlayerSkinApplier : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string blendParam = "Blend";

    int _blendHash;

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        _blendHash = Animator.StringToHash(blendParam);
    }

    public void ApplySkin(PlayerSkinSO skin)
    {
        if (!animator || !skin) return;
        animator.SetFloat(_blendHash, skin.blendIndex); // anında değişir
        ProgressManager.SetSelectedSkinId(skin.skinId); // kaydet
    }
}