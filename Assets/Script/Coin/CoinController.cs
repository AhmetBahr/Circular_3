using System;
using UnityEngine;

public enum CoinDirection
{
    Red,
    Orange,
    Black
}

public class CoinController : MonoBehaviour
{
    public CoinDirection coinDirection;
    public GameManager gameManager;

    [SerializeField] private float moveSpeed = 5f;

    // Pozitif: saat yönü, negatif: saat yönü tersi
    [SerializeField] private float rotateSpeed = 180f;

    [SerializeField] private float lifeTime = 5f;

    private Vector3 moveDirection;
    private float lifeTimer;

    // Her coin kendi açısını tutar; pool içinde aktif/pasif olurken
    // OnEnable'da sıfırlamıyoruz, böylece açı "sürekli artmış" görünür.
    private float zAngle = 0f;

    void OnEnable()
    {
        lifeTimer = 0f;
        moveDirection = GetDirectionVector(coinDirection);
        // zAngle'ı BİLEREK sıfırlamıyoruz: sürekli artış hissi için
    }

    void OnDisable()
    {
        lifeTimer = 0f;
        // zAngle'ı da sıfırlamıyoruz
    }

    private void Awake()
    {
        var rb2 = GetComponent<Rigidbody2D>();
        if (rb2 == null)
            rb2 = gameObject.AddComponent<Rigidbody2D>();

        rb2.bodyType = RigidbodyType2D.Kinematic;
        rb2.gravityScale = 0f;
        rb2.simulated = true;

        var gameManagerObject = GameObject.Find("GameManager");
        if (gameManagerObject) gameManager = gameManagerObject.GetComponent<GameManager>();
    }

    void Update()
    {
        // Eğer pause anında da dönsün istersen Time.unscaledDeltaTime kullan.
        float dt = Time.deltaTime;

        // Açıyı sürekli arttır
        zAngle += rotateSpeed * dt;

        // 0..360 aralığında tut (taşma birikmesin)
        zAngle = Mathf.Repeat(zAngle, 360f);

        // Z ekseni etrafında uygula
        transform.rotation = Quaternion.AngleAxis(zAngle, Vector3.forward);

        // Hareket
        transform.position += moveDirection * moveSpeed * dt;

        // Yaşam süresi
        lifeTimer += dt;
        if (lifeTimer >= lifeTime)
        {
            gameObject.SetActive(false);
        }
    }

    private Vector3 GetDirectionVector(CoinDirection dir)
    {
        switch (dir)
        {
            case CoinDirection.Red:
                return new Vector3(1, -1, 0).normalized;   // sağ-alt çapraz
            case CoinDirection.Orange:
                return new Vector3(-1, -1, 0).normalized;  // sol-alt çapraz
            case CoinDirection.Black:
                return Vector3.down;                        // düz aşağı
            default:
                return Vector3.down;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    public void Collect()
    {
        if (!gameObject.activeInHierarchy) return;

        VibrationManager.VibrateShort();
        AudioManager.instance?.PlaySfx(SfxEvent.Pickup);

        if (gameManager)
        {
            gameManager.MainScore++;
            if (gameManager.mainScoreText)
                gameManager.mainScoreText.text = gameManager.MainScore.ToString();
            gameManager.playercoin++;
        }

        // TODO: tatlı bir yok olma animasyonu ekle

        gameObject.SetActive(false);
    }
    
}
