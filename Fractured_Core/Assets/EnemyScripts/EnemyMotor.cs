using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMotor : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 1.4f;
    [SerializeField] private SpriteRenderer sprite;

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
        if (!isMoving) return;

        Vector2 currentPos = rb.position;
        Vector2 toTarget = destination - currentPos;

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
