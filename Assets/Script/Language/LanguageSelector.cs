using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSelector : MonoBehaviour
{
    public void SetLanguage(string languageCode)
    {
        StartCoroutine(LanguageManager.Instance.LoadLanguage(languageCode));
    }
}