using System;
using System.IO;
using UnityEngine;

public static class ProgressManager
{
    private static string savePath => Path.Combine(Application.persistentDataPath, "progress.json");

    public static PlayerProgressData LoadProgress()
    {
        PlayerProgressData data;

        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            data = JsonUtility.FromJson<PlayerProgressData>(json);
        }
        else
        {
            data = new PlayerProgressData();
            SaveProgress(data);
        }

        return data;
    }

    public static void SaveProgress(PlayerProgressData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public static void ResetProgress()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
    }

    public static int GetHighScore()
    {
        return LoadProgress().highScore;
    }

    public static void SetHighScore(int coin)
    {
        var data = LoadProgress();
        data.highScore = Mathf.Max(data.highScore, coin);
        SaveProgress(data);
    }
    
    public static int GetPlayerCoin()
    {
        return LoadProgress().playercoin;
    }

    public static void SetPlayerCoin(int coin)
    {
        var data = LoadProgress();
        data.playercoin = coin; 
        SaveProgress(data);
    }

    public static int LoadPlayerCoin()
    {
        return LoadProgress().playercoin;
    }

    public static bool GetDarkTheme()
    {
        return LoadProgress().isDarkTheme;
    }

    public static void SetDarkTheme(bool value)
    {
        var data = LoadProgress();
        data.isDarkTheme = value;
        SaveProgress(data);
    }

    public static bool GetHardCore()
    {
        return LoadProgress().isHardCore;
    }

    public static void SetHardCore(bool value)
    {
        var data = LoadProgress();
        data.isHardCore = value;
        SaveProgress(data);
    }

    public static bool GetSoundOpen()
    {
        return LoadProgress().isSoundOpen;
    }

    public static void SetSoundOpen(bool value)
    {
        var data = LoadProgress();
        data.isSoundOpen = value;
        SaveProgress(data);
    }

    public static string GetLanguage()
    {
        return LoadProgress().selectedLanguage;
    }

    public static void SetLanguage(string languageCode)
    {
        var data = LoadProgress();
        data.selectedLanguage = languageCode;
        SaveProgress(data);
    }
    
    public static bool IsItemBought(string itemId)
    {
        var data = LoadProgress();
        return Array.Exists(data.boughtItems, id => id == itemId);
    }

    public static void MarkItemAsBought(string itemId)
    {
        var data = LoadProgress();
    
        if (!Array.Exists(data.boughtItems, id => id == itemId))
        {
            var list = new System.Collections.Generic.List<string>(data.boughtItems);
            list.Add(itemId);
            data.boughtItems = list.ToArray();
            SaveProgress(data);
        }
    }

    
}
