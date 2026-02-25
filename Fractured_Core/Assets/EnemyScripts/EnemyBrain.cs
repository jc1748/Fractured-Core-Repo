using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    //enum- a named list of values
    //using this to track what the enemy is currently doing
    public enum State
    {
        Idle, 
        Chase,
        Attack,
        Recover 
    }

    [Header("Detection Settings")]
    [SerializeField] private float aggroRange = 8f;  //distance where enemy notices player
    [SerializeField] private float attackRange = 1.8f; //distance required to attack
    [SerializeField] private float loseAggroRange = 10f; //slightly larger than aggro range
    [SerializeField] private float thinkInterval = 0.1f; //how often AI "thinks"

    //other script references
    [SerializeField] private Transform target; //player transform
    [SerializeField] private EnemyMotor motor; //handles movement
    [SerializeField] private EnemyCombat combat; //handles attacking

    [Header("Debug")]
    [SerializeField] private State currentState = State.Idle;

    private float thinkTimer;

    //automatically grap components if missing
    private void Reset()
    {
        motor = GetComponent<EnemyMotor>();
        combat = GetComponent<EnemyCombat>();
    }

    private void Awake()
    {
        if (!motor) motor = GetComponent<EnemyMotor>();
        if (!combat) combat = GetComponent<EnemyCombat>();
    }

    private void Start()
    {
        //automatically find player with tag
        if (!target)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                target = player.transform;
            }
        }
    }

    private void Update()
    {
        //countdown timer
        thinkTimer -=Time.deltaTime;

        //only run ai logic occasionally
        if (thinkTimer > 0f)
        {
            return;
        }

        thinkTimer = thinkInterval;

        //run decision logic
        TickBrain();
    }

    private void TickBrain()
    {
        //if no player exists--do nothing
        if (!target)
        {
            SetState(State.Idle);
            return;
        }

        //calculate distance to player
        float distance = Vector3.Distance(transform.position, target.position);

        //lose aggro if too far
        if(distance > loseAggroRange)
        {
            SetState(State.Idle);
            motor.Stop();
            return;
        }
        //player detected logic
        if(distance <= aggroRange)
        {
            //if close enough and attack is ready then attack
            if(distance <= attackRange && combat.IsReady)
            {
                SetState(State.Attack);
                motor.Stop(); //stop moving before attacking
                combat.TryAttack(target);

                //immediately enter recovery state
                SetState(State.Recover);
                return;
            }
            //otherwise chase player
            SetState(State.Chase);
            motor.MoveTo(target.position);
            return;
        }

        //default fallback
        SetState(State.Idle);
        motor.Stop();

    }

    private void SetState(State next)
    {
        // Prevent unnecessary switching
        if (currentState == next)
            return;

        currentState = next;

        // Later:
        // animator.SetBool("IsMoving", next == State.Chase);
    }

    //debug visuals
    private void OnDrawGizmosSelected()
    {
        // Yellow = detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);

        // Red = attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Gray = aggro loss range
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, loseAggroRange);
    }

}
