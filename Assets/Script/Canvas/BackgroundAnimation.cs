using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    [SerializeField] private Transform bgA;     // SpriteRenderer taşıyan objeler
    [SerializeField] private Transform bgB;
    [SerializeField] private float moveSpeed = 5f;    // world units / sn
    [SerializeField] private float leftThresholdX = -2150f; // bu eşiğin SOLUNA tamamen çıkınca respawn

    float wA, wB;

    void Start()
    {
        // Genişlikleri bir kez ölç (scale dahil gerçek genişlik)
        wA = GetWidth(bgA);
        wB = GetWidth(bgB);
        // Başlangıçta asla “düzeltme/ışınlama” yapma — ilk frame zıplaması olmasın
    }

    void Update()
    {
        float dx = moveSpeed * Time.deltaTime;

        // ikisini de sola kaydır
        bgA.position += Vector3.left * dx;
        bgB.position += Vector3.left * dx;

        // A tamamen sol eşiğin dışına çıktıysa, B'nin sağına al
        if (Right(bgA, wA) <= leftThresholdX)
        {
            float bRight = Right(bgB, wB);
            bgA.position = new Vector3(bRight + wA * 0.5f, bgA.position.y, bgA.position.z);
        }

        // B tamamen sol eşiğin dışına çıktıysa, A'nın sağına al
        if (Right(bgB, wB) <= leftThresholdX)
        {
            float aRight = Right(bgA, wA);
            bgB.position = new Vector3(aRight + wB * 0.5f, bgB.position.y, bgB.position.z);
        }
    }

    // yardımcılar
    float GetWidth(Transform t)
    {
        var sr = t.GetComponent<SpriteRenderer>();
        if (!sr)
        {
            Debug.LogError("SpriteRenderer yok: " + t.name);
            return 0f;
        }
        return sr.bounds.size.x; // scale dahil
    }

    float Right(Transform t, float w) => t.position.x + w * 0.5f;
}