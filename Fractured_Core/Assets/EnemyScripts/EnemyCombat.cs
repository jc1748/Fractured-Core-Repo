using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private float cooldown = 1.25f;
    [SerializeField] private float windupTime = 0.15f;
    [SerializeField] private int damage = 1;

    private float nextReadyTime;

    // Property = read-only access
    public bool IsReady => Time.time >= nextReadyTime;

    private Transform pendingTarget;

    // Called by EnemyBrain
    public void TryAttack(Transform target)
    {
        // Prevent attack spam
        if (!IsReady || target == null)
            return;

        nextReadyTime = Time.time + cooldown;

        pendingTarget = target;

        // Simulate attack animation delay
        Invoke(nameof(ApplyHit), windupTime);
    }

    private void ApplyHit()
    {
        if (!pendingTarget)
            return;

        // Look for damageable component
        IDamageable damageable =
            pendingTarget.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        pendingTarget = null;
    }
}

// Interface = contract for anything that can take damage
public interface IDamageable
{
    void TakeDamage(int amount);
}
