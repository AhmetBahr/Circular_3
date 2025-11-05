using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private float delaySeconds = 1f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(delaySeconds);

        Debug.Log($"[BOOT] Loading '{gameSceneName}'...");
        var op = SceneManager.LoadSceneAsync(gameSceneName, LoadSceneMode.Single);
        op.completed += _ => Debug.Log($"[BOOT] Loaded: {gameSceneName}");
    }
}