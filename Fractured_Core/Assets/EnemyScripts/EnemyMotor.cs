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

    [Header("Knockback")]
    [SerializeField] private float knockbackDrag = 18f; //how quickly knockback slows down over time, higher number = enemy stops sliding faster
    [SerializeField] private float minimumKnockbackSpeed = 0.15f; //when knockback velocity becomes smaller than this, stop knockback completely
    [SerializeField] private float bounceMultiplier = 0.45f; //how much velocity is kept when bouncing off a wall. ex 0.5 means bouceback with half the speed
    [SerializeField] private float wallCheckDistance = 0.08f; //how far forwared we check for a wall
    [SerializeField] private LayerMask bounceLayers; //which layers count as walls

    private Rigidbody2D rb;
    private bool isMoving;
    private Vector2 destination;

    private bool isKnockedBack;
    private Vector2 knockbackVelocity;
    private float lastBounceTime;
    private float bounceCooldown = 0.08f;

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
        destination = worldPos; // Vector3 -> Vector2 implicit drops Z
        isMoving = true;
    }

    public void Stop()
    {
        isMoving = false;
    }

    private void FixedUpdate()
    { 
        Vector2 currentPos = rb.position;
        Vector2 toTarget = destination - currentPos;

        //Actually moving = we have intent to move, and we aren't already within stopping distance
        bool actuallyMoving = isMoving && toTarget.magnitude > stopDistance;

        //update animator every step
        if(animator != null)
        {
            animator.SetBool("IsMoving", actuallyMoving);
        }

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

        Vector2 step = toTarget.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(currentPos + step);
    }

    public void ApplyStats(EnemyStats stats)
    {
        if(stats == null) return;

        moveSpeed = stats.moveSpeed; //set speed
        stopDistance = stats.stopDistance; //set spacing
    }


}
