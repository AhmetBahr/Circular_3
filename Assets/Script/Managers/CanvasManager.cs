using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    [Header("Main Canvas")]
    public SettingsManager settingsManager;
    [field: SerializeField] public CanvasGroup MainUpButton;
    [field: SerializeField] public CanvasGroup MainDownButton;
    [field: SerializeField] public CanvasGroup MainCenterText;
    [field: SerializeField] public TMP_Text [] playerCoinText;
    [field: SerializeField] public TMP_Text T_HighScoreText;

    [SerializeField] private GameObject centralCircular;
    [SerializeField] private Animator centralCircularAnimator;
    [SerializeField] private GameManager gameManager;
    public GameObject DeletePopup;
    
    [Header("GameMenu & PauseMenu Canvas")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameMenuGameObject;
    [SerializeField] private CanvasGroup gameMenuCanvasGroup;

    
    [Header("GameOver Canvas")]
    [field: SerializeField] public CanvasGroup GameOverCanvasGroup;
    [field: SerializeField] public Animator GameOverAnimator;
    [field: SerializeField] public GameObject NewHighScoreText;
    [field: SerializeField] public CircularProgressBar progressBar;
    
    [Header("Panels Settings")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject shopePanel;
    
    [Header("Language Settings")]
    public LanguageSelectorUI languageSelector;
    public GameObject languageChangedPopup;
    public TextMeshProUGUI popupText;
    
    private void OnEnable()
    {
        ProgressManager.OnCoinsChanged += UpdateCoinUI;
        UpdateCoinUI(ProgressManager.GetPlayerCoin()); // ilk değer
        var progress = ProgressManager.LoadProgress();
    }

    private void OnDisable()
    {
        ProgressManager.OnCoinsChanged -= UpdateCoinUI;
    }

    
    private void Start()
    {
        Invoke(nameof(startGameCanvas),1.5f);
        NewHighScoreText.SetActive(false);
        playerCoinText[0].text = gameManager.playercoin.ToString();
        playerCoinText[1].text = gameManager.playercoin.ToString();
        T_HighScoreText.text = gameManager.highScore.ToString(); 
        gameMenuGameObject.SetActive(false);

        int currentCoin = ProgressManager.GetPlayerCoin();
        UpdateCoinUI(currentCoin);
    }

    private void startGameCanvas()
    {
        StartCoroutine(FadeInCanvas(MainUpButton, 1f)); 
        StartCoroutine(FadeInCanvas(MainDownButton, 1f)); 
        StartCoroutine(FadeInCanvas(MainCenterText, 1f)); 
        
    }

    public void GameStartPanelOff()
    {
        StartCoroutine(FadeOutCanvas(MainUpButton, 1f)); 
        StartCoroutine(FadeOutCanvas(MainDownButton, 1f)); 
        StartCoroutine(FadeOutCanvas(MainCenterText, 1f));
        
        gameMenuGameObject.SetActive(true);
        StartCoroutine(FadeInCanvas(gameMenuCanvasGroup, 1f));
    }

    public void GameOverPanelOn()
    {
        NewHighScoreText.SetActive(gameManager.MainScore > gameManager.highScore);
        progressBar.isActive = true;
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
        Invoke(nameof(resartScene), 0.5f);   
        Time.timeScale = 1;
        //centralCircularAnimator.SetTrigger("endgame");
    }

    public void PauseMenuOpen()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        gameMenuGameObject.SetActive(false);
    }

    public void PauseMenuClose()
    {
        Time.timeScale = 1; //Daha sonra pause kapanınca geri sayım yapıcaz
        pauseMenu.SetActive(false);
        gameMenuGameObject.SetActive(true);
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
    
    public void UpdateCoinUI(int currentCoin)
    {
        playerCoinText[0].text  = currentCoin.ToString();
        playerCoinText[1].text  = currentCoin.ToString();
    }

}
