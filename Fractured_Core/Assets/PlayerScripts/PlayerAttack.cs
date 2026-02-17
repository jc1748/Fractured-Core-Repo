using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 0.7f;//how far they attack
    public int attackDamage = 1;//damage
    public float attackRate = 2f;//attacks per second

    private float nextAttackTime = 0f;//cool down time
    public LayerMask enemyLayers;//which Layers are Enemies

    private Animator anim;

    [Header("Attack Indicator")]
    public SpriteRenderer attackIndicator;
    public Color indicatorColor = new Color(1, 1, 0, 0.25f); // yellow transparent

    [Header("Ultimate Settings")]
    private PlayerUltimate playerUltimate;

    [Header("XP Settings")]
    private PlayerXP playerXP;
    public int xpPerHit = 1;

    [Header("Player Stats Settings")]
    private PlayerStats playerStats;

    void Awake()
    {
        playerXP = GetComponent<PlayerXP>();
        playerStats = GetComponent<PlayerStats>();
        playerUltimate = GetComponent<PlayerUltimate>();
        anim = GetComponent<Animator>();
    }

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
        }
    }
    void Attack()
    {
        //animation configuration
        if(anim != null)
        {
            anim.SetTrigger("attack");
        }

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
                //start with base damage
                int totalDamage = attackDamage;

                //if player stats exists, scale damage by strength multiplier
                if (playerStats != null)
                {
                    //multiply base damage by a scaling number
                    float scaled = attackDamage * playerStats.GetDamageMultiplier();

                    //convert float to int (rounded)
                    totalDamage = Mathf.RoundToInt(scaled);
                }

                //apply damage once, then gain XP if damage actually happened
                bool didDamage = eh.TakeDamage(totalDamage);
                if (didDamage)
                {
                    //gain XP
                    if (playerXP != null)
                        playerXP.AddXP(xpPerHit);

                    //gain ult charge
                    if (playerUltimate != null)
                        playerUltimate.GainUltFromHit();
                }

                if (didDamage && playerXP !=null)
                {
                    playerXP.AddXP(xpPerHit);
                }

            }

        }
    }

}
