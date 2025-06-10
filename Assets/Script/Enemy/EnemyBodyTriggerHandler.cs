using UnityEngine;

public class EnemyBodyTriggerHandler : MonoBehaviour
{
    // Parent (EnemyController) scriptine referans
    private EnemyController _enemyController;

    void Awake()
    {
        // Parent objedeki EnemyController scriptini bul
        _enemyController = GetComponentInParent<EnemyController>();
        if (_enemyController == null)
        {
            Debug.LogError("EnemyBodyTriggerHandler, parent objesinde EnemyController bulamadı!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
      
        if (_enemyController != null)
        {
            _enemyController.HandleTriggerEnter2D(other);
        }
    }
}