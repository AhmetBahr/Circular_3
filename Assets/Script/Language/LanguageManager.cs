using System;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;

    // UI'lerin dinleyeceği olay
    public static event Action OnLanguageChanged;

    [Header("Döngü sırası (sol/sağ bu sırada gezer)")]
    public Texts_Script.Lang[] order = new[] {
        Texts_Script.Lang.TR, Texts_Script.Lang.EN, Texts_Script.Lang.DE, Texts_Script.Lang.FR,
        Texts_Script.Lang.ES, Texts_Script.Lang.IT, Texts_Script.Lang.RU, Texts_Script.Lang.AZ
    };

    public bool IsReady { get; private set; } = true;

    private int _idx = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        // İstersen aç: sahneler arası kalıcı olsun
        // DontDestroyOnLoad(gameObject);

        // Son dili yükle (yoksa TR)
        var saved = PlayerPrefs.GetString("lang", "TR").ToUpperInvariant();
        SetLanguageByCode(saved);
    }

    // --- Dışarıdan çağrılacaklar ---
    public void NextLanguage()
    {
        if (order.Length == 0) return;
        _idx = (_idx + 1) % order.Length;
        Apply(order[_idx]);
    }

    public void PrevLanguage()
    {
        if (order.Length == 0) return;
        _idx = (_idx - 1 + order.Length) % order.Length;
        Apply(order[_idx]);
    }

    public void SetLanguageByCode(string code)
    {
        code = (code ?? "").Trim().ToUpperInvariant();
        var lang = code switch
        {
            "EN" => Texts_Script.Lang.EN,
            "DE" => Texts_Script.Lang.DE,
            "FR" => Texts_Script.Lang.FR,
            "ES" => Texts_Script.Lang.ES,
            "IT" => Texts_Script.Lang.IT,
            "RU" => Texts_Script.Lang.RU,
            "AZ" => Texts_Script.Lang.AZ,
            _    => Texts_Script.Lang.TR,
        };
        SetLanguage(lang);
    }

    public void SetLanguage(Texts_Script.Lang lang)
    {
        // order indexini hizala
        for (int i = 0; i < order.Length; i++)
            if (order[i] == lang) { _idx = i; break; }

        Apply(lang);
    }

    public string GetCurrentCode() => Texts_Script.CurrentLang.ToString(); // TR/EN/...

    // --- İç mantık ---
    private void Apply(Texts_Script.Lang lang)
    {
        if (Texts_Script.CurrentLang != lang)
        {
            Texts_Script.CurrentLang = lang;
            PlayerPrefs.SetString("lang", lang.ToString());
            PlayerPrefs.Save();
        }
        OnLanguageChanged?.Invoke(); // herkes güncellensin
    }
    
    public static void ForceNotify()
    {
        OnLanguageChanged?.Invoke();
    }

}
