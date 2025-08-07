using System;

[Serializable]
public class PlayerProgressData
{
    public int highScore = 0;
    public int playercoin = 0;

    public bool isDarkTheme = false;
    public bool isHardCore = false;
    public bool isSoundOpen = true;

    public string selectedLanguage = "EN";
    
    public string[] boughtItems = new string[0];
}