using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 5f;
    private float currentHealth;

    [Header("Health Bar")]
    public Transform healthBar;  // drag the health bar here
    private Vector3 originalScale;

    [Header("Flash settings")]
    public SpriteRenderer spriteRenderer;//assign player sprite
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;

    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            originalScale = healthBar.localScale;
        }
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("Player took " + amount + " damage!");

        // update health bar scaling
        if (healthBar != null)
        {
            float percent = Mathf.Clamp01(currentHealth / maxHealth);
            healthBar.localScale = new Vector3(originalScale.x * percent, originalScale.y, originalScale.z);
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

    void Die()
    {
        Debug.Log("Player Died!");
        gameObject.SetActive(false);
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if(currentHealth> maxHealth)
        {
            currentHealth = maxHealth;
        }

        //update health bar
        if(healthBar != null)
        {
            float percent= Mathf.Clamp01(currentHealth / maxHealth);
            //localScale.x,y,z is the size of my healthbar so .x is the one that grows or shrinks
            healthBar.localScale = new Vector3(originalScale.x * percent, originalScale.y, originalScale.z);
        }
    }

    private System.Collections.IEnumerator FlashRed()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }
}
