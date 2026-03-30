using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMotor : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 1.4f;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator; //controls idle and run

    [Header("Bounce")]
    [SerializeField] private float wallCheckDistance = 0.15f;
    [SerializeField] private LayerMask bounceLayers;
    [SerializeField] private float bounceCooldown = 0.08f;

    [Header("Air / Juggle Settings")]
    [SerializeField] private float gravity = 18f;   //how fast enemy falls back down
    [SerializeField] private float maxFallSpeed = 12f;  //prevents super fast falling

    private Rigidbody2D rb;

    //normal ai movement
    private bool isMoving;
    private Vector2 destination;

    //ground knockback
    private bool isKnockedBack; //true while enemy is knocked back
    private Vector2 knockbackVelocity; //current knockback movement speed
    public bool IsAirborne => isAirborne; //lets the other scripts know if the enemy is currently in the air
    private float lastBounceTime; //prevents rapid repeated bouncing
    private float currentBounceDamping = 0.45f; //current bounce damping for this hit
    private float knockbackTimer;

    //air juggle state
    private bool isAirborne;    //true while enemy is in air
    private float verticalVelocity;     //upward & downward speed
    private float groundY;      //the Y position enemy should land back on


    public float MoveSpeed => moveSpeed; //allows brain to read base speed
    public bool IsKnockedBack => isKnockedBack; //allows other scripts to check if enemy is knocked back

    public void SetMoveSpeed(float newSpeed) 
    { 
        moveSpeed = newSpeed; //allows brain to modify it
    } 

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Auto-find these so you don't have to drag them every time
        if (sprite == null) sprite = GetComponentInChildren<SpriteRenderer>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    public void MoveTo(Vector3 worldPos)
    {
        if (isKnockedBack || isAirborne) 
            return;//if enemy is knocked back, ignore movement commands

        destination = worldPos; // Vector3 -> Vector2 implicit drops Z
        isMoving = true;
    }

    public void Stop()
    {
        isMoving = false;
    }

    private void FixedUpdate()
    {
        //if enemy is in the air, update vertical motion first
        if (isAirborne)
        {
            HandleAirborne();
        }


        //if enemy is being knocked back, ignore normal movement
        if (isKnockedBack)
        {
            HandleKnockback();
            return;
        }

        //update animator when airborne
        // If enemy is airborne, do not allow normal chase movement
        if (isAirborne)
        {
            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
            }

            return;
        }

        Vector2 currentPos = rb.position;
        Vector2 toTarget = destination - currentPos;

        //Actually moving = we have intent to move, and we aren't already within stopping distance
        bool actuallyMoving = isMoving && toTarget.magnitude > stopDistance;

        //update animator every step
        if(animator != null)
        {
            animator.SetBool("IsMoving", actuallyMoving);
        }

        //if not moving, stop here
        if (!actuallyMoving) return;

        // Stop before overlapping player
        if (toTarget.magnitude <= stopDistance)
        {
            isMoving = false;
            return;
        }

        // Flip sprite left/right
        if (sprite != null && Mathf.Abs(toTarget.x) > 0.01f)
            sprite.flipX = toTarget.x < 0f;

        //calculate movement step or simply just move
        Vector2 step = toTarget.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(currentPos + step);
    }

    private void HandleKnockback()
    {
        Debug.Log("Knockback active. Velocity: " + knockbackVelocity + " Timer: " + knockbackTimer);
        // During knockback, enemy should not play run animation
        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }

        // Check whether we hit a wall and should bounce
        CheckForBounce();

        // Move using knockback velocity
        Vector2 currentPos = rb.position;
        Vector2 step = knockbackVelocity * Time.fixedDeltaTime;
        rb.MovePosition(currentPos + step);

        // Count down knockback time
        knockbackTimer -= Time.fixedDeltaTime;

        // Flip sprite in knockback direction
        if (sprite != null && Mathf.Abs(knockbackVelocity.x) > 0.01f)
            sprite.flipX = knockbackVelocity.x < 0f;

        // When timer ends, stop knockback
        if (knockbackTimer <= 0f)
        {
            knockbackVelocity = Vector2.zero;
            isKnockedBack = false;
        }
    }

    private void CheckForBounce()
    {
        // Don't allow bounce every single frame
        if (Time.time < lastBounceTime + bounceCooldown)
            return;

        // Only check bounce if moving sideways
        if (Mathf.Abs(knockbackVelocity.x) <= 0.01f)
            return;

        // Direction enemy is moving
        Vector2 dir = new Vector2(Mathf.Sign(knockbackVelocity.x), 0f);

        // Raycast ahead to see if a wall is directly in front
        RaycastHit2D hit = Physics2D.Raycast(
            rb.position,
            dir,
            wallCheckDistance,
            bounceLayers
        );

        // If we hit a wall, reverse x velocity
        if (hit.collider != null)
        {
            knockbackVelocity = new Vector2(
                -knockbackVelocity.x * currentBounceDamping,
                knockbackVelocity.y
            );

            lastBounceTime = Time.time;
        }

    }

    private void HandleAirborne()
    {
        //apply gravity every physics step
        verticalVelocity -= gravity * Time.fixedDeltaTime;
        
        //prevent the enemy from falling too fast
        if(verticalVelocity < -maxFallSpeed)
        {
            verticalVelocity = -maxFallSpeed;
        }

        //move the enemy vertically
        Vector2 currentPos = rb.position;
        float newY = currentPos.y + (verticalVelocity * Time.fixedDeltaTime);

        //if enemy has reached the ground again, land
        if(newY <= groundY)
        {
            newY = groundY;
            verticalVelocity = 0f;
            isAirborne = false;

            Debug.Log(name + " landed");
        }

        //apply the new vertical position
        rb.MovePosition(new Vector2(currentPos.x, newY));

    }

    //main knockback function
    //uses knockback data structure
    public void ApplyKnockback(KnockbackData data, float hitDirection)
    {
        // Stop normal AI movement
        isMoving = false;

        // Enter knockback mode
        isKnockedBack = true;

        // Set the timer from your data
        knockbackTimer = data.duration;

        // Set bounce damping from your data
        currentBounceDamping = data.bounceDamping;

        // Horizontal knockback happens on the X axis
        knockbackVelocity = new Vector2(data.horizontalForce * hitDirection,0f);

        // If this attack has vertical force, launch the enemy into the air
        if (data.verticalForce > 0f)
        {
          // Store the ground position so enemy knows where to land
          groundY = rb.position.y;

          // Mark as airborne
          isAirborne = true;

          // Set upward launch speed
          verticalVelocity = data.verticalForce;

          Debug.Log(name + " launched into the air with vertical force: " + verticalVelocity);
        }

       // Helpful debug message so you can confirm it fired
       Debug.Log("Knockback applied: " + knockbackVelocity);
    }


    public void ApplyStats(EnemyStats stats)
    {
        if(stats == null) return;

        moveSpeed = stats.moveSpeed; //set speed
        stopDistance = stats.stopDistance; //set spacing
    }


}
