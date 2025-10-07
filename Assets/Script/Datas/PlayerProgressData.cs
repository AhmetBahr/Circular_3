using System;

[Serializable]
public class PlayerProgressData
{
    public int highScore = 0;
    public int playercoin = 0;
    public int interstitialCounter;

    public bool isDarkTheme = false;
    public bool isHardCore = false;

    // Eski alan (geri uyum için tutuyoruz)
    public bool isSoundOpen = true;   // <- isSfxOpen yerine eski projede kullanıldıysa bozulmasın

    // YENİ: ayrı anahtarlar
    public bool isMusicOpen = true;
    public bool isSfxOpen = true;
    public bool isVibrationOn = true;

    public string selectedLanguage = "EN";
    public string[] boughtItems = new string[0];

    // Skin mağazası için
    public string selectedSkinId = "";
    public string selectedBackgroundId = "";
}