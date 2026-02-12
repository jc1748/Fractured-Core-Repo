using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    public float contactDamage = 1f;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth player = other.gameObject.GetComponent<PlayerHealth>();

        if (player != null)
        {
            player.TakeDamage(contactDamage);
        }
    }

}
