using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaSegmentedUI : MonoBehaviour
{
    [Serializable]
    public class Segment
    {
        public Image image;        // Segment_X (top->bottom sırayla)
        public Sprite greenSprite; // Normal renk
        public Sprite yellowSprite;// Uyarı rengi (opsiyonel)
    }

    [Header("Segments (Top -> Bottom)")]
    [SerializeField] private List<Segment> segments = new List<Segment>();

    [Header("Yellow Switch")]
    [SerializeField] private bool switchAllToYellowUnderHalf = true;
    [SerializeField] private int yellowOnlyBottomN = 0;

    [Header("Fill Settings")]
    [SerializeField] private Image.FillMethod fillMethod = Image.FillMethod.Horizontal;   // <- varsayılan: Horizontal
    [SerializeField] private Image.OriginHorizontal originHorizontal = Image.OriginHorizontal.Left; // <- varsayılan: Left
    [SerializeField] private Image.OriginVertical originVertical = Image.OriginVertical.Top;

    private float max = 100f;

    public void Init(float staminaMax)
    {
        max = Mathf.Max(1f, staminaMax);
        ApplyImageSettingsToAll();
        UpdateUI(max);
    }

    public void UpdateUI(float current)
    {
        current = Mathf.Clamp(current, 0f, max);
        float perSeg = max / segments.Count;

        for (int i = 0; i < segments.Count; i++)
        {
            var seg = segments[i];
            if (!seg.image) continue;

            float segStart = max - perSeg * (i + 1);
            float segEnd   = max - perSeg * i;

            float segVal = Mathf.InverseLerp(segStart, segEnd, current);
            segVal = Mathf.Clamp01(segVal);
            seg.image.fillAmount = segVal;

            bool shouldBeYellow = false;
            if (current <= max * 0.5f)
            {
                if (switchAllToYellowUnderHalf) shouldBeYellow = true;
                else if (yellowOnlyBottomN > 0)
                {
                    int bottomStartIndex = Mathf.Max(0, segments.Count - yellowOnlyBottomN);
                    if (i >= bottomStartIndex) shouldBeYellow = true;
                }
            }

            if (shouldBeYellow && seg.yellowSprite != null)
                seg.image.sprite = seg.yellowSprite;
            else if (seg.greenSprite != null)
                seg.image.sprite = seg.greenSprite;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Inspector’da yanlışlıkla değişse bile kilitle
        ApplyImageSettingsToAll();
    }
#endif

    private void ApplyImageSettingsToAll()
    {
        foreach (var s in segments)
        {
            if (!s.image) continue;

            s.image.type = Image.Type.Filled;
            s.image.fillMethod = fillMethod;

            // Origin, seçilen fill method’a göre ayarlanır
            if (fillMethod == Image.FillMethod.Horizontal)
                s.image.fillOrigin = (int)originHorizontal; // Left/Right
            else if (fillMethod == Image.FillMethod.Vertical)
                s.image.fillOrigin = (int)originVertical;   // Top/Bottom

            // Başlangıçta full dolu gözüksün
            s.image.fillAmount = 1f;
        }
    }
}
