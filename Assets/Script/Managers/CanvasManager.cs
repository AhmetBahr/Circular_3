using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    [Header("Main Canvas")]
    public SettingsManager settingsManager;
    [field: SerializeField] public CanvasGroup MainCanvasGroup;
    [field: SerializeField] public TMP_Text T_HighScoreText;
    [SerializeField] private GameObject centralCircular;
    [SerializeField] private Animator centralCircularAnimator;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TMP_Text playerCoinText;
    [field: SerializeField] public int highScore = 0;
    [field: SerializeField] public int playercoin = 0;
    public GameObject DeletePopup;
    
    [Header("GameOver Canvas")]
    [field: SerializeField] public CanvasGroup GameOverCanvasGroup;
    [field: SerializeField] public Animator GameOverAnimator;
    [field: SerializeField] public GameObject NewHighScoreText;

    [Header("Panels Settings")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject shopePanel;
    
    [Header("Language Settings")]
    public LanguageSelectorUI languageSelector;
    public GameObject languageChangedPopup;
    public TextMeshProUGUI popupText;
    
    private void OnEnable()
    {
        var progress = ProgressManager.LoadProgress();

        highScore = progress.highScore;
        playercoin = progress.playercoin;
        
    }
    
    private void Start()
    {
        Invoke(nameof(startGameCanvas),1.5f);
        NewHighScoreText.SetActive(false);
        playerCoinText.text = playercoin.ToString();
        T_HighScoreText.text = highScore.ToString(); 

            
    }

    private void startGameCanvas()
    {
        StartCoroutine(FadeInCanvas(MainCanvasGroup, 1f)); 
    }

    public void GameStartPanelOff()
    {
        StartCoroutine(FadeOutCanvas(MainCanvasGroup, 1f)); 
    }

    public void GameOverPanelOn()
    {
        StartCoroutine(FadeInCanvas(GameOverCanvasGroup, 1f)); 
        NewHighScoreText.SetActive(gameManager.MainScore > gameManager.highScore);
        GameOverAnimator.SetTrigger("gameover");
    }
    
    private IEnumerator FadeOutCanvas(CanvasGroup canvasGroup, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    
    private IEnumerator FadeInCanvas(CanvasGroup canvasGroup, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        // Etkileşimleri aç
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, time / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(.5f);
   
    }

    public void OnClickSettingsPanel()
    {
        settingsPanel.SetActive(true);
        
        if (gameManager.isSoundOpen)
        {
            settingsManager.SoundSlider.value = 1;
            settingsManager.SoundSliderToogle.sliderValue = 1;
            settingsManager.SoundSliderToogle.CurrentValue = true;
        }
        else
        {
            settingsManager.SoundSlider.value = 0;
            settingsManager.SoundSliderToogle.sliderValue = 0;
            settingsManager.SoundSliderToogle.CurrentValue = false;

        }
    }

    public void OnclickCloseSettingPanel()
    {
        ProgressManager.SetSoundOpen(gameManager.isSoundOpen);
        ProgressManager.SetLanguage(languageSelector.GetSelectedLanguageCode());

        if (languageSelector.LanguageChanged())
        {
            StartCoroutine(ApplyLanguageChange(languageSelector.GetSelectedLanguageCode()));
        }
        else
        {
            settingsPanel.SetActive(false);
        }
    }
    
    private IEnumerator ApplyLanguageChange(string newLang)
    {
        if (languageChangedPopup != null && popupText != null)
        {
            // popupText.text = "Dil değiştiriliyor...";
            languageChangedPopup.SetActive(true);
        }

        yield return StartCoroutine(LanguageManager.Instance.LoadLanguage(newLang));
        yield return new WaitForSeconds(1.8f); 
        

        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnClickShopePanel()
    {
        shopePanel.SetActive(true);
    }
    
    public void OnclickCloseShopePanel()
    {
        shopePanel.SetActive(false);
    }

    public void OnClickRestartGame()
    {
        Invoke(nameof(resartScene), 1.5f);   
        centralCircularAnimator.SetTrigger("endgame");
    }

    public void resartScene()
    {
        SceneManager.LoadScene("GameScene");
    }
    
    public void OpenPopUp()
    {
        DeletePopup.SetActive(true);
    }

    public void ClosePopUp()
    {
        DeletePopup.SetActive(false);
    }
    
    public void DeleteHighScore()
    {
        ProgressManager.ResetProgress();
        SceneManager.LoadScene("GameScene");
        
    }

}
