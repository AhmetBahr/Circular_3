using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [Header("Toggles")]
    public ToggleSwitch MusicToggle;
    public ToggleSwitch SfxToggle;
    public ToggleSwitch VibrationToggle;

    private void Start()
    {
        // Oyun ilk açıldığında kayıtlı değerleri uygula ve UI'ı sessizce senkronla
        ApplySavedToSystems();
        SyncUIFromSaved();
    }

    /// <summary>
    /// Kayıtlı ayarları sistemlere uygular (AudioManager & Vibration).
    /// </summary>
    private void ApplySavedToSystems()
    {
        bool musicOn = ProgressManager.GetMusicOpen();
        bool sfxOn   = ProgressManager.GetSfxOpen();
        bool vibOn   = ProgressManager.GetVibrationOpen();

        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetMusicEnabled(musicOn);
            AudioManager.instance.SetSfxEnabled(sfxOn);
        }
        VibrationManager.isVibrationOn = vibOn;
    }

    /// <summary>
    /// UI öğelerini kayıtlı değerlere göre sessizce günceller (event tetiklemez).
    /// CanvasManager panel açarken de bunu çağırabilirsin.
    /// </summary>
    public void SyncUIFromSaved()
    {
        bool musicOn = ProgressManager.GetMusicOpen();
        bool sfxOn   = ProgressManager.GetSfxOpen();
        bool vibOn   = ProgressManager.GetVibrationOpen();

        if (MusicToggle     != null) MusicToggle.SetIsOnWithoutNotify(musicOn, animate:false);
        if (SfxToggle       != null) SfxToggle.SetIsOnWithoutNotify(sfxOn,    animate:false);
        if (VibrationToggle != null) VibrationToggle.SetIsOnWithoutNotify(vibOn, animate:false);

    }

    // ===== UI Event Handlers (ToggleSwitch onToggleOn/off'tan çağır) =====
    public void OnMusicToggleOn()  => SetMusic(true);
    public void OnMusicToggleOff() => SetMusic(false);

    public void OnSfxToggleOn()    => SetSfx(true);
    public void OnSfxToggleOff()   => SetSfx(false);

    public void OnVibToggleOn()    => SetVibration(true);
    public void OnVibToggleOff()   => SetVibration(false);

    // ===== Apply methods =====
    private void SetMusic(bool isOn)
    {
        ProgressManager.SetMusicOpen(isOn);
        if (AudioManager.instance != null)
            AudioManager.instance.SetMusicEnabled(isOn);
        // UI yansıması (eski alanlar)
        if (MusicToggle != null) MusicToggle.SetIsOnWithoutNotify(isOn, animate:false);
    }

    private void SetSfx(bool isOn)
    {
        ProgressManager.SetSfxOpen(isOn);
        if (AudioManager.instance != null)
            AudioManager.instance.SetSfxEnabled(isOn);

        if (SfxToggle != null) SfxToggle.SetIsOnWithoutNotify(isOn, animate:false);
        
    }

    private void SetVibration(bool isOn)
    {
        ProgressManager.SetVibrationOpen(isOn);
        VibrationManager.isVibrationOn = isOn;

        if (VibrationToggle != null) VibrationToggle.SetIsOnWithoutNotify(isOn, animate:false);
    }
}
