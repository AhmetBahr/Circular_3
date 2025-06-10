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
    [SerializeField] private float rotateSpeed = 180f;
    [SerializeField] private float lifeTime = 5f;
    
    private Vector3 moveDirection;
    private float lifeTimer;
    
    void OnEnable()
    {
        lifeTimer = 0f;
        moveDirection = GetDirectionVector(coinDirection);
    }
    
    void OnDisable()
    {
        lifeTimer = 0f;
    }

    private void Awake()
    {
        var gameManagerObject = GameObject.Find("GameManager");
        gameManager = gameManagerObject.GetComponent<GameManager>();

    }

    void Update()
    {
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        lifeTimer += Time.deltaTime;
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
                return new Vector3(1, -1, 0).normalized; // sağ-alt çapraz
            case CoinDirection.Orange:
                return new Vector3(-1, -1, 0).normalized; // sol-alt çapraz
            case CoinDirection.Black:
                return Vector3.down; // düz aşağı
            default:
                return Vector3.down;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.MainScore++;
            gameManager.mainScoreText.text = gameManager.MainScore.ToString();
            gameManager.playercoin++;
            //ToDo player objesine coin arttır 
            //ToDo ses ekle
            //ToDo tatlı bir yok olma aniamsyonu ekle 
            gameObject.SetActive(false); 
            
        }
    }
}

