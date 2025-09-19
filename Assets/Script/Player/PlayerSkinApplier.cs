using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkinApplier : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private string blendParam = "Blend";
    private int _blendHash;
    private bool _paramIsFloat = true;

    [Header("Color Targets (kullandığını doldur)")]
    [SerializeField] private SpriteRenderer spriteRenderer; // 2D
    [SerializeField] private Image uiImage;                 // UI
    [SerializeField] private Renderer[] meshRenderers;      // 3D

    [Header("İleri Ayarlar")]
    [Tooltip("Sprite'ı materyal üzerinden de renklendir (normalde KAPALI tut).")]
    [SerializeField] private bool tintSpritesViaMaterial = false;
    [Tooltip("Sprite'ı materyalden boyamıyorsan materyal rengini otomatik beyaza sıfırla.")]
    [SerializeField] private bool clearSpriteMatToWhite = true;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId     = Shader.PropertyToID("_Color");

    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        _blendHash = Animator.StringToHash(blendParam);

        if (animator != null)
        {
            foreach (var p in animator.parameters)
            {
                if (p.nameHash == _blendHash)
                {
                    _paramIsFloat = (p.type == AnimatorControllerParameterType.Float);
                    break;
                }
            }
        }
    }

    public void ApplySkin(PlayerSkinSO skin)
    {
        if (!skin) return;

        // Blend
        if (animator)
        {
            if (_paramIsFloat) animator.SetFloat(_blendHash, skin.blendIndex);
            else               animator.SetInteger(_blendHash, skin.blendIndex);
        }

        // Renk
        if (skin.overridePlayerColor)
        {
            var col = skin.GetResolvedColor();
            ApplyColor(col);
        }

        // Kaydet
        ProgressManager.SetSelectedSkinId(skin.skinId);
    }

    private void ApplyColor(Color col)
    {
        // --- SpriteRenderer: sadece sprite rengini değiştir ---
        if (spriteRenderer)
        {
            spriteRenderer.color = col;

            // Materyalden boyamıyorsak, varsa materyal rengini beyaza çek
            if (!tintSpritesViaMaterial)
            {
                var mat = spriteRenderer.material; // instance
                if (mat)
                {
                    if (mat.HasProperty(BaseColorId)) mat.SetColor(BaseColorId, Color.white);
                    else if (mat.HasProperty(ColorId)) mat.SetColor(ColorId, Color.white);
                }
            }
            else
            {
                // Bilerek materyalden de tint'lemek istersen
                var mat = spriteRenderer.material;
                if (mat)
                {
                    if (mat.HasProperty(BaseColorId)) mat.SetColor(BaseColorId, col);
                    else if (mat.HasProperty(ColorId)) mat.SetColor(ColorId, col);
                }
            }
        }

        // --- UI Image ---
        if (uiImage) uiImage.color = col;

        // --- 3D Renderer'lar: SpriteRenderer'ları atla ---
        if (meshRenderers != null)
        {
            foreach (var rend in meshRenderers)
            {
                if (!rend) continue;
                if (spriteRenderer && rend == spriteRenderer) continue; // aynısı
                if (rend is SpriteRenderer) continue;                   // her ihtimale karşı

                var mats = rend.materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    var m = mats[i];
                    if (!m) continue;

                    if (m.HasProperty(BaseColorId)) m.SetColor(BaseColorId, col);
                    else if (m.HasProperty(ColorId)) m.SetColor(ColorId, col);
                }
            }
        }
    }
}
