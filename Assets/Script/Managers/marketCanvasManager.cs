using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

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

    [Header("Buton ve Raycast Kilidi")]
    [SerializeField] private Button shopeOpenButton;              // paneli açan buton
    [SerializeField] private GameObject shopeOpenButtonRoot;      // butonun root objesi (opsiyonel, SetActive için)
    [SerializeField] private Graphic[] extraRaycastTargetsToDisable; // Image/Text gibi raycast alanları (opsiyonel)

    private int currentIndex = -1;
    private bool _isLocked; // iç durum

    private void Start()
    {
        // Başlangıçta oyun başladıysa kilitle
        bool shouldLock = GameManager.Instance && GameManager.Instance.isGameStarted;
        SetShopOpenLock(shouldLock);
    }

    /// <summary>
    /// DIKKAT: Bu fonksiyon inspector’dan butona bağlı. Oyun başladıysa en başta geri dön.
    /// </summary>
    public void OnClickShopePanel()
    {
        // 1) KOD KORUMASI — oyun başladıysa asla açma
        if (GameManager.Instance && GameManager.Instance.isGameStarted)
            return;

        // 2) Ek güvenlik — button kilitliyse de açma
        if (_isLocked || (shopeOpenButton && !shopeOpenButton.interactable))
            return;

        if (shopePanel != null) shopePanel.SetActive(true);
        SetCanvasGroupInstant(MainUpButton,   0f, true);
        SetCanvasGroupInstant(MainDownButton, 0f, true);
        SetCanvasGroupInstant(MainCenterText, 0f, true);

        OpenMarket(defaultMarketIndex); // her açılışta varsayılan market
        
        LanguageManager.ForceNotify();
    }

    public void OnclickCloseShopePanel()
    {
        if (shopePanel != null) shopePanel.SetActive(false);
        SetCanvasGroupInstant(MainUpButton,   1f, true);
        SetCanvasGroupInstant(MainDownButton, 1f, true);
        SetCanvasGroupInstant(MainCenterText, 1f, true);
    }

    // === Oyun akışına dışarıdan çağrılabilir kilit/aç ===
    public void OnGameStarted_LockShop()   => SetShopOpenLock(true);
    public void OnGameEnded_UnlockShop()   => SetShopOpenLock(false);

    private void SetShopOpenLock(bool locked)
    {
        _isLocked = locked;

        // a) Button komponentini kilitle / aç
        if (shopeOpenButton)
        {
            shopeOpenButton.interactable = !locked;
            shopeOpenButton.enabled = !locked; // inspector onclick tetiklerini de keser
        }

        // b) Ek raycast hedeflerini kapat
        if (extraRaycastTargetsToDisable != null)
        {
            for (int i = 0; i < extraRaycastTargetsToDisable.Length; i++)
                if (extraRaycastTargetsToDisable[i] != null)
                    extraRaycastTargetsToDisable[i].raycastTarget = !locked;
        }

        // c) En kesin yöntem: tamamen gizlemek istersen (opsiyonel)
        // if (shopeOpenButtonRoot) shopeOpenButtonRoot.SetActive(!locked);
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

            float h = (i == selected) ? selectedHeight : defaultHeight;
            SetButtonHeight(tb, h);

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
