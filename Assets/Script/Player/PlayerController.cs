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

    
    private void Start()
    {
        currentSpeed = baseSpeed;
        currentStamina = staminaMax;
        startGameButton.SetActive(false);
        
        staminaSlider.maxValue = staminaMax;
        staminaSlider.value = currentStamina;
    }

    private void Update()
    {
        HandleInput();
        HandleStamina();
        MovePlayer();
        
        Invoke(nameof(ActivateStartButton), 2.2f);
       //set active on 
    }
    
    private void ActivateStartButton()
    {
        startGameButton.SetActive(true);
    }

    // Tıklamanın yön değiştirmek için mi yoksa hızlanmak için mi olduğunu anlamak için kullandığım fonksiyon
    private void HandleInput()
    {
        // Mouse butona basıldı
        if (Input.GetMouseButtonDown(0))
        {
            clickTimer = 0f;
            isHolding = true;
        }

        // Basılı tutuluyorsa süreyi say
        if (isHolding)
        {
            clickTimer += Time.deltaTime;

            if (clickTimer >= clickThreshold && canBoost && !isBoosting && currentStamina > 0f)
            {
                isBoosting = true;
                currentSpeed = maxSpeed;
            }
        }

        // Mouse butonu bırakıldı
        if (Input.GetMouseButtonUp(0))
        {
            // Kısa tıklama ise yön değiştir
            if (clickTimer < clickThreshold)
            {
                direction *= -1;
                Vector3 scale = transform.localScale;
                scale.y *= -1;
                transform.localScale = scale;
            }

            // Boost varsa durdur
            if (isBoosting)
            {
                isBoosting = false;
                currentSpeed = baseSpeed;
                staminaRegenTimer = 0f;
                canBoost = false; // regen tamamlanmadan boost tekrar açılamasın
            }

            isHolding = false;
        }
    }


    //karakterin Stamina ve hız işlemleri
    private void HandleStamina()
    {
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

                if (staminaRegenTimer >= staminaRegenDelay)
                {
                    currentStamina += staminaRegenRate * Time.deltaTime;
                    currentStamina = Mathf.Min(currentStamina, staminaMax);

                    if (currentStamina >= staminaMax / 2f)
                    {
                        canBoost = true;
                    }
                }
            }
        }

        // UI Güncellemeleri
        staminaSlider.value = currentStamina;

        if (currentStamina >= staminaMax / 2f)
            staminaBackground.color = new Color(0.1f, 0.5f, 0.2f, 0.8f); // yeşilimsi
        else
            staminaBackground.color = new Color(0.5f, 0.1f, 0.2f, 0.5f); // kırmızımsı
    }



    //Oyunun başlatan, spawner ve canvalrı değiştirdiğim fonksiyon
    public void OnClickStartGameInput()
    {
        startGameButton.SetActive(false);
        Debug.Log("OnClickStartGameInput");
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
        Debug.Log("Player Death");
        gameOverScript.fadeIn = true;
        canvasManager.GameOverPanelOn();
        gameManager.OnGameEnd();
        gameObject.SetActive(false);
        //ToDo player objesini setactif ile kapat
        //ToDo canvasmanagerdan deathpaneli indir
        //ToDo oyunda düşman ve coin spawnını durdur ama oyun durmasın 
        //ToDo animasyon ve sesler ekle 
    }

    //karakterin genel olark hareketi main fonksiyon
    private void MovePlayer()
    {
        angle += direction * currentSpeed * Time.deltaTime;

        float x = centerPoint.position.x + Mathf.Cos(angle) * radius;
        float y = centerPoint.position.y + Mathf.Sin(angle) * radius;

        transform.position = new Vector3(x, y, transform.position.z);

        // Yönü çevir
        float lookAngle = angle * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, lookAngle);
    }

}
