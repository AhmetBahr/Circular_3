using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CircularProgressBar : MonoBehaviour
{
    public bool isActive = false;
    public TMP_Text TimerText;
    public Image fill;
    public float maxTime;
    public GameObject cicrularProgressBar;
    public GameObject NormalRestartButton;
    
    public float time;

    private void Start()
    {
        time = maxTime;
    }

    private void Update()
    {
        if (isActive)
        {
            NormalRestartButton.SetActive(false);
            cicrularProgressBar.SetActive(true);
            time -= Time.deltaTime;
            TimerText.text = ""+ (int)time;
            fill.fillAmount = time / maxTime;

            if (time <= 0)
            {
                isActive = false;
            }
            
        }

        if (!isActive)
        {
            cicrularProgressBar.SetActive(false);
            NormalRestartButton.SetActive(true);
        }
    }
}