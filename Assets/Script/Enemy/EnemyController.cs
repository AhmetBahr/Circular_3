using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DOTween kalabilir ama artık rotasyon için kullanılmıyor

public enum EnemyDirection
{
    Down, Up, LeftDown, RightDown, LeftUp, RightUp
}

public class EnemyController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float minTurnDelay = 1f;
    [SerializeField] private float maxTurnDelay = 3f;

    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 8f;

    [Header("Refs")]
    [SerializeField] private Rigidbody2D rb2;
    [SerializeField] private Collider2D col;
    [SerializeField] private SpriteRenderer spRender;
    [SerializeField] private Transform visualsChild; // EnemyBody

    private Vector3 _moveDirection;
    private float _turnTimer;
    private float _nextTurnTime;
    private float _lifeTimer;

    public EnemyDirection enemyDirection;

    // === YENİ: Sürekli rotasyon ayarları ===
    [Header("Continuous Rotation")]
    [Tooltip("Saniyede derece cinsinden taban hız")]
    [SerializeField] private float baseRotationSpeed = 180f; // dps
    [Tooltip("Spawn anında yön +/- rastgele seçilsin mi?")]
    [SerializeField] private bool randomizeDirectionOnSpawn = true;

    private float _currentRotSpeed; // işleyen hız (+/–)

    // === ESKİ: rastgele aralıklı rotasyon (devre dışı) ===
    /*
    [SerializeField] private float minRotationDelay = 3f; 
    [SerializeField] private float maxRotationDelay = 13f; 
    private float _rotationTimer;
    private float _nextRotationTime;
    [SerializeField] private float rotationDuration = 0.5f; 
    */

    void OnEnable()
    {
        _lifeTimer = 0f;
        _turnTimer = 0f;
        _moveDirection = GetDirectionVector(enemyDirection);

        ScheduleNextTurn();

        // === YENİ: spawn'da dönüş yönünü ve hızı belirle ===
        float sign = randomizeDirectionOnSpawn ? (Random.value < 0.5f ? -1f : 1f) : 1f;
        _currentRotSpeed = baseRotationSpeed * sign;

        // === ESKİ: rastgele aralıklı rotasyon zamanlaması (kaldırıldı) ===
        // ScheduleNextRotation();

        if (visualsChild != null)
        {
            visualsChild.DOKill(true);
            // Artık sabit döneceği için açılı sıfırlamak zorunlu değil;
            // istersen aç: visualsChild.localRotation = Quaternion.identity;
        }
    }

    void OnDisable()
    {
        _lifeTimer = 0f;

        if (visualsChild != null)
        {
            visualsChild.DOKill(true);
            // visualsChild.localRotation = Quaternion.identity;
        }
    }

    private void Awake()
    {
        var gameManagerObject = GameObject.Find("Player");
        if (gameManagerObject != null)
        {
            playerController = gameManagerObject.GetComponent<PlayerController>();
        }
        else
        {
            Debug.LogError("Player objesi sahnede bulunamadı!");
        }

        if (visualsChild == null)
        {
            visualsChild = transform.Find("EnemyBoudy");
            if (visualsChild == null)
            {
                Debug.LogError("Altında 'EnemyBoudy' bulunamadı!");
                return;
            }
        }

        rb2 = visualsChild.GetComponent<Rigidbody2D>();
        col = visualsChild.GetComponent<Collider2D>();
        spRender = visualsChild.GetComponent<SpriteRenderer>();

        if (rb2 == null) Debug.LogWarning("Rigidbody2D 'EnemyBoudy' üzerinde yok. Rotasyon Transform ile yapılacak.");
        if (col == null) Debug.LogError("Collider2D 'EnemyBoudy' üzerinde bulunamadı!");
        if (spRender == null) Debug.LogError("SpriteRenderer 'EnemyBoudy' üzerinde bulunamadı!");
    }

    void Update()
    {
        // Hareket
        transform.position += _moveDirection * moveSpeed * Time.deltaTime;

        // === ESKİ: aralıklı rastgele rotasyon (kaldırıldı) ===
        /*
        _rotationTimer += Time.deltaTime;
        if (_rotationTimer >= _nextRotationTime)
        {
            ApplyRandomRotation();
            ScheduleNextRotation();
        }
        */

        // Yaşam süresi
        _lifeTimer += Time.deltaTime;
        if (_lifeTimer >= lifeTime)
        {
            gameObject.SetActive(false);
        }

        // Rigidbody2D yoksa görseli burada döndür
        if (rb2 == null && visualsChild != null)
        {
            visualsChild.Rotate(0f, 0f, _currentRotSpeed * Time.deltaTime);
        }
    }

    // void FixedUpdate()
    // {
    //     // Rigidbody2D varsa fiziksel rotasyon tercih edilir
    //     if (rb2 != null)
    //     {
    //         rb2.MoveRotation(rb2.rotation + _currentRotSpeed * Time.fixedDeltaTime);
    //     }
    // }

    void ScheduleNextTurn()
    {
        _turnTimer = 0f;
        _nextTurnTime = Random.Range(minTurnDelay, maxTurnDelay);
    }

    // === ESKİ: aralıklı rotasyon zamanlayıcısı (kaldırıldı) ===
    /*
    void ScheduleNextRotation()
    {
        _rotationTimer = 0f;
        _nextRotationTime = Random.Range(minRotationDelay, maxRotationDelay);
    }

    void ApplyRandomRotation()
    {
        if (visualsChild == null) return;
        float[] rotationAngles = { -90f, 90f, 180f };
        float randomAngle = rotationAngles[Random.Range(0, rotationAngles.Length)];
        visualsChild.DOLocalRotate(new Vector3(0, 0, randomAngle), rotationDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutQuad);
    }
    */

    private Vector3 GetDirectionVector(EnemyDirection dir)
    {
        switch (dir)
        {
            case EnemyDirection.RightDown: return new Vector3(1, -1, 0).normalized;
            case EnemyDirection.LeftDown:  return new Vector3(-1, -1, 0).normalized;
            case EnemyDirection.Down:      return Vector3.down;
            case EnemyDirection.Up:        return Vector3.up;
            case EnemyDirection.LeftUp:    return new Vector3(-1, 1, 0).normalized;
            case EnemyDirection.RightUp:   return new Vector3(1, 1, 0).normalized;
            default:                       return Vector3.down;
        }
    }

    public void HandleTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            VibrationManager.VibrateDeath();
            playerController.PlayerDeath();
            gameObject.SetActive(false);
        }
    }
}
