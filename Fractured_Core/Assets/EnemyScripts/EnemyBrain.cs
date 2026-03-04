using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    public enum State { Idle, Chase, Windup, Attack, Recover }

    [Header("Detection")]
    [SerializeField] private float aggroRangeX = 8f;     // horizontal aggro
    [SerializeField] private float loseAggroRangeX = 10f;
    [SerializeField] private float thinkInterval = 0.05f;

    [Header("Combat (X axis)")]
    [SerializeField] private float engageDistanceX = 1.6f; // “pace” distance on X
    [SerializeField] private float attackRangeX = 2.0f;    // can attempt attacks within this X range
    [SerializeField] private float deadZoneX = 0.15f;      // prevents jitter

    [Header("Vertical Tracking (Castle Crashers vibe)")]
    [SerializeField] private float alignRangeY = 2.5f;     // only try to align Y if within this
    [SerializeField] private float alignDeadZoneY = 0.15f; // don't micro-jitter in Y

    [Header("Timing")]
    [SerializeField] private float windupTime = 0.25f;   // quick burst before swing
    [SerializeField] private float recoverTime = 0.25f;  // pause after swing

    [Header("Speed Feel")]
    [SerializeField] private float chaseSpeedMultiplier = 1.0f;   // slow pacing
    [SerializeField] private float windupSpeedMultiplier = 1.8f;  // fast step-in

    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private EnemyMotor motor;
    [SerializeField] private EnemyCombat combat;
    [SerializeField] private EnemyHealth enemyHealth;

    [Header("Debug")]
    [SerializeField] private State state = State.Idle;

    private float thinkTimer;
    private float stateTimer;

    private float baseMoveSpeed;

    private void Awake()
    {
        if (!motor) motor = GetComponent<EnemyMotor>();
        if (!combat) combat = GetComponent<EnemyCombat>();
        if (!enemyHealth) enemyHealth = GetComponent<EnemyHealth>();

        baseMoveSpeed = motor.MoveSpeed;
    }

    private void Start()
    {
        if (!target)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player) target = player.transform;
        }
    }

    private void Update()
    {
        thinkTimer -= Time.deltaTime;
        if (thinkTimer > 0f) return;
        thinkTimer = thinkInterval;

        Tick();
    }

    private void Tick()
    {
        //if hitstunned or attack-locked, freeze behavior and cancel any queued attack
        if(enemyHealth != null && (enemyHealth.IsStunned || enemyHealth.AttackLocked))
        {
            motor.Stop();
            
            if(combat != null)
            {
                combat.CancelAttack(); //prevents "i got hit but still hit you" moment
            }

            return;
        }


        if (!target)
        {
            state = State.Idle;
            motor.Stop();
            return;
        }

        Vector2 enemyPos = transform.position;
        Vector2 playerPos = target.position;

        float dx = Mathf.Abs(playerPos.x - enemyPos.x); // horizontal distance
        float dy = Mathf.Abs(playerPos.y - enemyPos.y); // vertical distance

        // Aggro based on X (side scroller)
        if (dx > loseAggroRangeX)
        {
            state = State.Idle;
            motor.Stop();
            return;
        }

        if (dx > aggroRangeX)
        {
            state = State.Idle;
            motor.Stop();
            return;
        }

        // Handle timed states
        if (state == State.Windup || state == State.Recover)
        {
            stateTimer -= thinkInterval;

            if (state == State.Windup)
            {
                motor.SetMoveSpeed(baseMoveSpeed * windupSpeedMultiplier);

                // Step in toward the player during windup
                motor.MoveTo(playerPos);

                if (stateTimer <= 0f)
                    state = State.Attack;

                return;
            }

            if (state == State.Recover)
            {
                // Freeze after swing
                motor.Stop();
                motor.SetMoveSpeed(baseMoveSpeed * chaseSpeedMultiplier);

                if (stateTimer <= 0f)
                    state = State.Chase;

                return;
            }
        }

        // Attack state: attempt once, then recover (or go back to chase if not ready)
        if (state == State.Attack)
        {
            motor.Stop();

            if (combat != null && combat.IsReady && dx <= attackRangeX)
            {
                combat.TryAttack(target);
                state = State.Recover;
                stateTimer = recoverTime;
            }
            else
            {
                state = State.Chase;
            }

            return;
        }

        // Normal chase/pacing
        state = State.Chase;
        motor.SetMoveSpeed(baseMoveSpeed * chaseSpeedMultiplier);

        // If vertically close enough, align Y a bit (Castle Crashers style)
        // This helps enemies “line up” rather than attack from weird heights.
        Vector2 desiredPos = enemyPos;

        if (dy <= alignRangeY && dy > alignDeadZoneY)
        {
            desiredPos.y = playerPos.y; // move toward player's Y
        }

        // Horizontal spacing logic
        if (dx > engageDistanceX + deadZoneX)
        {
            // Too far: move in (to player x and maybe y)
            desiredPos.x = playerPos.x;
            motor.MoveTo(desiredPos);
            return;
        }

        // Close enough: hold position (don't push into player)
        motor.Stop();

        // If in attack range and ready, start windup burst
        if (dx <= attackRangeX && combat != null && combat.IsReady)
        {
            state = State.Windup;
            stateTimer = windupTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // For side scroller, X ranges are more useful drawn as lines.
        Vector3 pos = transform.position;

        Gizmos.color = new Color(1f, 1f, 0f, 0.7f); // aggro
        Gizmos.DrawLine(pos + Vector3.left * aggroRangeX, pos + Vector3.right * aggroRangeX);

        Gizmos.color = new Color(1f, 0f, 0f, 0.8f); // attack range
        Gizmos.DrawLine(pos + Vector3.left * attackRangeX, pos + Vector3.right * attackRangeX);
    }
}