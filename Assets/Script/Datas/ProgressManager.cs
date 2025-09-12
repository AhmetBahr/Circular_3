using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class ProgressManager
{
    // ---------------- Storage ----------------
    private static string savePath => Path.Combine(Application.persistentDataPath, "progress.json");
    private static PlayerProgressData _cache; // disk erişimini azalt

    private static void EnsureLoaded()
    {
        if (_cache != null) return;

        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                _cache = JsonUtility.FromJson<PlayerProgressData>(json);
            }
            catch
            {
                // bozuk dosya vb. durumda yeni veri oluştur
                _cache = new PlayerProgressData();
                SaveProgress(_cache);
            }
        }
        else
        {
            _cache = new PlayerProgressData();
            SaveProgress(_cache);
        }

        // null safe
        if (_cache.boughtItems == null) _cache.boughtItems = new string[0];
        if (_cache.selectedLanguage == null) _cache.selectedLanguage = "EN";
        if (_cache.selectedSkinId == null) _cache.selectedSkinId = "";
    }

    public static PlayerProgressData LoadProgress()
    {
        EnsureLoaded();
        return _cache;
    }

    public static void SaveProgress(PlayerProgressData data)
    {
        if (data == null) return;
        _cache = data; // cache'i güncelle
        var json = JsonUtility.ToJson(_cache, true);
        File.WriteAllText(savePath, json);
    }

    public static void ResetProgress()
    {
        _cache = new PlayerProgressData();
        SaveProgress(_cache);
        RaiseCoinsChanged(); // coin UI anında güncellensin
    }

    // ---------------- High Score ----------------
    public static int GetHighScore()
    {
        EnsureLoaded();
        return _cache.highScore;
    }

    public static void SetHighScore(int score)
    {
        EnsureLoaded();
        _cache.highScore = Mathf.Max(_cache.highScore, score);
        SaveProgress(_cache);
    }

    // ---------------- Coins (+ event) ----------------
    public static event Action<int> OnCoinsChanged;

    private static void RaiseCoinsChanged()
    {
        EnsureLoaded();
        OnCoinsChanged?.Invoke(_cache.playercoin);
    }

    public static int GetPlayerCoin()
    {
        EnsureLoaded();
        return _cache.playercoin;
    }

    // Geriye dönük uyumluluk için bırakıldı; aynı işi yapar.
    public static int LoadPlayerCoin() => GetPlayerCoin();

    public static void SetPlayerCoin(int coins)
    {
        EnsureLoaded();
        _cache.playercoin = coins;
        SaveProgress(_cache);
        RaiseCoinsChanged();
    }

    public static void AddCoins(int amount)
    {
        EnsureLoaded();
        _cache.playercoin += amount;
        SaveProgress(_cache);
        RaiseCoinsChanged();
    }

    public static bool SpendCoins(int amount)
    {
        EnsureLoaded();
        if (_cache.playercoin < amount) return false;
        _cache.playercoin -= amount;
        SaveProgress(_cache);
        RaiseCoinsChanged();
        return true;
    }

    // ---------------- Settings ----------------
    public static bool GetDarkTheme()  { EnsureLoaded(); return _cache.isDarkTheme; }
    public static void SetDarkTheme(bool v) { EnsureLoaded(); _cache.isDarkTheme = v; SaveProgress(_cache); }

    public static bool GetHardCore()   { EnsureLoaded(); return _cache.isHardCore; }
    public static void SetHardCore(bool v) { EnsureLoaded(); _cache.isHardCore = v; SaveProgress(_cache); }

    public static bool GetSoundOpen()  { EnsureLoaded(); return _cache.isSoundOpen; }
    public static void SetSoundOpen(bool v) { EnsureLoaded(); _cache.isSoundOpen = v; SaveProgress(_cache); }

    public static string GetLanguage() { EnsureLoaded(); return _cache.selectedLanguage; }
    public static void SetLanguage(string code) { EnsureLoaded(); _cache.selectedLanguage = code ?? "EN"; SaveProgress(_cache); }

    // ---------------- Shop: Ownership ----------------
    public static bool IsItemBought(string itemId)
    {
        EnsureLoaded();
        if (string.IsNullOrEmpty(itemId) || _cache.boughtItems == null) return false;
        return Array.Exists(_cache.boughtItems, id => id == itemId);
    }

    public static void MarkItemAsBought(string itemId)
    {
        EnsureLoaded();
        if (string.IsNullOrEmpty(itemId)) return;

        // zaten varsa ekleme
        if (Array.Exists(_cache.boughtItems, id => id == itemId)) return;

        var list = new List<string>(_cache.boughtItems);
        list.Add(itemId);
        _cache.boughtItems = list.ToArray();
        SaveProgress(_cache);
    }

    // ---------------- Selected Skin ----------------
    public static string GetSelectedSkinId()
    {
        EnsureLoaded();
        return _cache.selectedSkinId ?? "";
    }

    public static void SetSelectedSkinId(string skinId)
    {
        EnsureLoaded();
        _cache.selectedSkinId = skinId ?? "";
        SaveProgress(_cache);
    }
}
