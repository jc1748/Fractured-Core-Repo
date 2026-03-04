using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float damage = 1f;

    [Header("Timing")]
    [SerializeField] private float cooldown = 1.0f;//time between attacks
    [SerializeField] private float windupTime = 0.15f; //delay before hit happens (should feel like windup)

    [Header("Hitbox")]
    [SerializeField] private Transform hitboxOrigin; //where the hitbox is centered (in front on enemy)
    [SerializeField] private float hitboxRadius = 0.6f; //size of hit area
    [SerializeField] private LayerMask playerLayer; //set this to Player layer

    [Header("Optional Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTriggerName = "Attack";

    private EnemyHealth health; //reference to enemy health (for stun checks)

    private Vector3 originalHitboxLocalPos;
    private SpriteRenderer sprite;

    private float nextReadyTime;

    //enemy brain reads this
    public bool IsReady => Time.time >= nextReadyTime;

    private void Awake()
    {
        if(hitboxOrigin != null)
        {
            originalHitboxLocalPos = hitboxOrigin.localPosition;
        }
        sprite = GetComponentInChildren<SpriteRenderer>();

        //auto-find animator so Attack trigger works
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        health = GetComponent<EnemyHealth>();
    }

    private void Update()
    {
        if (hitboxOrigin == null || sprite == null) return;

        // If sprite is flipped, mirror hitbox on X axis
        if (sprite.flipX)
        {
            hitboxOrigin.localPosition =
                new Vector3(-Mathf.Abs(originalHitboxLocalPos.x),
                            originalHitboxLocalPos.y,
                            originalHitboxLocalPos.z);
        }
        else
        {
            hitboxOrigin.localPosition =
                new Vector3(Mathf.Abs(originalHitboxLocalPos.x),
                            originalHitboxLocalPos.y,
                            originalHitboxLocalPos.z);
        }
    }

    //called by EnemyBrain when it decides to attack
    public void TryAttack(Transform target)
    {
        //don't let enemy start an attack while stunned
        if(health !=null && (health.IsStunned || health.AttackLocked))
        {
            return;
        }

        if (!IsReady) return;

        //start cooldown immediately so there's no spam
        nextReadyTime = Time.time + cooldown;

        //play attack animations
        if(animator != null && !string.IsNullOrEmpty(attackTriggerName))
        {
            animator.SetTrigger(attackTriggerName);
        }

        //delay the hit slightly so it matches up with animation windup
        Invoke(nameof(DoHit), windupTime);
    }

    //this acutally applies damage if the player is inside the hitbox
    private void DoHit()
    {
        if(hitboxOrigin == null)
        {
            Debug.LogWarning("EnemyCombat: hitboxOrigin is not assigned.");
            return;
        }

        //find all colliders in a circle around the hitbox origin (2D physics)
        Collider2D hit = Physics2D.OverlapCircle(hitboxOrigin.position, hitboxRadius, playerLayer);
        if(hit == null)
        {
            return; //player not in range at the hit moment
        }

        //deal damage if that object can take damage
        PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
        if(playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    //stops an attack that was already queued up(windup invoke)
    public void CancelAttack()
    {
        CancelInvoke(nameof(DoHit));

        if(animator != null)
        {
            animator.ResetTrigger(attackTriggerName);
            //animator.Play("Idle"); --play the idle animation 
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (hitboxOrigin == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(hitboxOrigin.position, hitboxRadius);
    }

}

