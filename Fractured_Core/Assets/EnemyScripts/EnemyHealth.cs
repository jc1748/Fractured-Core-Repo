using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float enemyMaxHealth = 5f;
    private float currentHealth= 5f;

    [Header("Health Bar")]
    public Transform healthBar;   // assign a child object (sprite) in Inspector
    private Vector3 originalScale;

    [Header("Hitstun Settings")]
    [SerializeField] private float defaultHitstun = 0.12f; //how long enemy freezes when hit

    [SerializeField] private float attackLockoutAfterHit = 0.15f; //extra time enemy cant attack after hit
    private float attackLockoutTimer; //counts down lockout
    public bool AttackLocked => attackLockoutTimer > 0f;

    [Header("Flash settings")]
    public SpriteRenderer spriteRenderer;//assign enemy sprite
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;

    [Header("Drops")]
    public GameObject healthPickupPrefab;
    public float dropChance = 1f;//1 always drops


    private Color originalColor;
    private float stunTimer; //counts down stun time
    public bool IsStunned => stunTimer > 0f;

    private EnemyCombat combat; //used to cancel queued hits

    [SerializeField] private Animator animator; //enemy animator

    private Rigidbody2D rb;

    public GameObject damageNumberPrefab;
    public Canvas worldSpaceCanvas;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        combat = GetComponent<EnemyCombat>();

        //auto find sprite renderer
        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if(animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    void Start()
    {
        currentHealth = enemyMaxHealth;
        if (healthBar != null)
        {
            originalScale = healthBar.localScale;
        }
        //save original color for flashing
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

    }

    private void Update()
    {
        //count down hitstun timeer
        if(stunTimer > 0f)
        {
            stunTimer -= Time.deltaTime;
        }

        if (attackLockoutTimer > 0f)
        {
            attackLockoutTimer -= Time.deltaTime;
        }
    }

    //applies hitstun (keeps the longer stun if multiple hits happen fast
    private void ApplyHitstun(float duration)
    {
        stunTimer = duration; //refresh stun each time you get hit
    }

    public bool TakeDamage(int damage, float hitstun = -1f)
    {
        if (damage <= 0)
        {
            return false;
        }

        if (currentHealth <= 0)
        {
            return false;//already dead
        }

        //stop enemy from finishing a queued hit
        if(combat != null)
        {
            combat.CancelAttack();
        }

        currentHealth -= damage;

        //apply hitstun
        float stunToApply = (hitstun < 0f) ? defaultHitstun : hitstun;
        ApplyHitstun(stunToApply);
        attackLockoutTimer = Mathf.Max(attackLockoutTimer, attackLockoutAfterHit);

        //freeze animation when being hit
        if(animator != null)
        {
            animator.speed = 0f; //pause animation while stunned
            CancelInvoke(nameof(ResumeAnimator)); //avoid stacking
            Invoke(nameof(ResumeAnimator), stunToApply);
        }


        //flash white/red or play animation


        //ui damage
        if (damageNumberPrefab != null && worldSpaceCanvas != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 1f;

            // Convert world position to canvas local space

            GameObject num = Instantiate(damageNumberPrefab, worldSpaceCanvas.transform);
            num.transform.position = spawnPos;

            DamageNumber dn = num.GetComponent<DamageNumber>();
            dn.SetDamage(damage);

            dn.SetScale(ComboManager.instance.damageMultiplier);

        }

        UpdateHealthBar();
        TriggerFlash();
  
        //check death
        if (currentHealth <= 0) 
        {
            Die();
        }

        return true; //damage applied
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null) return;

        float healthPercent = Mathf.Clamp01(currentHealth / enemyMaxHealth);
        healthBar.localScale = new Vector3(originalScale.x * healthPercent, originalScale.y, originalScale.z);
    }

    private void TriggerFlash()
    {
        if (spriteRenderer == null) return;

        StopCoroutine(nameof(FlashRed)); //prevents overlap spam
        StartCoroutine(FlashRed());
    }

    //enemy animation "stops" when hit and resumes after stun
    private void ResumeAnimator()
    {
        if(animator != null)
        {
            animator.speed = 1f;
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + "Died!");

        //death fx here
        if(healthPickupPrefab != null && Random.value<= dropChance)//checks if prefab is assigned in inspector and gives a random number between 0 and 1
        {
            //spawns health pickup and spawns at enemy position and gives no rotation
            Instantiate(healthPickupPrefab, transform.position, Quaternion.identity);
        }

        gameObject.SetActive(false);
    }

    private System.Collections.IEnumerator FlashRed()
    {
        spriteRenderer.color= flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }
    
}
