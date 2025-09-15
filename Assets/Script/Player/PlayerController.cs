using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    
    [Header("Object Variables")]
    [SerializeField] private CanvasManager canvasManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GamOverManager gameOverScript;

    [Header("Core Settings")]
    [SerializeField] private Transform centerPoint;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float radius = 5f;
    [SerializeField] private GameObject startGameButton;
    
    private int direction = 1;
    private float angle = 0f;

    [Header("Stamina Settings")]
    [SerializeField] private float staminaMax = 3f;
    [SerializeField] private float staminaRegenRate = 1f;
    [SerializeField] private float staminaDrainRate = 1f;
    [SerializeField] private float staminaRegenDelay = 2f;
    [SerializeField] private float staminaRegenDelayHigh = 0.75f; // %50 üstü için
    [SerializeField] private float currentStamina;

    private float staminaRegenTimer = 0f;
    
    [Header("Speed Settings")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float baseSpeed = 1f;
    [SerializeField] private float maxSpeed = 5f;
    private bool canBoost = true;
    private bool isBoosting = false;
    
    private bool isHolding = false;
    private bool isUsingStamina = false;

    private float clickTimer = 0f;
    private float clickThreshold = 0.2f;
    
    [Header("UI")]
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image staminaBackground;
    [SerializeField] private StaminaSegmentedUI staminaUI;
    
    [Header("Spin Settings (Z)")]
    [SerializeField] private float spinSpeedDegPerSec = 180f; // X hızın (°/sn)
    private float spinAccum = 0f;
    
    private void Start()
    {
        currentSpeed = baseSpeed;
        currentStamina = staminaMax;
        startGameButton.SetActive(false);

        if (staminaUI != null) staminaUI.Init(staminaMax);
    }

    private void Update()
    {
        HandleInput();
        HandleStamina();
        MovePlayer();
        Invoke(nameof(ActivateStartButton), 2.2f);
    }
    
    private void ActivateStartButton()
    {
        startGameButton.SetActive(true);
    }

    // Tıklamanın yön değiştirmek için mi yoksa hızlanmak için mi olduğunu anlamak için kullandığım fonksiyon
     private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickTimer = 0f;
            isHolding = true;
        }

        if (isHolding)
        {
            clickTimer += Time.deltaTime;

            if (clickTimer >= clickThreshold && canBoost && !isBoosting && currentStamina > 0f)
            {
                isBoosting = true;
                currentSpeed = maxSpeed;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (clickTimer < clickThreshold)
            {
                // Yön değiştir
                direction *= -1;

                // (İsteğe bağlı) görsel flip aynı kalsın:
                Vector3 scale = transform.localScale;
               //scale.y *= -1;
                transform.localScale = scale;
                AudioManager.instance.PlaySfx(SfxEvent.Swing);
            }

            if (isBoosting)
            {
                isBoosting = false;
                currentSpeed = baseSpeed;
                staminaRegenTimer = 0f;
                canBoost = false;
            }

            isHolding = false;
        }
    }
     
    //karakterin Stamina ve hız işlemleri
    private void HandleStamina()
    {
        // --- mevcut stamina mantığın aynen kalsın ---
        if (isBoosting)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Max(currentStamina, 0f);
            if (Mathf.Approximately(currentStamina, 0f))
            {
                isBoosting = false;
                currentSpeed = baseSpeed;
                staminaRegenTimer = 0f;
                canBoost = false;
            }
        }
        else
        {
            if (currentStamina < staminaMax)
            {
                staminaRegenTimer += Time.deltaTime;

                if (currentStamina >= staminaMax / 2f)
                {
                    if (staminaRegenTimer >= staminaRegenDelayHigh)
                    {
                        currentStamina += staminaRegenRate * Time.deltaTime;
                        currentStamina = Mathf.Min(currentStamina, staminaMax);
                        canBoost = true;
                    }
                    else
                    {
                        canBoost = true;
                    }
                }
                else
                {
                    if (staminaRegenTimer >= staminaRegenDelay)
                    {
                        currentStamina += staminaRegenRate * Time.deltaTime;
                        currentStamina = Mathf.Min(currentStamina, staminaMax);
                        if (currentStamina >= staminaMax / 2f) canBoost = true;
                    }
                    else
                    {
                        canBoost = false;
                    }
                }
            }
        }

        // --- YENİ: segmented UI güncelle ---
        if (staminaUI != null)
            staminaUI.UpdateUI(currentStamina);
    }
    
    //Oyunun başlatan, spawner ve canvalrı değiştirdiğim fonksiyon
    public void OnClickStartGameInput()
    {
        startGameButton.SetActive(false);
        //Debug.Log("OnClickStartGameInput");
        if (!gameManager.isGameStarted)
        {
            gameManager.MainScoreTextGameObject.SetActive(true);
            gameManager.isGameStarted = true;
            canvasManager.GameStartPanelOff();
            //Debug.Log("Oyun başladı!");
        }
       
    }

    //Enemy objesine çarptığı zaman çalışıcak fonksiyon
    public void PlayerDeath()
    {
        // Ses & haptics
        AudioManager.instance?.PlaySfx(SfxEvent.Death);
        VibrationManager.VibrateDeath();

        // Oyun flow
        gameOverScript.fadeIn = true;
        canvasManager.GameOverPanelOn();
        gameManager.OnGameEnd();

        gameObject.SetActive(false);

        // TODO: düşman/coin spawner durdur (oyun durmadan)
        // TODO: animasyonlar
    }


    //karakterin genel olark hareketi main fonksiyon
    private void MovePlayer()
    {
        angle += direction * currentSpeed * Time.deltaTime;

        float x = centerPoint.position.x + Mathf.Cos(angle) * radius;
        float y = centerPoint.position.y + Mathf.Sin(angle) * radius;

        transform.position = new Vector3(x, y, transform.position.z);

        // Yörünge yönüne bakış
        float lookAngle = angle * Mathf.Rad2Deg;

        // Ek Z-spin: yön pozitifse +X, negatifse -X
        spinAccum += direction * spinSpeedDegPerSec * Time.deltaTime;

        // Toplam Z rotasyonu (teğet bakış + kendi etrafında spin)
        float totalZ = lookAngle + spinAccum;
        transform.rotation = Quaternion.Euler(0f, 0f, totalZ);
    }

}
