using System.Collections;
using UnityEngine;

public class DeathFlowController : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup fadeCanvasGroup; //black overaly Canvas
    public DeathMenuController deathMenu;
    public PlayerStats playerStats;

    [Header("Timings")]
    public float delayBeforeFade = 0.75f; //let body "land"
    public float fadeDuration = 0.8f;

    [Header("Pause Behavior")]
    public bool pauseWhenMenuOpens = true;

    private bool isDying;

    void Awake()
    {
        //ensure fade starts clear
        if(fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.interactable = false;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }

    //call this when player dies
    public void BeginDeathFlow()
    {
        if (isDying) return;
        isDying = true;

        StartCoroutine(DeathSequence());
    }


    private IEnumerator DeathSequence()
    {
        Debug.Log("DeathSequence started");


        //stop player input
        DisablePlayerControl();

        //small delay so the player stays on ground before fading
        yield return new WaitForSecondsRealtime(delayBeforeFade);
        Debug.Log("Starting fade");

        //fade to black using unscaled time
        yield return Fade(0f, 1f, fadeDuration);

        //decide what happens next:
        bool hasPoints = (playerStats != null && playerStats.statPoints >0);
        Debug.Log($"Fade complete. playerStats null? {playerStats == null}. statPoints={(playerStats != null ? playerStats.statPoints : -1)}. hasPoints={hasPoints}");

        if (deathMenu != null)
        {
            if (pauseWhenMenuOpens)
            {
                Time.timeScale = 0f;
            }

           deathMenu.Show(playerStats); //show even if 0 points
        }
        else
        {
            Debug.LogError("DeathMenuController not Assigned on DeathFlowController");
        }
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (fadeCanvasGroup == null) yield break;

        fadeCanvasGroup.blocksRaycasts = false;//block clicks during fade
        fadeCanvasGroup.interactable = false;

        float t = 0f;
        fadeCanvasGroup.alpha = from;

        while(t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = (duration <= 0) ? 1f : Mathf.Clamp01(t/duration);
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, a);
            yield return null;
        }

        fadeCanvasGroup.alpha = to;
    }

    private void DisablePlayerControl()
    {
        var attack = GetComponent<PlayerAttack>();
        if(attack) attack.enabled = false;
    }
}
