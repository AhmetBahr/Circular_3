using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CircularProgressBar : MonoBehaviour
{
    public bool isActive = false;
    public TMP_Text TimerText;
    public Image fill;
    public float maxTime = 3f;
    public GameObject cicrularProgressBar;
    public GameObject NormalRestartButton;

    private float time;

    private void Start()
    {
        ResetImmediate();
    }

    private void Update()
    {
        if (isActive)
        {
            NormalRestartButton.SetActive(false);
            cicrularProgressBar.SetActive(true);

            time -= Time.unscaledDeltaTime; // unscaled: pause’den etkilenmez
            TimerText.text = ((int)Mathf.Ceil(time)).ToString();
            fill.fillAmount = time / maxTime;

            if (time <= 0f)
            {
                isActive = false;
                ShowNormalButton();
            }
        }
    }

    private void ShowNormalButton()
    {
        cicrularProgressBar.SetActive(false);
        NormalRestartButton.SetActive(true);
    }

    // === YENİ: revive/gameover için çağıracağınlar ===
    public void RestartCooldown()
    {
        time = maxTime;
        isActive = true;
        NormalRestartButton.SetActive(false);
        cicrularProgressBar.SetActive(true);
    }

    public void ResetImmediate()
    {
        time = maxTime;
        isActive = false;
        fill.fillAmount = 1f;

        // UI default hali
        cicrularProgressBar.SetActive(false);
        NormalRestartButton.SetActive(true);
        if (TimerText) TimerText.text = maxTime.ToString("0");
    }
}