using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    public static bool isVibrationOn = true; // default açık

#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject unityActivity;
    private static AndroidJavaObject vibrator;

    static VibrationManager()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = unityActivity.Call<AndroidJavaObject>("getApplicationContext");
            vibrator = context.Call<AndroidJavaObject>("getSystemService", "vibrator");
        }
    }
#endif

    /// <summary>
    /// Basit titreşim (ms cinsinden).
    /// </summary>
    public static void Vibrate(long milliseconds)
    {
        if (!isVibrationOn) return; // 🔹 kapalıysa hiçbir şey yapma

#if UNITY_ANDROID && !UNITY_EDITOR
        if (vibrator != null)
        {
            vibrator.Call("vibrate", milliseconds);
        }
#elif UNITY_IOS && !UNITY_EDITOR
        Handheld.Vibrate();
#else
        //Debug.Log("Vibration not supported in editor.");
#endif
    }

    /// <summary>
    /// Kısa titreşim (ör: doğru item aldığında).
    /// </summary>
    public static void VibrateShort()
    {
        if (!isVibrationOn) return;
        Debug.Log("Kısa Titreşim");
        Vibrate(50);
    }

    /// <summary>
    /// Uzun titreşim (ör: öldüğünde).
    /// </summary>
    public static void VibrateDeath()
    {
        if (!isVibrationOn) return;
        Debug.Log("Uzun Titreşim");
        Vibrate(150);
    }

    /// <summary>
    /// PlayerPrefs’ten yükle.
    /// </summary>
    public static void LoadSettings()
    {
        isVibrationOn = PlayerPrefs.GetInt("VibrationMode", 1) == 1;
    }

    /// <summary>
    /// Ayarı kaydet.
    /// </summary>
    public static void SaveSettings()
    {
        PlayerPrefs.SetInt("VibrationMode", isVibrationOn ? 1 : 0);
    }
}
