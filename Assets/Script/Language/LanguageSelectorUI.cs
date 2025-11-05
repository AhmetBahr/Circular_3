using UnityEngine;
using TMPro;

public class LanguageSelectorUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI languageLabel; // ortadaki TR/EN yazısı

    private void OnEnable()
    {
        LanguageManager.OnLanguageChanged += RefreshLabel;
        RefreshLabel();
    }

    private void OnDisable()
    {
        LanguageManager.OnLanguageChanged -= RefreshLabel;
    }

    public void OnRightButton()  // sağ buton
    {
        LanguageManager.Instance?.NextLanguage();
        // label event ile otomatik güncellenecek
    }

    public void OnLeftButton()   // sol buton
    {
        LanguageManager.Instance?.PrevLanguage();
    }

    private void RefreshLabel()
    {
        if (languageLabel == null || LanguageManager.Instance == null) return;
        languageLabel.text = LanguageManager.Instance.GetCurrentCode(); // TR, EN, DE...
    }
}