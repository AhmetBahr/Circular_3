using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [field: SerializeField] private GamOverManager gameOverScript;
    [Header("Core Variables")] 
    [field: SerializeField] public int MainScore = 0;
    [field: SerializeField] public bool isGameStarted = false;
    [field: SerializeField] public int highScore = 0;
    [field: SerializeField] public int playercoin = 0;
    
    public bool isSoundOpen;

    
    [Header("Text Variables")]
    [field: SerializeField] public TMP_Text mainScoreText;
    [field: SerializeField] public GameObject MainScoreTextGameObject;
    //public TMP_Text T_ButtonCountText;
    
    private void OnEnable()
    {
        var progress = ProgressManager.LoadProgress();

        highScore = progress.highScore;
        playercoin = progress.playercoin;
        isSoundOpen = progress.isSoundOpen;
    }
    private void Start()
    {
        MainScore = 0;   
        mainScoreText.text = MainScore.ToString();
        
    }


    public void OnGameEnd()
    {
        if (MainScore > highScore)
        {
            highScore = MainScore;
            ProgressManager.SetHighScore(highScore);
            
        }
        ProgressManager.SetPlayerCoin(playercoin);
    }
    
}
