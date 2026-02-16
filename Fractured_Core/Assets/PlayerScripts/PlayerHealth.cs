using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 5f;
    public float currentHealth;

    private bool isDead = false;

    [Header("Flash settings")]
    public SpriteRenderer spriteRenderer;//assign player sprite
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;

    [Header("Death Menu")]
    public DeathMenuController deathMenu;

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
        if (isDead) return;

        //ignore invalid damage
        if (amount <= 0) return;

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

        //flash feedback
        if(spriteRenderer != null)
        {
            StopCoroutine(nameof(FlashRed)); //prevents overlapping flash
            StartCoroutine(FlashRed());
        }

        //death check
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        Debug.Log("Player Died!");
        
        //show death menu (and spend stat points)
        if(deathMenu != null)
        {
            PlayerStats stats = GetComponent<PlayerStats>();
            deathMenu.Show(stats);
        }
        else
        {
            Debug.LogError("DeathMenu controller not assigned on Player health");
        }

        //disable movement/attack so player can't act while dead
        PlayerMovement move = GetComponent<PlayerMovement>();
        if (move != null) move.enabled = false;

        PlayerAttack atk = GetComponent<PlayerAttack>();
        if(atk != null) atk.enabled = false;

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
