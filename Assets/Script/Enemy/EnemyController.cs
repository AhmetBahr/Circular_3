using UnityEngine;

public enum EnemyDirection { Down, Up, LeftDown, RightDown, LeftUp, RightUp }

public class EnemyController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform visualsChild;   // Sprite/Rigidbody/Collider burada
    [SerializeField] private Rigidbody2D rb2;          // opsiyonel (child üstünde olabilir)
    [SerializeField] private Collider2D col;           // opsiyonel (child)
    [SerializeField] private SpriteRenderer spRender;  // opsiyonel (child)

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    public EnemyDirection enemyDirection = EnemyDirection.Down;

    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 8f;

    [Header("Spin")]
    [Tooltip("Saniyede derece (degrees per second)")]
    [SerializeField] private float spinSpeedDps = 180f;
    [Tooltip("Spawn anında yön rastgele saat yönü / tersi seçilsin")]
    [SerializeField] private bool randomizeSpinDirection = true;

    private Vector3 _moveDir;
    private float _lifeTimer;
    private float _signedSpin; // +cw / -ccw

    // İstersen spawner/prefab üstünden hız override etmek için
    public void SetSpinSpeed(float dps) => spinSpeedDps = dps;

    private void Awake()
    {
        // Player referansı (etiketle bulmak daha güvenli)
        if (playerController == null)
        {
            var playerObj = GameObject.FindWithTag("Player");
            if (playerObj) playerController = playerObj.GetComponent<PlayerController>();
        }

        // Görsel/child çöz
        if (visualsChild == null)
        {
            var sr = GetComponentInChildren<SpriteRenderer>();
            if (sr) visualsChild = sr.transform;
            else visualsChild = transform; // son çare
        }

        // Bileşenleri child’dan topla
        if (rb2 == null && visualsChild) rb2 = visualsChild.GetComponent<Rigidbody2D>() ?? visualsChild.GetComponentInChildren<Rigidbody2D>();
        if (col == null && visualsChild) col = visualsChild.GetComponent<Collider2D>()     ?? visualsChild.GetComponentInChildren<Collider2D>();
        if (spRender == null && visualsChild) spRender = visualsChild.GetComponent<SpriteRenderer>() ?? visualsChild.GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _lifeTimer = 0f;
        _moveDir   = GetDir(enemyDirection);

        // Dönüş yönünü belirle
        float sign = randomizeSpinDirection ? (Random.value < 0.5f ? -1f : 1f) : 1f;
        _signedSpin = spinSpeedDps * sign;
    }

    private void Update()
    {
        // Lineer hareket
        transform.position += _moveDir * moveSpeed * Time.deltaTime;

        // Ömür
        _lifeTimer += Time.deltaTime;
        if (_lifeTimer >= lifeTime)
        {
            gameObject.SetActive(false);
            return;
        }

        // Rigidbody yoksa görseli Transform ile döndür
        if (rb2 == null && visualsChild != null)
        {
            visualsChild.Rotate(0f, 0f, _signedSpin * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        // Rigidbody varsa fizik tabanlı rotasyon
        if (rb2 != null)
        {
            rb2.MoveRotation(rb2.rotation + _signedSpin * Time.fixedDeltaTime);
        }
    }

    private static Vector3 GetDir(EnemyDirection d)
    {
        switch (d)
        {
            case EnemyDirection.RightDown: return new Vector3( 1, -1, 0).normalized;
            case EnemyDirection.LeftDown:  return new Vector3(-1, -1, 0).normalized;
            case EnemyDirection.Down:      return Vector3.down;
            case EnemyDirection.Up:        return Vector3.up;
            case EnemyDirection.LeftUp:    return new Vector3(-1,  1, 0).normalized;
            case EnemyDirection.RightUp:   return new Vector3( 1,  1, 0).normalized;
            default:                       return Vector3.down;
        }
    }

    // Çocuğun collider'ından relay ile çağırıyorsan bunu kullan
    public void HandleTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            VibrationManager.VibrateDeath();
            if (playerController) playerController.PlayerDeath();
            gameObject.SetActive(false);
        }
    }

    // Eğer relay kullanmıyorsan ve collider bu objenin altında ise:
    // private void OnTriggerEnter2D(Collider2D other) => HandleTriggerEnter2D(other);
}
