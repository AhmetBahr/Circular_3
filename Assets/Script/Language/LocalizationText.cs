using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizationText : MonoBehaviour
{
    public string key;

    private TextMeshProUGUI textComponent;

    void OnEnable()
    {
        LanguageManager.OnLanguageChanged += UpdateText;
        if (textComponent == null)
            textComponent = GetComponent<TextMeshProUGUI>();
        UpdateText();
    }

    void OnDisable()
    {
        LanguageManager.OnLanguageChanged -= UpdateText;
    }

    public void UpdateText()
    {
        if (LanguageManager.Instance != null && LanguageManager.Instance.isReady)
        {
            textComponent.text = LanguageManager.Instance.GetLocalizedValue(key);
        }
    }
}