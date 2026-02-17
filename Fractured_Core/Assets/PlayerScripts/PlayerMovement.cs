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

    private Animator anim;


    private Rigidbody2D rb;

    private Vector2 moveInput;
    private bool isDashing = false;
    private float dashTimeLeft = 0f;
    private float nextDashTime = 0f;

    //reference stat system so move speed stat can affect movement
    private PlayerStats playerStats;

    //store this in fixed update (fixed update runs on a physics update
    private float effectiveMoveSpeed;

  
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        //grab player stats from the same game object(player)
        playerStats = GetComponent<PlayerStats>();

        //animation set up
        anim = GetComponent<Animator>();

        // Make sure these are set in Inspector too:
        // Rigidbody2D Body Type = Kinematic
        // Collision Detection = Continuous (optional)
        // Interpolate = Interpolate (optional)
    }

    private void Update()
    {
        //read player input each frame
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = allowVerticalMovement ? Input.GetAxisRaw("Vertical") : 0f;

        //normalize so diagonal movement isn't faster than straight movement
        moveInput = new Vector2(horizontal, vertical).normalized;

        // Flip sprite on horizontal movement only (preserve scale)
        if (horizontal != 0)
        {
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * Mathf.Sign(horizontal);//keeps size and only flips direction
            transform.localScale = s;
        }

        //start with base speed
        effectiveMoveSpeed = moveSpeed;

        //if player stats exists, multiply by move speed mulitplier
        if(playerStats != null)
        {
            effectiveMoveSpeed *= playerStats.GetMoveSpeedMultiplier();
        }

        // Dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextDashTime && !isDashing)
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

        //animation movement
        if (anim != null)
        {
            bool isMoving = moveInput.sqrMagnitude > 0.01f && !isDashing;
            anim.SetBool("isMoving", isMoving);
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

            //by default, dashSpeed is NOT affected by Move Speed stat
            delta = new Vector2(facing * dashSpeed, 0f) * Time.fixedDeltaTime;
        }
        else
        {
            //use effectiveMoveSpeed (baseSpeed * move speed mulitplier)
            delta = moveInput * effectiveMoveSpeed * Time.fixedDeltaTime;
        }

        //move position--good for kinematic/controlled movement
        rb.MovePosition(currentPos + delta);
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        nextDashTime = Time.time + dashCooldown;
    }
}
