using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Instance;

    [Header("Sahnedeki Background Image'lar")]
    public SpriteRenderer[] backgroundImages;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Backgroundları verilen spritelerle günceller.
    /// </summary>
    public void SetBackgrounds(Sprite[] newSprites)
    {
        backgroundImages[0].sprite = newSprites[0];
        backgroundImages[1].sprite  = newSprites[1];
        backgroundImages[2].sprite  = newSprites[2];
        backgroundImages[3].sprite  = newSprites[3];
        backgroundImages[4].sprite  = newSprites[2];
        backgroundImages[5].sprite  = newSprites[3];
    }
}