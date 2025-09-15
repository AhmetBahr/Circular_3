using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    public static bool isVibrationOn = true;

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

    public static void Vibrate(long milliseconds)
    {
        if (!isVibrationOn) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        if (vibrator != null) vibrator.Call("vibrate", milliseconds);
#elif UNITY_IOS && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }

    public static void VibrateShort()  { Vibrate(50);  }
    public static void VibrateDeath()  { Vibrate(150); }

    public static void LoadSettings()
    {
        isVibrationOn = ProgressManager.GetVibrationOpen();
    }

    public static void SaveSettings()
    {
        ProgressManager.SetVibrationOpen(isVibrationOn);
    }
}