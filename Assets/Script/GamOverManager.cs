using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public CanvasGroup deathCanvasGroup;
    public Button restartButton, AdmobButton;
    [SerializeField] private Animator panelAnimator;

    public void Show(float fadeDuration = 0.5f)
    {
        // CanvasGroup başlangıç
        deathCanvasGroup.alpha = 0f;
        deathCanvasGroup.interactable = false;
        deathCanvasGroup.blocksRaycasts = false;
        if (restartButton) restartButton.interactable = false;
        if (AdmobButton)   AdmobButton.interactable   = false;

        // Animator güvenli başlat
        if (panelAnimator)
        {
            panelAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            panelAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            panelAnimator.ResetTrigger("admob");
            panelAnimator.Rebind();          // tüm param/state’i temizler
            panelAnimator.Update(0f);

            panelAnimator.SetTrigger("gameover"); // Any State -> DeathPanelAnimator
            panelAnimator.Update(0f);             // anında uygula
        }

        StartCoroutine(FadeInUnscaled(fadeDuration));
    }

    public void BackToIdle()
    {
        if (panelAnimator)
        {
            panelAnimator.ResetTrigger("gameover");
            panelAnimator.SetTrigger("admob"); // DeathPanelAnimator -> Idle
            panelAnimator.Update(0f);
        }
    }

    public void ResetImmediate()
    {
        if (panelAnimator)
        {
            panelAnimator.Rebind();
            panelAnimator.Update(0f);
            panelAnimator.Play("Idle", 0, 0f);
            panelAnimator.Update(0f);
        }

        deathCanvasGroup.alpha = 0f;
        deathCanvasGroup.interactable = false;
        deathCanvasGroup.blocksRaycasts = false;

        if (restartButton) restartButton.interactable = false;
        if (AdmobButton)   AdmobButton.interactable   = false;
    }

    private IEnumerator FadeInUnscaled(float dur)
    {
        float t = 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            deathCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / dur);
            yield return null;
        }
        deathCanvasGroup.alpha = 1f;
        deathCanvasGroup.blocksRaycasts = true;
        deathCanvasGroup.interactable = true;

        if (restartButton) restartButton.interactable = true;
        if (AdmobButton)   AdmobButton.interactable   = true;
    }
}
