using UnityEngine;

//this script sits on the same object as the Animator
//its job is to receive animation events and forward them to the real player attack script on the parent object
public class PlayerAnimationEventRelay : MonoBehaviour
{
    // Reference to the real PlayerAttack script
    private PlayerAttack playerAttack;

    private void Awake()
    {
        // Find PlayerAttack on this object or its parent objects
        playerAttack = GetComponentInParent<PlayerAttack>();

        // Warn you if it cannot be found
        if (playerAttack == null)
        {
            Debug.LogWarning("PlayerAnimationEventRelay could not find PlayerAttack in parent objects.");
        }
    }

    // This method name must match the animation event name
    public void AnimEvent_DoHit()
    {
        // Forward the event to the real attack script
        if (playerAttack != null)
        {
            playerAttack.AnimEvent_DoHit();
        }
    }
}
