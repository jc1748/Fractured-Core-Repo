using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //where the attack originates from (empty transform in front of player)
    public Transform attackPoint;

    [Header("Attack 1 Settings")]
    public KeyCode attack1Key = KeyCode.E; //keyboard button for first attack
    public float attack1Range = 0.7f;      //how far the attack reaches
    public int attack1Damage = 1;          //base damage before scaling
    public float attack1Rate = 2f;         //attacks per second (cooldown control)
    public string attack1Trigger = "attack"; //animator trigger name

    [Header("Attack 2 Settings")]
    public KeyCode attack2Key = KeyCode.Q; //keyboard button for second attack
    public float attack2Range = 1.0f;      //usually larger range (heavy attack example)
    public int attack2Damage = 2;          //higher base damage
    public float attack2Rate = 1.2f;       //slower attack (heavier feel)
    public string attack2Trigger = "attack2"; //animator trigger name for 2nd attack

    private float nextAttackTime = 0f; //tracks cooldown time between attacks

    public LayerMask enemyLayers; //which layers count as enemies

    private Animator anim; //reference to animator component

    [Header("Attack Indicator")]
    public SpriteRenderer attackIndicator; //visual circle indicator
    public Color indicatorColor = new Color(1, 1, 0, 0.25f); //transparent yellow

    [Header("Ultimate Settings")]
    private PlayerUltimate playerUltimate; //reference to ult system

    [Header("XP Settings")]
    private PlayerXP playerXP; //reference to XP system
    public int xpPerHit = 1;   //XP gained per successful hit

    [Header("Player Stats Settings")]
    private PlayerStats playerStats; //reference to strength multiplier system

    [Header("Hitstun Settings")]
    public float attack1Hitstun = 0.10f; //short hitstun for light attack
    public float attack2Hitstun = 0.18f; //longer stun for heavy attack

    void Awake()
    {
        //grab needed components from this same player object
        playerXP = GetComponent<PlayerXP>();
        playerStats = GetComponent<PlayerStats>();
        playerUltimate = GetComponent<PlayerUltimate>();

        //if animator is on same object
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        //configure attack indicator once at start
        if (attackIndicator != null)
        {
            attackIndicator.color = indicatorColor;
            attackIndicator.gameObject.SetActive(false); //hide until attacking
        }
    }

    void Update()
    {
        //prevent attacking if still on cooldown
        if (Time.time < nextAttackTime)
            return;

        //Attack 1 input check
        if (Input.GetKeyDown(attack1Key))
        {
            DoAttack(attack1Range, attack1Damage, attack1Rate, attack1Trigger, attack1Hitstun);
            return;
        }

        //Attack 2 input check
        if (Input.GetKeyDown(attack2Key))
        {
            DoAttack(attack2Range, attack2Damage, attack2Rate, attack2Trigger, attack2Hitstun);
            return;
        }
    }

    //shared attack logic so we don't duplicate code
    void DoAttack(float range, int baseDamage, float rate, string triggerName, float hitstun)
    {
        //set next cooldown time
        nextAttackTime = Time.time + 1f / rate;

        //trigger animation
        if (anim != null && !string.IsNullOrEmpty(triggerName))
        {
            anim.SetTrigger(triggerName);
        }

        Debug.Log($"Player used {triggerName}");

        //show attack indicator at attack point
        if (attackIndicator != null)
        {
            float diameter = range * 2f; //circle size = radius * 2
            attackIndicator.transform.localScale = new Vector3(diameter, diameter, 1f);

            attackIndicator.transform.position = attackPoint.position;
            attackIndicator.gameObject.SetActive(true);
        }

        //detect all enemies within circular attack range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, range, enemyLayers);

        //loop through all enemies hit
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth eh = enemy.GetComponent<EnemyHealth>();
            if (eh == null) continue;

            //start with base damage
            int totalDamage = baseDamage;

            //if stats exist, apply strength multiplier
            if (playerStats != null)
            {
                float scaled = baseDamage * playerStats.GetDamageMultiplier();
                totalDamage = Mathf.RoundToInt(scaled); //convert float to int
            }

            //apply damage and check if damage actually happened
            bool didDamage = eh.TakeDamage(totalDamage, hitstun);

            if (didDamage)
            {
                //add XP
                if (playerXP != null)
                    playerXP.AddXP(xpPerHit);

                //add ult charge
                if (playerUltimate != null)
                    playerUltimate.GainUltFromHit();
            }
        }
    }
}
