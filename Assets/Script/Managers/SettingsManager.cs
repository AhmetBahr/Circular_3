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
    
    
    public void SoundToggleOn()
    {
        //AudioManager.instance.sfxSource.mute = !AudioManager.instance.sfxSource.mute;
        AudioManager.instance.sfxSource.mute = true;
        PlayerPrefs.SetInt("SoundMode", 1);
        print("Ses Açıldı");

    }

    public void SoundToggleOff()
    {
        //AudioManager.instance.sfxSource.mute = !AudioManager.instance.sfxSource.mute;
        PlayerPrefs.SetInt("SoundMode", 0);
        AudioManager.instance.sfxSource.mute = false;
        print("Ses kapandı");
    }
}
