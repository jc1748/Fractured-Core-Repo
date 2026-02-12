using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    // Beat 'em up style: allow up/down movement too
    public bool allowVerticalMovement = true;

    [Header("Dash Settings")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 2f;


    private Rigidbody2D rb;

    private Vector2 moveInput;
    private bool isDashing = false;
    private float dashTimeLeft = 0f;
    private float nextDashTime = 0f;

    // If you still want jump later, keep it separate from this (beat 'em ups often fake jump)
    // public float jumpForce = 12f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Make sure these are set in Inspector too:
        // Rigidbody2D Body Type = Kinematic
        // Collision Detection = Continuous (optional)
        // Interpolate = Interpolate (optional)
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = allowVerticalMovement ? Input.GetAxisRaw("Vertical") : 0f;

        moveInput = new Vector2(horizontal, vertical).normalized;

        // Flip on horizontal movement only
        if (horizontal != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontal), 1f, 1f);
        }

        // Dash input
        if (Input.GetKeyDown(KeyCode.R) && Time.time >= nextDashTime && !isDashing)
        {
            StartDash();
        }

        // Dash timers + ghost spawns
        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;

            if (dashTimeLeft <= 0f)
            {
                isDashing = false;
            }
        }
    }

    private void FixedUpdate()
    {
        Vector2 currentPos = rb.position;
        Vector2 delta;

        if (isDashing)
        {
            // Dash in facing direction (x only, like Castle Crashers)
            float facing = Mathf.Sign(transform.localScale.x);
            delta = new Vector2(facing * dashSpeed, 0f) * Time.fixedDeltaTime;
        }
        else
        {
            delta = moveInput * moveSpeed * Time.fixedDeltaTime;
        }

        rb.MovePosition(currentPos + delta);
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        nextDashTime = Time.time + dashCooldown;
    }
}
