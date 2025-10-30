using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;

    public string currentLanguage = "en";
    private Dictionary<string, string> localizedText;
    public bool isReady = false;

    public delegate void LanguageChanged();
    public static event LanguageChanged OnLanguageChanged;

    private Coroutine _loadRoutine; // ← çakışmaları önlemek için

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // sahneler arası taşımak istersen aç

            string savedLanguage = PlayerPrefs.GetString("language", currentLanguage);
            ChangeLanguageImmediate(savedLanguage); // ← anında yükleme
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Dili anında değiştirir. Önceki yükleme devam ediyorsa iptal eder.
    /// </summary>
    public void ChangeLanguageImmediate(string languageCode)
    {
        if (_loadRoutine != null) StopCoroutine(_loadRoutine);
        _loadRoutine = StartCoroutine(LoadLanguage(languageCode));
    }

    public IEnumerator LoadLanguage(string languageCode)
    {
        isReady = false;
        string filePath = Path.Combine(Application.streamingAssetsPath, $"text_{languageCode}.json");
        string jsonContent = "";

#if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest www = UnityWebRequest.Get(filePath);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load localization file: " + www.error);
            yield break;
        }

        jsonContent = www.downloadHandler.text;
#else
        if (!File.Exists(filePath))
        {
            Debug.LogError("Localization file not found: " + filePath);
            yield break;
        }

        jsonContent = File.ReadAllText(filePath);
#endif

        LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(jsonContent);
        if (loadedData == null || loadedData.items == null)
        {
            Debug.LogError("Failed to parse localization JSON");
            yield break;
        }

        localizedText = loadedData.ToDictionary();
        isReady = true;
        currentLanguage = languageCode;
        PlayerPrefs.SetString("language", languageCode);
        OnLanguageChanged?.Invoke();
    }

    public string GetLocalizedValue(string key)
    {
        if (localizedText != null && localizedText.ContainsKey(key))
            return localizedText[key];
        return $"#{key}";
    }
}

[System.Serializable]
public class LocalizationData
{
    public LocalizationItem[] items;

    public Dictionary<string, string> ToDictionary()
    {
        var dict = new Dictionary<string, string>();
        foreach (var item in items)
        {
            dict[item.key] = item.value;
        }
        return dict;
    }
}

[System.Serializable]
public class LocalizationItem
{
    public string key;
    public string value;
}
