using UnityEngine;

public enum SfxEvent { Pickup, Death, Swing }

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Playlist (8 parça)")]
    public AudioClip[] musicClips = new AudioClip[8];

    [Header("SFX Clips (3 durum)")]
    public AudioClip sfxPickup;
    public AudioClip sfxDeath;
    public AudioClip sfxButton;

    private int _currentTrack = 0;

    // Müzik yalnızca ilk boot'ta auto-start etsin
    private static bool _bootStartedOnce = false;

    private void Awake()
    {
        // --- Tek örnek + DDOL ---
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = false;
            musicSource.ignoreListenerPause = true;
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
        }
    }

    private void Start()
    {
        ApplyMusicEnabled(ProgressManager.GetMusicOpen());
        ApplySfxEnabled(ProgressManager.GetSfxOpen());

        if (ProgressManager.GetMusicOpen() && !_bootStartedOnce)
        {
            PlayTrack(_currentTrack);
            _bootStartedOnce = true;
        }
    }

    private void Update()
    {
        if (ProgressManager.GetMusicOpen() && musicSource.clip != null && !musicSource.isPlaying)
            NextTrack();

#if UNITY_EDITOR
        // Konsolda hangi şarkı çalıyor
        // string clipName = musicSource.clip ? musicSource.clip.name : "none";
        // string state = musicSource.isPlaying ? "▶ playing" : "⏸ stopped";
        // Debug.Log($"[Music] {clipName} ({state})");
#endif
    }

    // ---- Music ----
    public void PlayTrack(int index)
    {
        if (musicClips == null || musicClips.Length == 0) return;

        index = Mathf.Clamp(index, 0, musicClips.Length - 1);
        var next = musicClips[index];
        if (next == null) return;

        if (musicSource.isPlaying && musicSource.clip == next) return;

        _currentTrack = index;
        musicSource.clip = next;

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
        if (!enabled)
            musicSource.Stop();
        else if (!musicSource.isPlaying && musicSource.clip != null)
            musicSource.Play();
    }

    // ---- SFX ----
    public void PlaySfx(SfxEvent e)
    {
        if (!ProgressManager.GetSfxOpen()) return;

        AudioClip clip = null;
        switch (e)
        {
            case SfxEvent.Pickup: clip = sfxPickup; break;
            case SfxEvent.Death:  clip = sfxDeath;  break;
            case SfxEvent.Swing:  clip = sfxButton; break;
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
        sfxSource.mute = !enabled;
    }
}
