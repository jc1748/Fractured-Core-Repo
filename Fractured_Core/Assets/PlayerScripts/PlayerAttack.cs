using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 0.7f;//how far they attack
    public int attackDamage = 1;//damage
    public float attackRate = 2f;//attacks per second
    public float playerLaunchFollowForce = 4f;//how far the player moves upward when using launch attack

    private float nextAttackTime = 0f;//cool down time
    public LayerMask enemyLayers;//which Layers are Enemies

    [Header("Attack Indicator")]
    public SpriteRenderer attackIndicator;
    public Color indicatorColor = new Color(1, 1, 0, 0.25f); // yellow transparent

    void Start()
    {
        if (attackIndicator != null)
        {
            float diameter = attackRange * 2f;
            attackIndicator.transform.localScale = new Vector3(diameter, diameter, 1);
            attackIndicator.color = indicatorColor;
            attackIndicator.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        //only attack when cooldown has expired
        if(Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Attack();
                //set next attack time
                nextAttackTime = Time.time + 1f / attackRate;
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                LaunchAttack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }
    void Attack()
    {
        Debug.Log("Player attacked!");

        if (attackIndicator != null)
        {
            attackIndicator.transform.position = attackPoint.position;
            attackIndicator.gameObject.SetActive(true);
        }

        //detect enemies in a circle at the attack point
        Collider2D[] hitEnemies= Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        //apply damage to each enemy detected
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth eh = enemy.GetComponent<EnemyHealth>();
            if (eh != null)
            {

                int totalDamage = attackDamage;
                
                eh.TakeDamage(totalDamage);

            }
        }
        if (attackIndicator != null)
            attackIndicator.gameObject.SetActive(false);
    }

    void LaunchAttack()
    {
        Debug.Log("Launch Attack!");

        if (attackPoint == null)
        {
            Debug.LogError("AttackPoint is NOT assigned on PlayerAttack.");
            return;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is missing on the player.");
            return;
        }

        if (attackIndicator != null)
        {
            attackIndicator.transform.position = attackPoint.position;
            attackIndicator.gameObject.SetActive(true);
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * playerLaunchFollowForce, ForceMode2D.Impulse);
    }
        //lets me see the attack range in scene view
        void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }


}
