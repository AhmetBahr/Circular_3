using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [Header("Main Canvas")]
    public SettingsManager settingsManager;
    [field: SerializeField] public CanvasGroup MainUpButton;
    [field: SerializeField] public CanvasGroup MainDownButton;
    [field: SerializeField] public CanvasGroup MainCenterText;
    [field: SerializeField] public TMP_Text[] playerCoinText;
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
    [SerializeField] private GameOverManager gameOverManager;

    [Header("Panels Settings")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Language Settings")]
    public LanguageSelectorUI languageSelector;
    public GameObject languageChangedPopup;
    public TextMeshProUGUI popupText;

    [Header("Admob Manager")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Button AdmobButton;

    [Header("Buttons")]
    [SerializeField] private Button settingsButton; // <-- AYAR BUTONU REFERANSI

    private void OnEnable()
    {
        ProgressManager.OnCoinsChanged += UpdateCoinUI;
        UpdateCoinUI(ProgressManager.GetPlayerCoin());
        var progress = ProgressManager.LoadProgress();
    }

    private void OnDisable()
    {
        ProgressManager.OnCoinsChanged -= UpdateCoinUI;
    }

    private void Start()
    {
        Invoke(nameof(startGameCanvas), 1.5f);
        NewHighScoreText.SetActive(false);

        // Run içi coin (0) gösterme yerine kalıcı coin UI güncel tutulacak.
        if (playerCoinText != null)
        {
            for (int i = 0; i < playerCoinText.Length; i++)
                if (playerCoinText[i]) playerCoinText[i].text = gameManager.playercoin.ToString();
        }

        if (T_HighScoreText) T_HighScoreText.text = gameManager.highScore.ToString();
        gameMenuGameObject.SetActive(false);

        int currentCoin = ProgressManager.GetPlayerCoin();
        UpdateCoinUI(currentCoin);

        // GameOver paneli başta kapalı ve tıklanamaz olsun
        SetCanvasGroupInstant(GameOverCanvasGroup, 0f, false);

        // Oyun başlamadan önce ayar butonu açık, oyun başlayınca kapatacağız.
        if (settingsButton) settingsButton.interactable = true;
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

        // Oyun başladı: flag + ayar butonunu kilitle
        if (GameManager.Instance) GameManager.Instance.isGameStarted = true;
        if (settingsButton) settingsButton.interactable = false;
    }

    // === GAME OVER AÇILIŞ ===
    public void GameOverPanelOn()
    {
        NewHighScoreText.SetActive(gameManager.MainScore > gameManager.highScore);
        progressBar?.RestartCooldown();
        gameOverManager?.Show(0.5f);

        // Oyun bitti: istersen ayar butonunu tekrar aç (istemezsen bu satırı sil)
        if (settingsButton) settingsButton.interactable = true;
        if (GameManager.Instance) GameManager.Instance.isGameStarted = false;
    }

    public void ResetGameOverUI()
    {
        gameOverManager?.ResetImmediate();
        NewHighScoreText?.SetActive(false);
        progressBar?.ResetImmediate();
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

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, time / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        yield return new WaitForSeconds(.5f);
    }

    public void OnClickSettingsPanel()
    {
        // Güvenlik: buton disable ise hiç açma
        if (settingsButton && !settingsButton.interactable) return;
        settingsPanel.SetActive(true);
    }

    public void OnclickCloseSettingPanel()
    {
        ProgressManager.SetSoundOpen(gameManager.isSoundOpen);

        // Dil kaydı LanguageManager içinde PlayerPrefs’e zaten yazılıyor.
        // ProgressManager.SetLanguage(languageSelector.GetSelectedLanguageCode()); // ← gerek yok

        settingsPanel.SetActive(false);
        // Burada senin akışına göre:
        // ShowMainMenu();  veya  SceneManager.LoadScene("MainMenu");
    }

    public void OnClick_AdRevive()
    {
        if (AdmobButton) AdmobButton.interactable = false;

        AdManager.Instance.ShowRewarded(
            RewardPlacement.Revive,
            onReward: () =>
            {
                player.RespawnDefault();
                progressBar?.ResetImmediate();
                if (AdmobButton) AdmobButton.interactable = true;
            },
            onUnavailable: () =>
            {
                if (AdmobButton) AdmobButton.interactable = true;
            }
        );
    }

    public void OnClick_AdRewardCoin()
    {
        if (AdmobButton) AdmobButton.interactable = false;

        AdManager.Instance.ShowRewarded(
            RewardPlacement.Coins,
            onReward: () =>
            {
                ProgressManager.AddCoins(AdManager.Instance.coinsAmount); // coin ödülü
                if (AdmobButton) AdmobButton.interactable = true;
            },
            onUnavailable: () =>
            {
                if (AdmobButton) AdmobButton.interactable = true;
            }
        );
    }

    private IEnumerator ApplyLanguageChange(string newLang)
    {
        if (languageChangedPopup != null && popupText != null)
            languageChangedPopup.SetActive(true);

        yield return StartCoroutine(LanguageManager.Instance.LoadLanguage(newLang));
        yield return new WaitForSeconds(1.8f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnClickCloseDeathPanel()
    {
        SetCanvasGroupInstant(GameOverCanvasGroup, 0f, false);
    }

    public void SetCanvasGroupInstant(CanvasGroup canvasGroup, float alpha, bool interactable)
    {
        canvasGroup.alpha = alpha;
        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = interactable;
    }

    public void OnClickRestartGame()
    {
        ProgressManager.IncrementInterstitialCounter();
        int counter = ProgressManager.GetInterstitialCounter();
        
        int freq = AdManager.Instance ? AdManager.Instance.GetInterstitialFrequency() : 3;

        if (counter >= freq && AdManager.Instance)
        {
            AdManager.Instance.TryShowInterstitial(() =>
            {
                ProgressManager.ResetInterstitialCounter();
                DoRestart(); 
            });
        }
        else
        {
            DoRestart();
        }
    }

    private void DoRestart()
    {
        Time.timeScale = 1;
        Invoke(nameof(resartScene), 0.5f);
    }

    public void PauseMenuOpen()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        gameMenuGameObject.SetActive(false);
    }

    public void PauseMenuClose()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        gameMenuGameObject.SetActive(true);
    }

    public void resartScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OpenPopUp()  => DeletePopup.SetActive(true);
    public void ClosePopUp() => DeletePopup.SetActive(false);

    public void DeleteHighScore()
    {
        ProgressManager.ResetProgress();
        SceneManager.LoadScene("GameScene");
    }

    public void UpdateCoinUI(int currentCoin)
    {
        if (playerCoinText == null) return;
        for (int i = 0; i < playerCoinText.Length; i++)
            if (playerCoinText[i]) playerCoinText[i].text = currentCoin.ToString();
    }
}
