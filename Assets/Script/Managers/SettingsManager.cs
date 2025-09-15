using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    
    [Header("Options References")]
    public Slider SoundSlider;
    public ToggleSwitch SoundSliderToogle;
    public ToggleSwitch VibrationToggle; // vibrasyon için toggle referansı
    
    private void Start()
    {
        VibrationManager.LoadSettings();
        // buradan toggle UI güncellemesi yapabilirsin
    }
    
    public void SoundToggleOn()
    {
        AudioManager.instance.sfxSource.mute = true;
        PlayerPrefs.SetInt("SoundMode", 1);
        print("Ses Açıldı");
    }

    public void SoundToggleOff()
    {
        PlayerPrefs.SetInt("SoundMode", 0);
        AudioManager.instance.sfxSource.mute = false;
        print("Ses kapandı");
    }

    public void VibrationToggleOn()
    {
        VibrationManager.isVibrationOn = true;
        VibrationManager.SaveSettings();
        print("Titreşim Açıldı");
    }

    public void VibrationToggleOff()
    {
        VibrationManager.isVibrationOn = false;
        VibrationManager.SaveSettings();
        print("Titreşim Kapandı");
    }
}