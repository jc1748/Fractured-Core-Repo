using UnityEngine;

// This script gives the player a controlled "fake" jump.
// It does NOT use Rigidbody gravity.
// Instead, it moves the visual part of the player upward and back down
// in a way that is easy to tune for combat games.
public class PlayerJump : MonoBehaviour
{
    [Header("Jump Input")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Header("References")]
    // This should be the child object that contains the sprite / animator / attack point
    // We move this up and down during jump.
    [SerializeField] private Transform visualRoot;

    [SerializeField] private Animator animator;

    [Header("Jump Settings")]
    // Starting upward speed when jump begins
    [SerializeField] private float jumpForce = 10f;

    // How quickly player falls back down
    [SerializeField] private float gravity = 22f;

    // Max visual height allowed, just as a safety clamp
    [SerializeField] private float maxJumpHeight = 3.5f;

    // Small delay after landing before player can jump again
    [SerializeField] private float landingRecovery = 0.05f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    // True while player is in the air
    private bool isAirborne = false;

    // Current up/down speed
    private float verticalVelocity = 0f;

    // Current visual height above ground
    private float airOffsetY = 0f;

    // Small recovery after landing
    private float landingRecoveryTimer = 0f;

    // Store the original local position of the visual root
    private Vector3 visualStartLocalPos;

    // Public properties so other scripts can check jump state
    public bool IsAirborne
    {
        get { return isAirborne; }
    }

    public bool IsGrounded
    {
        get { return !isAirborne && landingRecoveryTimer <= 0f; }
    }

    private void Awake()
    {
        // Auto-find animator if not assigned
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        // If visualRoot was not assigned, try to use the first child
        if (visualRoot == null && transform.childCount > 0)
        {
            visualRoot = transform.GetChild(0);
        }

        // Save the starting local position so we can always return to it
        if (visualRoot != null)
        {
            visualStartLocalPos = visualRoot.localPosition;
        }
    }

    private void Update()
    {
        // Count down landing recovery time
        if (landingRecoveryTimer > 0f)
        {
            landingRecoveryTimer -= Time.deltaTime;
        }

        // Jump input
        if (Input.GetKeyDown(jumpKey) && IsGrounded)
        {
            StartJump();
        }

        // Update air movement every frame
        HandleJumpMotion();

        // Update the visual height of the player
        UpdateVisualHeight();

        // Optional animator parameter
        if (animator != null)
        {
            animator.SetBool("IsAirborne", isAirborne);
        }
    }

    private void StartJump()
    {
        isAirborne = true;
        verticalVelocity = jumpForce;

        if (showDebugLogs)
        {
            Debug.Log("Player jump started");
        }
    }

    private void HandleJumpMotion()
    {
        // If player is not airborne and already on ground, do nothing
        if (!isAirborne && airOffsetY <= 0f)
            return;

        // Apply gravity
        verticalVelocity -= gravity * Time.deltaTime;

        // Move vertical offset using current velocity
        airOffsetY += verticalVelocity * Time.deltaTime;

        // Safety clamp
        if (airOffsetY > maxJumpHeight)
        {
            airOffsetY = maxJumpHeight;
        }

        // If player reaches the ground again, land
        if (airOffsetY <= 0f)
        {
            airOffsetY = 0f;
            verticalVelocity = 0f;

            // Only do landing logic if we were actually airborne
            if (isAirborne)
            {
                isAirborne = false;
                landingRecoveryTimer = landingRecovery;

                if (showDebugLogs)
                {
                    Debug.Log("Player landed");
                }
            }
        }
    }

    private void UpdateVisualHeight()
    {
        if (visualRoot == null)
            return;

        // Start from the original position, then add our jump height
        Vector3 pos = visualStartLocalPos;
        pos.y += airOffsetY;
        visualRoot.localPosition = pos;
    }

    // Optional helper if other scripts want current air height
    public float GetAirHeight()
    {
        return airOffsetY;
    }
}
