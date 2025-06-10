using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DoTween kullanmak için bu using direktifini ekle

public enum EnemyDirection
{
    Down,            // Düz aşağı
    Up,              // Düz yukarı
    LeftDown,        // Sol aşağı çapraz
    RightDown,       // Sağ aşağı çapraz
    LeftUp,          // Sol yukarı çapraz
    RightUp          // Sağ yukarı çapraz
}


public class EnemyController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
     
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float minTurnDelay = 1f;
    [SerializeField] private float maxTurnDelay = 3f;
    [SerializeField] private float lifeTime = 8f;

    [SerializeField] private Rigidbody2D rb2;
    [SerializeField] private Collider2D col;
    [SerializeField] private SpriteRenderer spRender;
    [SerializeField] private Transform visualsChild; // EnemyBody objesinin referansı
    
    private Vector3 _moveDirection;
    private float _turnTimer;
    private float _nextTurnTime;
    private float _lifeTimer;

    public EnemyDirection enemyDirection;
    
    [SerializeField] private float minRotationDelay = 3f; 
    [SerializeField] private float maxRotationDelay = 13f; 
    private float _rotationTimer;
    private float _nextRotationTime;

    [SerializeField] private float rotationDuration = 0.5f; 


    void OnEnable()
    {
        _lifeTimer = 0f;
        _turnTimer = 0f;
        _moveDirection = GetDirectionVector(enemyDirection);
        
        ScheduleNextTurn();
        ScheduleNextRotation();
        
        if (visualsChild != null) 
        {
            
            visualsChild.DOKill(true); 
            visualsChild.localRotation = Quaternion.identity;
        }
    }

    void OnDisable()
    {
        _lifeTimer = 0f;
       
        if (visualsChild != null) 
        {
            visualsChild.DOKill(true);
            visualsChild.localRotation = Quaternion.identity;
        }
    }

    private void Awake()
    {
        var gameManagerObject = GameObject.Find("Player");
        if (gameManagerObject != null) // Null kontrolü ekle
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
                Debug.LogError("Enemy objesinin altında 'EnemyBoudy' adında bir çocuk obje bulunamadı veya Inspector'dan atanmadı!");
                return;
            }
        }

       
        rb2 = visualsChild.GetComponent<Rigidbody2D>();
        col = visualsChild.GetComponent<Collider2D>();
        spRender = visualsChild.GetComponent<SpriteRenderer>();
        
        if (rb2 == null) Debug.LogError("Rigidbody2D 'EnemyBoudy' objesi üzerinde bulunamadı!");
        if (col == null) Debug.LogError("Collider2D 'EnemyBoudy' objesi üzerinde bulunamadı!");
        if (spRender == null) Debug.LogError("SpriteRenderer 'EnemyBoudy' objesi üzerinde bulunamadı!");
    }
    
    void Update()
    {
        // Hareket
        transform.position += _moveDirection * moveSpeed * Time.deltaTime;

        // Rastgele rotasyon zamanlayıcısı
        _rotationTimer += Time.deltaTime;
        if (_rotationTimer >= _nextRotationTime)
        {
            ApplyRandomRotation();
            ScheduleNextRotation();
        }

        // Yaşam süresi
        _lifeTimer += Time.deltaTime;
        if (_lifeTimer >= lifeTime)
        {
            gameObject.SetActive(false);
        }
    }

    void ScheduleNextTurn()
    {
        _turnTimer = 0f;
        _nextTurnTime = Random.Range(minTurnDelay, maxTurnDelay);
    }
    
    void ScheduleNextRotation()
    {
        _rotationTimer = 0f;
        _nextRotationTime = Random.Range(minRotationDelay, maxRotationDelay);
    }

    void ApplyRandomRotation()
    {
        if (visualsChild == null) return; // Null kontrolü
        
        float[] rotationAngles = { -90f, 90f, 180f };
        float randomAngle = rotationAngles[Random.Range(0, rotationAngles.Length)];

        // DoTween ile yumuşak rotasyon
        visualsChild.DOLocalRotate(new Vector3(0, 0, randomAngle), rotationDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutQuad); 
    }

    private Vector3 GetDirectionVector(EnemyDirection dir)
    {
        switch (dir)
        {
            case EnemyDirection.RightDown:
                return new Vector3(1, -1, 0).normalized;
            case EnemyDirection.LeftDown:
                return new Vector3(-1, -1, 0).normalized;
            case EnemyDirection.Down:
                return Vector3.down;
            case EnemyDirection.Up:
                return Vector3.up;
            case EnemyDirection.LeftUp:
                return new Vector3(-1, 1, 0).normalized;
            case EnemyDirection.RightUp:
                return new Vector3(1, 1, 0).normalized;
            default:
                return Vector3.down;
        }
    }


    public void HandleTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerController.PlayerDeath();
            gameObject.SetActive(false);
        }
    }
}