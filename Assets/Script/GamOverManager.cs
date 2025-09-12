using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamOverManager : MonoBehaviour
{
    [Header("Text Elements and Buttons")]
    public Button restartButton;
    public Button AdmobButton;
    //[SerializeField] private string key;
    
    [Header("Object References")]
    public CanvasGroup deathCanvasGroup;
    [SerializeField] private CanvasManager canvasManager;    
    
    [Header("Controller References")]
    public bool fadeIn = false;
    
    // public void setUp(int score, int hScore)
    // { 
    //     currentScoreText.text = score.ToString() +" "+ LanguageManager.Instance.GetLocalizedValue(key);
    // }
    
    private void Update()
    {
        if (fadeIn)
        {
            if (deathCanvasGroup.alpha < 1)
            {
                deathCanvasGroup.alpha += (Time.deltaTime/2);
                if (deathCanvasGroup.alpha >= 1)
                {
                    fadeIn = false;
                    canvasManager.GameOverAnimator.SetTrigger("gameover");
                    deathCanvasGroup.blocksRaycasts = true;
                    
                }
                
                restartButton.interactable = true;
                AdmobButton.interactable = true;
            }
        }
    }
}
