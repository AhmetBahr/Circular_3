using UnityEngine;

public enum SfxEvent
{
    Pickup,     // örn: coin toplama
    Death,      // örn: öldüğünde
    Swing      // örn: Player tıklaması 
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;    // Loop kapalı, parça bitince bir sonrakine geçeceğiz
    public AudioSource sfxSource;      // OneShot için

    [Header("Music Playlist (8 parça)")]
    public AudioClip[] musicClips = new AudioClip[8];

    [Header("SFX Clips (3 durum)")]
    public AudioClip sfxPickup;
    public AudioClip sfxDeath;
    public AudioClip sfxButton;

    private int _currentTrack = 0;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Kaynakları eksikse otomatik ekle
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = false; // biz elde döndüreceğiz
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
        }
    }

    void Start()
    {
        // Ayarları yükle
        ApplyMusicEnabled(ProgressManager.GetMusicOpen());
        ApplySfxEnabled(ProgressManager.GetSfxOpen());

        // Müzik açıksa başlat
        if (ProgressManager.GetMusicOpen())
        {
            PlayTrack(_currentTrack);
        }
    }

    void Update()
    {
        // Müzik açıksa ve çalma bitti ise sıradaki parçaya geç
        if (ProgressManager.GetMusicOpen() && musicSource.clip != null && !musicSource.isPlaying)
        {
            NextTrack();
        }
    }

    // ------------- Music -------------
    public void PlayTrack(int index)
    {
        if (musicClips == null || musicClips.Length == 0) return;
        index = Mathf.Clamp(index, 0, musicClips.Length - 1);
        _currentTrack = index;

        musicSource.clip = musicClips[_currentTrack];
        if (musicSource.clip == null) return;

        if (ProgressManager.GetMusicOpen())
            musicSource.Play();
    }

    public void NextTrack()
    {
        if (musicClips == null || musicClips.Length == 0) return;
        _currentTrack = (_currentTrack + 1) % musicClips.Length;
        PlayTrack(_currentTrack);
    }

    public void PrevTrack()
    {
        if (musicClips == null || musicClips.Length == 0) return;
        _currentTrack = (_currentTrack - 1 + musicClips.Length) % musicClips.Length;
        PlayTrack(_currentTrack);
    }

    public void SetMusicEnabled(bool enabled)
    {
        ProgressManager.SetMusicOpen(enabled);
        ApplyMusicEnabled(enabled);
    }

    private void ApplyMusicEnabled(bool enabled)
    {
        musicSource.mute = !enabled;
        if (enabled)
        {
            if (!musicSource.isPlaying) PlayTrack(_currentTrack);
        }
        else
        {
            // tamamen sessiz istiyorsan sadece mute yeter; durdurmak istersen:
            musicSource.Stop();
        }
    }

    // ------------- SFX -------------
    public void PlaySfx(SfxEvent e)
    {
        if (!ProgressManager.GetSfxOpen()) return;

        AudioClip clip = null;
        switch (e)
        {
            case SfxEvent.Pickup: clip = sfxPickup; break;
            case SfxEvent.Death:  clip = sfxDeath;  break;
            case SfxEvent.Swing: clip = sfxButton; break;
        }
        if (clip != null) sfxSource.PlayOneShot(clip);
    }

    public void SetSfxEnabled(bool enabled)
    {
        ProgressManager.SetSfxOpen(enabled);
        ApplySfxEnabled(enabled);
    }

    private void ApplySfxEnabled(bool enabled)
    {
        // OneShot çalmayı engellemek için kontrol PlaySfx içinde.
        // Yine de sfxSource'u mute edebiliriz:
        sfxSource.mute = !enabled;
    }
}
