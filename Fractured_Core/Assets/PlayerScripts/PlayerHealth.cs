using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 5f;
    public float currentHealth;

    [Header("Flash settings")]
    public SpriteRenderer spriteRenderer;//assign player sprite
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;

    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void TakeDamage(float amount)
    {
        //grab stats from the same player object
        PlayerStats stats = GetComponent<PlayerStats>();

        //start with the incoming damage
        float finalDamage = amount;

        //if stats exist, reduce damage using defense multiplier
        //example= defense multiplier 0.90 means take 90% damage

        if (stats != null)
        {
            finalDamage *= stats.GetDefenseMultiplier();
        }

        currentHealth -= finalDamage;

        Debug.Log("Player took " + finalDamage + " damage (after defense)!");
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

    }


    private System.Collections.IEnumerator FlashRed()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }
}
