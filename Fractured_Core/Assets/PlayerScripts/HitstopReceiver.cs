using System.Collections;
using UnityEngine;

public class HitstopReceiver : MonoBehaviour
{
    [Header("Optional")]
    [SerializeField] private Animator animator; //freezes animations
    [SerializeField] private Rigidbody2D rb; //freezes physics(maybe implement this)

    private float cachedAnimatorSpeed = 1f;
    private Vector2 cachedVelocity;
    private Coroutine stopRoutine;

    private void Awake()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        if (animator != null) cachedAnimatorSpeed = animator.speed;
    }

    //freezes this object briefly(uses realtime so it works even if timescale changes later)
    public void DoHitstop(float duration)
    {
        if(duration <= 0f) return;

        //restart if already stopping 
        if(stopRoutine != null)
        {
            StopCoroutine(stopRoutine);
        }
        stopRoutine = StartCoroutine(HitstopRoutine(duration));

    }

    private IEnumerator HitstopRoutine(float duration)
    {
        //cache current values
        if (animator != null) cachedAnimatorSpeed = animator.speed;

        if (rb != null) cachedVelocity = rb.linearVelocity;

        //apply freeze
        if(animator != null)
        {
            animator.speed = 0f;
        }

        if(rb!= null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        //wait using realtime so this is always consistent
        yield return new WaitForSecondsRealtime(duration);

        //restore
        if(animator!= null)
        {
            animator.speed = cachedAnimatorSpeed;
        }

        if(rb!= null)
        {
            rb.linearVelocity = cachedVelocity;
        }

        stopRoutine = null;
    }
}
