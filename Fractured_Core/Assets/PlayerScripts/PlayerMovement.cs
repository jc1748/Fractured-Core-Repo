using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    // Beat 'em up style movement on the flat gameplay plane
    public bool allowVerticalMovement = true;

    [Header("Dash Settings")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 2f;


    [Header("Jump Settings")]
    public KeyCode jumpKey = KeyCode.Space;

    // How strong the jump starts
    public float jumpForce = 10f;

    // How fast the player falls back down
    public float jumpGravity = 22f;

    // Safety cap so jump does not go absurdly high
    public float maxJumpHeight = 3.5f;

    // Small delay after landing before another jump
    public float landingRecovery = 0.05f;

    [Header("Jump Visual References")]
    // Assign this ONLY if you have a visual object you want to lift up/down.
    // Do NOT assign the root player object here.
    public Transform visualToLift;

    private Animator anim;
    private Rigidbody2D rb;

    private Vector2 moveInput;
    private bool isDashing = false;
    private float dashTimeLeft = 0f;
    private float nextDashTime = 0f;

    // Reference stat system so move speed stat can affect movement
    private PlayerStats playerStats;

    // Store this in Update and use in FixedUpdate
    private float effectiveMoveSpeed;

    // JUMP STATE
    
    // True while player is in the air
    private bool isAirborne = false;

    // Current up/down jump speed
    private float verticalVelocity = 0f;

    // Fake jump height above the ground plane
    private float jumpHeight = 0f;

    // Small cooldown after landing
    private float landingRecoveryTimer = 0f;

    // Starting positions for lifted transforms
    private Vector3 visualStartLocalPos;
    private Vector3 attackPointStartLocalPos;

    // Public properties so other scripts can check jump state
    public bool IsAirborne
    {
        get { return isAirborne; }
    }

    public bool IsGrounded
    {
        get { return !isAirborne && landingRecoveryTimer <= 0f; }
    }

    // Optional helper for other systems
    public float GetJumpHeight()
    {
        return jumpHeight;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Grab player stats from the same object
        playerStats = GetComponent<PlayerStats>();

        // Try to find animator on this object first
        anim = GetComponent<Animator>();

        // If animator is not on root, look in children
        if (anim == null)
        {
            anim = GetComponentInChildren<Animator>();
        }

        // Save starting local position of the visual object
        if (visualToLift != null)
        {
            visualStartLocalPos = visualToLift.localPosition;

            // Warn if root object was assigned by mistake
            if (visualToLift == transform)
            {
                Debug.LogWarning("PlayerMovement: visualToLift should not be the root player object.");
            }
        }
    }

    private void Update()
    {
        // Read player input each frame
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = allowVerticalMovement ? Input.GetAxisRaw("Vertical") : 0f;

        // Normalize so diagonal movement isn't faster
        moveInput = new Vector2(horizontal, vertical).normalized;

        // Flip sprite / player facing on horizontal movement only
        if (horizontal != 0)
        {
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * Mathf.Sign(horizontal);
            transform.localScale = s;
        }

        // Start with base speed
        effectiveMoveSpeed = moveSpeed;

        // Apply move speed stat multiplier if it exists
        if (playerStats != null)
        {
            effectiveMoveSpeed *= playerStats.GetMoveSpeedMultiplier();
        }

        // Jump input
        if (Input.GetKeyDown(jumpKey) && IsGrounded)
        {
            StartJump();
        }

        // Dash input
        // For now, only allow dash while grounded.
        // This keeps things simpler and avoids weird air dash behavior.
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextDashTime && !isDashing && IsGrounded)
        {
            StartDash();
        }

        // Dash timer
        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;

            if (dashTimeLeft <= 0f)
            {
                isDashing = false;
            }
        }

        // Landing recovery timer
        if (landingRecoveryTimer > 0f)
        {
            landingRecoveryTimer -= Time.deltaTime;
        }

        // Update jump motion every frame
        HandleJumpMotion();

        // Update any lifted visual objects
        UpdateJumpVisuals();

        // Animation parameters
        if (anim != null)
        {
            bool isMoving = moveInput.sqrMagnitude > 0.01f && !isDashing;
            anim.SetBool("isMoving", isMoving);
            anim.SetBool("IsAirborne", isAirborne);
        }
    }

    private void FixedUpdate()
    {
        Vector2 currentPos = rb.position;
        Vector2 delta;

        if (isDashing)
        {
            // Dash in facing direction on X only
            float facing = Mathf.Sign(transform.localScale.x);
            delta = new Vector2(facing * dashSpeed, 0f) * Time.fixedDeltaTime;
        }
        else
        {
            // Normal grounded/air movement on the gameplay plane
            // This still lets the player move while airborne.
            delta = moveInput * effectiveMoveSpeed * Time.fixedDeltaTime;
        }

        rb.MovePosition(currentPos + delta);
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        nextDashTime = Time.time + dashCooldown;
    }

    private void StartJump()
    {
        isAirborne = true;
        verticalVelocity = jumpForce;

        // Optional debug
        Debug.Log("Player jump started");
    }

    //this is used for the launcher attack to bring the player in the air a bit when using the attack
    public void LaunchPlayer(float launchForce)
    {
        isAirborne = true;
        verticalVelocity = launchForce;
    }

    private void HandleJumpMotion()
    {
        // If we are not airborne and already on the ground, do nothing
        if (!isAirborne && jumpHeight <= 0f)
            return;

        // Apply fake gravity
        verticalVelocity -= jumpGravity * Time.deltaTime;

        // Move jump height using current vertical speed
        jumpHeight += verticalVelocity * Time.deltaTime;

        // Clamp maximum height
        if (jumpHeight > maxJumpHeight)
        {
            jumpHeight = maxJumpHeight;
        }

        // Land when reaching ground again
        if (jumpHeight <= 0f)
        {
            jumpHeight = 0f;
            verticalVelocity = 0f;

            if (isAirborne)
            {
                isAirborne = false;
                landingRecoveryTimer = landingRecovery;

                Debug.Log("Player landed");
            }
        }
    }

    private void UpdateJumpVisuals()
    {
        // Lift the visual object if assigned
        if (visualToLift != null)
        {
            Vector3 pos = visualStartLocalPos;
            pos.y += jumpHeight;
            visualToLift.localPosition = pos;
        }

    }
}
