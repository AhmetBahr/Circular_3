using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [field: SerializeField] private GameOverManager gameOverScript;
    
    [Header("Core Variables")] 
    [field: SerializeField] public int MainScore = 0;
    [field: SerializeField] public bool isGameStarted = false;
    [field: SerializeField] public int highScore = 0;
    [field: SerializeField] public int playercoin = 0;
    
    public bool isSoundOpen;
    public CanvasManager canvasManager;
    
    [Header("Text Variables")]
    [field: SerializeField] public TMP_Text mainScoreText;
    [field: SerializeField] public GameObject MainScoreTextGameObject;
    
    private void Awake()
    {
        // Sağlam singleton guard
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Sahneler arası kalması gerekmiyorsa DontDestroyOnLoad koymuyoruz.
        // DontDestroyOnLoad(gameObject);
    }
    
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
        if (mainScoreText) mainScoreText.text = MainScore.ToString();
        playercoin = 0; // Bu run içinde toplanan coin
        isGameStarted = false;
    }
    
    public void OnGameEnd()
    {
        if (MainScore > highScore)
        {
            highScore = MainScore;
            ProgressManager.SetHighScore(highScore);
        }

        // Kalıcı coin güncelle
        int coinBeforeGame = ProgressManager.GetPlayerCoin();
        int coinGainedDuringGame = playercoin;
        int newTotal = coinBeforeGame + coinGainedDuringGame;
        ProgressManager.SetPlayerCoin(newTotal);
    }
}
