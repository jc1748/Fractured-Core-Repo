using System.Collections;
using UnityEngine;

public class DeathFlowController : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup fadeCanvasGroup;     // black overlay
    public DeathMenuController deathMenu;   // death/upgrade menu controller

    [Header("Timings")]
    public float delayBeforeFade = 0.75f;
    public float fadeDuration = 0.8f;

    [Header("Pause Behavior")]
    public bool pauseWhenMenuOpens = true;

    private bool isDying;
    private PlayerStats currentStats;       // <- stores the stats passed in

    void Awake()
    {
        // Ensure fade starts clear
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.interactable = false;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }

    // Call this when player dies (pass the dying player's stats)
    public void BeginDeathFlow(PlayerStats stats)
    {
        if (isDying) return;
        isDying = true;

        currentStats = stats;
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Stop player input (this script is on UI, so it won't find PlayerAttack here)
        // Disable control from PlayerHealth instead (recommended), so leave this empty or safe.

        yield return new WaitForSecondsRealtime(delayBeforeFade);

        // Fade to black (unscaled time)
        yield return Fade(0f, 1f, fadeDuration);

        if (pauseWhenMenuOpens)
            Time.timeScale = 0f;

        if (deathMenu != null)
        {
            // show menu even if 0 points
            deathMenu.Show(currentStats);
        }
        else
        {
            Debug.LogError("DeathMenuController not assigned on DeathFlowController.");
        }
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (fadeCanvasGroup == null) yield break;

        // block clicks during fade
        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.interactable = false;

        float t = 0f;
        fadeCanvasGroup.alpha = from;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = (duration <= 0f) ? 1f : Mathf.Clamp01(t / duration);
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, a);
            yield return null;
        }

        fadeCanvasGroup.alpha = to;

        // allow UI interaction after fade (menu is on top)
        fadeCanvasGroup.blocksRaycasts = false;
    }

    public void ResetFlowState()
    {
        StopAllCoroutines();
        isDying = false;
        currentStats = null;

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.interactable = false;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }


}