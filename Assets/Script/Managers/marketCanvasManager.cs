using UnityEngine;
using UnityEngine.UI;

public class MarketCanvasManager : MonoBehaviour
{
    [Header("Panel & Ana UI")]
    [SerializeField] private GameObject shopePanel;
    [SerializeField] private CanvasGroup MainUpButton;
    [SerializeField] private CanvasGroup MainDownButton;
    [SerializeField] private CanvasGroup MainCenterText;

    [Header("Market Sayfaları (sırayla: Market_1, Market_2, Market_3)")]
    [SerializeField] private GameObject[] markets;
    [SerializeField] private int defaultMarketIndex = 0;

    [System.Serializable]
    public class TabButton
    {
        public RectTransform rect;  
        public RawImage icon;        
      
    }

    [Header("Alt Bar Butonları (markets ile aynı sıra)")]
    [SerializeField] private TabButton[] tabButtons;

    [Header("Görsel Ayarlar")]
    [SerializeField] private float defaultHeight = 180f;
    [SerializeField] private float selectedHeight = 150f;
    [SerializeField] private Color defaultIconColor = new Color(0, 0, 0, 1);
    [SerializeField] private Color selectedIconColor = new Color(1, 1, 1, 1);

    private int currentIndex = -1;

    // === Panel Aç/Kapat ===
    public void OnClickShopePanel()
    {
        if (shopePanel != null) shopePanel.SetActive(true);
        SetCanvasGroupInstant(MainUpButton,   0f, true);
        SetCanvasGroupInstant(MainDownButton, 0f, true);
        SetCanvasGroupInstant(MainCenterText, 0f, true);

        OpenMarket(defaultMarketIndex); // her açılışta Market_1
    }

    public void OnclickCloseShopePanel()
    {
        if (shopePanel != null) shopePanel.SetActive(false);
        SetCanvasGroupInstant(MainUpButton,   1f, true);
        SetCanvasGroupInstant(MainDownButton, 1f, true);
        SetCanvasGroupInstant(MainCenterText, 1f, true);
    }

    // === Sekme Yönetimi ===
    public void OpenMarket(int index)
    {
        if (markets == null || markets.Length == 0) return;
        if (index < 0 || index >= markets.Length) return;
        if (currentIndex == index) return;

        for (int i = 0; i < markets.Length; i++)
            if (markets[i] != null) markets[i].SetActive(i == index);

        UpdateBottomBarVisuals(index);
        currentIndex = index;
    }

    public void OpenMarket_1() => OpenMarket(0);
    public void OpenMarket_2() => OpenMarket(1);
    public void OpenMarket_3() => OpenMarket(2);

    // === Alt bar görselleri ===
    private void UpdateBottomBarVisuals(int selected)
    {
        if (tabButtons == null || tabButtons.Length == 0) return;

        for (int i = 0; i < tabButtons.Length; i++)
        {
            var tb = tabButtons[i];
            if (tb == null) continue;

            // boy
            float h = (i == selected) ? selectedHeight : defaultHeight;
            SetButtonHeight(tb, h);

            // ikon rengi
            if (tb.icon != null)
                tb.icon.color = (i == selected) ? selectedIconColor : defaultIconColor;
        }
    }

    private void SetButtonHeight(TabButton tb, float height)
    {
            var size = tb.rect.sizeDelta;
            size.y = height;
            tb.rect.sizeDelta = size;
        
    }

    // === Helpers ===
    private void SetCanvasGroupInstant(CanvasGroup cg, float alpha, bool interactable)
    {
        if (cg == null) return;
        cg.alpha = alpha;
        cg.interactable = interactable;
        cg.blocksRaycasts = interactable;
    }
}
