using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float enemyMaxHealth = 5f;
    private float currentHealth= 5f;
    public float launchForce = 6f;

    [Header("Health Bar")]
    public Transform healthBar;   // assign a child object (sprite) in Inspector
    private Vector3 originalScale;

    [Header("Flash settings")]
    public SpriteRenderer spriteRenderer;//assign enemy sprite
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;

    [Header("Drops")]
    public GameObject healthPickupPrefab;
    public float dropChance = 1f;//1 always drops


    private Color originalColor;

    private Rigidbody2D rb;

    public GameObject damageNumberPrefab;

    public Canvas worldSpaceCanvas;

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
        rb = GetComponent<Rigidbody2D>();//required or else capsule won't fly

    }

    public void TakeLaunchDamage(int damage)
    {
        currentHealth -= damage;

        // pop enemy upward
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * launchForce, ForceMode2D.Impulse);
        }

        // UI damage for launch attack
        if (damageNumberPrefab != null && worldSpaceCanvas != null)
        {
            // spawn higher
            Vector3 spawnPos = transform.position + Vector3.up * 1.5f;

            GameObject num = Instantiate(damageNumberPrefab, worldSpaceCanvas.transform);
            num.transform.position = spawnPos;

            DamageNumber dn = num.GetComponent<DamageNumber>();
            dn.SetDamage(damage);

            //COMBO-SCALED SIZE � VERY NOTICEABLE
            float launchScale = 1.5f * ComboManager.instance.damageMultiplier;
            dn.SetScale(launchScale);
        }
        //update healthbar,flash,etc.
        if (healthBar != null)
        {
            float healthPercent = Mathf.Clamp01(currentHealth / enemyMaxHealth);
            healthBar.localScale = new Vector3(originalScale.x * healthPercent, originalScale.y, originalScale.z);
        }

        if (spriteRenderer != null)
        {
            StartCoroutine(FlashRed());
        }

        if (currentHealth <= 0)
        {
            Die();
        }

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
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
   
        if (healthBar != null)
        {
            float healthPercent = Mathf.Clamp01(currentHealth / enemyMaxHealth);
            healthBar.localScale = new Vector3(originalScale.x * healthPercent, originalScale.y, originalScale.z);
        }

        if(spriteRenderer != null)
        {
            StartCoroutine(FlashRed());
        }

        //check death
        if (currentHealth <= 0) 
        {
            Die();
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
