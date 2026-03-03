using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMotor : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 1.4f;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator; //controls idle and run

    private Rigidbody2D rb;
    private bool isMoving;
    private Vector2 destination;

    public float MoveSpeed => moveSpeed; //allows brain to read base speed

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


}
