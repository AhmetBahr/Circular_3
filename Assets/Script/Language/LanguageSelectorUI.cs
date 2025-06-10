using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LanguageSelectorUI : MonoBehaviour
{
    public TextMeshProUGUI languageLabel;
    public GameObject languageChangedPopup;
    
    private List<string> languageCodes = new List<string> { "tr", "en", "de", "fr", "es" , "az" , "ru" , "it" };
    private int currentIndex = 0;
    private string selectedLanguageCode;
    private string previousLanguageCode;

    void Start()
    {
        previousLanguageCode = PlayerPrefs.GetString("language", "en");
        currentIndex = languageCodes.IndexOf(previousLanguageCode);
        if (currentIndex < 0) currentIndex = 0;

        selectedLanguageCode = languageCodes[currentIndex];
        UpdateLanguageLabel();
    }

    public void NextLanguage()
    {
        currentIndex = (currentIndex + 1) % languageCodes.Count;
        selectedLanguageCode = languageCodes[currentIndex];
        UpdateLanguageLabel();
    }

    public void PreviousLanguage()
    {
        currentIndex = (currentIndex - 1 + languageCodes.Count) % languageCodes.Count;
        selectedLanguageCode = languageCodes[currentIndex];
        UpdateLanguageLabel();
    }

    private void UpdateLanguageLabel()
    {
        string code = languageCodes[currentIndex];
        languageLabel.text = code.ToString(); 
    }


    public string GetSelectedLanguageCode()
    {
        return selectedLanguageCode;
    }

    public bool LanguageChanged()
    {
        return selectedLanguageCode != previousLanguageCode;
    }

    public IEnumerator ShowPopup(string message)
    {
        if (languageChangedPopup != null)
        {
            languageChangedPopup.SetActive(true);
            languageChangedPopup.GetComponentInChildren<TextMeshProUGUI>().text = message;
            yield return new WaitForSeconds(2f);
            languageChangedPopup.SetActive(false);
        }
    }
}