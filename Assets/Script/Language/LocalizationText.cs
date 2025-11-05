using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizationText : MonoBehaviour
{
    [Tooltip("Texts_Script içindeki anahtar")]
    public string key;

    private TextMeshProUGUI _tmp;

    private void Awake()
    {
        if (_tmp == null) _tmp = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        LanguageManager.OnLanguageChanged += Apply;   // dili dinle
        Apply();
    }

    private void OnDisable()
    {
        LanguageManager.OnLanguageChanged -= Apply;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (_tmp == null) _tmp = GetComponent<TextMeshProUGUI>();
            Apply();
        }
    }
#endif

    private void Apply()
    {
        if (_tmp == null) return;
        _tmp.text = Texts_Script.Get(key);
    }
}