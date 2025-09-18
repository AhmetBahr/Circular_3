using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Object Variables")]
    [SerializeField] private CanvasManager canvasManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameOverManager gameOverScript;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private CoinSpawner  coinSpawner;
    
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
    [SerializeField] private float staminaRegenDelayHigh = 0.75f;
    [SerializeField] private float currentStamina;
    private float staminaRegenTimer = 0f;

    [Header("Speed Settings")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float baseSpeed = 1f;
    [SerializeField] private float maxSpeed = 5f;
    private bool canBoost = true;
    private bool isBoosting = false;

    private bool isHolding = false;
    private float clickTimer = 0f;
    private float clickThreshold = 0.2f;

    [Header("UI")]
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image staminaBackground;
    [SerializeField] private StaminaSegmentedUI staminaUI;

    [Header("Spin Settings (Z)")]
    [SerializeField] private float spinSpeedDegPerSec = 180f;
    private float spinAccum = 0f;

    // === YENİ: Animator & Blend Tree koruması ===
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string blendParameterName = "Blend"; // Blend Tree param adın neyse yaz
    private int blendParamHash;
    private float savedBlendParam = 0f;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        blendParamHash = Animator.StringToHash(blendParameterName);

        // 1) Devre dışı kalınca state’i koru (Unity 2020.2+)
        if (animator != null)
        {
            // derleyici eski versiyondaysa bu satır yok sayılır ama sorun değil
            animator.keepAnimatorStateOnDisable = true;
        }
        
        if (enemySpawner == null) enemySpawner = FindObjectOfType<EnemySpawner>();
        if (coinSpawner  == null) coinSpawner  = FindObjectOfType<CoinSpawner>();
    }

    private void OnEnable()
    {
        // 2) Güvenli geri yükleme: aktif olur olmaz son değeri yaz
        if (animator != null)
        {
            animator.keepAnimatorStateOnDisable = true; // tekrar teyit
            animator.SetFloat(blendParamHash, savedBlendParam);
            animator.Update(0f); // frame beklemeden grafiğe uygula
        }
    }

    private void OnDisable()
    {
        // Devre dışı kalmadan hemen önce param’ı kaydet
        if (animator != null)
        {
            savedBlendParam = animator.GetFloat(blendParamHash);
        }
    }

    private void Start()
    {
        currentSpeed = baseSpeed;
        currentStamina = staminaMax;
        startGameButton.SetActive(false);
        staminaUI?.Init(staminaMax);
        Invoke(nameof(ActivateStartButton), 2.2f);
    }

    private void Update()
    {
        HandleInput();
        HandleStamina();
        MovePlayer();
    }

    private void ActivateStartButton() => startGameButton.SetActive(true);

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
                direction *= -1;
                AudioManager.instance?.PlaySfx(SfxEvent.Swing);
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

                if (currentStamina >= staminaMax / 2f)
                {
                    if (staminaRegenTimer >= staminaRegenDelayHigh)
                    {
                        currentStamina += staminaRegenRate * Time.deltaTime;
                        currentStamina = Mathf.Min(currentStamina, staminaMax);
                        canBoost = true;
                    }
                    else canBoost = true;
                }
                else
                {
                    if (staminaRegenTimer >= staminaRegenDelay)
                    {
                        currentStamina += staminaRegenRate * Time.deltaTime;
                        currentStamina = Mathf.Min(currentStamina, staminaMax);
                        if (currentStamina >= staminaMax / 2f) canBoost = true;
                    }
                    else canBoost = false;
                }
            }
        }

        staminaUI?.UpdateUI(currentStamina);
    }

    public void OnClickStartGameInput()
    {
        startGameButton.SetActive(false);
        if (!gameManager.isGameStarted)
        {
            gameManager.MainScoreTextGameObject.SetActive(true);
            gameManager.isGameStarted = true;
            canvasManager.GameStartPanelOff();
        }
        
        enemySpawner?.StartSpawning();
        coinSpawner?.StartSpawning();
    }

    public void PlayerDeath()
    {
        // Ses & haptics
        AudioManager.instance?.PlaySfx(SfxEvent.Death);
        VibrationManager.VibrateDeath();

        // Game Over UI ve akış
        canvasManager?.GameOverPanelOn();  // fade/anim/butonlar burada yönetiliyor
        gameManager?.OnGameEnd();

        // Animator blend paramını, disable etmeden Yedekle
        if (animator != null)
            savedBlendParam = animator.GetFloat(blendParamHash);

        enemySpawner?.StopSpawning(true);
        coinSpawner?.StopSpawning(true);
        
        // Player'ı kapat
        gameObject.SetActive(false);
    }


    public void Respawn(Vector3 spawnPos)
    {
        canvasManager?.ResetGameOverUI();

        gameObject.SetActive(true);
        transform.position = spawnPos;
        angle = 0f;
        spinAccum = 0f;
        currentStamina = staminaMax;
        currentSpeed = baseSpeed;
        isBoosting = false;
        canBoost = true;
        
        enemySpawner?.StartSpawning();
        coinSpawner?.StartSpawning();

        staminaUI?.UpdateUI(currentStamina);
        StartCoroutine(RespawnProtection());
    }
    
    public void RespawnDefault()
    {
        // kendi mantığına göre bir default spawn noktası seç
        Vector3 spawnPos = centerPoint.position + new Vector3(radius, 0f, 0f);
        Respawn(spawnPos);
    }

    public void OnClickRespawn()
    {
        Respawn(new Vector3(0f, 0f, 0f));
    }

    private IEnumerator RespawnProtection()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        yield return new WaitForSeconds(3f);
        if (col != null) col.enabled = true;
    }

    private void MovePlayer()
    {
        angle += direction * currentSpeed * Time.deltaTime;
        angle = Mathf.Repeat(angle, Mathf.PI * 2f);

        float x = centerPoint.position.x + Mathf.Cos(angle) * radius;
        float y = centerPoint.position.y + Mathf.Sin(angle) * radius;
        transform.position = new Vector3(x, y, transform.position.z);

        float lookAngle = angle * Mathf.Rad2Deg;
        spinAccum += direction * spinSpeedDegPerSec * Time.deltaTime;
        spinAccum = Mathf.Repeat(spinAccum, 360f);

        float totalZ = lookAngle + spinAccum;
        transform.rotation = Quaternion.Euler(0f, 0f, totalZ);
    }
}
