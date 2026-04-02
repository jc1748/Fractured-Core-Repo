using UnityEngine;

//this script sits on the same object as the Animator
//its job is to receive animation events and forward them to the real player attack script on the parent object
public class PlayerAnimationEventRelay : MonoBehaviour
{
    // Reference to the real PlayerAttack script
    private PlayerAttack playerAttack;

    private void Awake()
    {
        playerAttack = GetComponentInParent<PlayerAttack>();

        if (playerAttack == null)
        {
            Debug.LogWarning("PlayerAnimationEventRelay could not find PlayerAttack in parent objects.");
        }
    }

    // Called by animation event for hit frames
    public void AnimEvent_DoHit()
    {
        if (playerAttack != null)
        {
            playerAttack.AnimEvent_DoHit();
        }
    }

    // Called by animation event for launcher self-lift
    public void AnimEvent_LaunchPlayer()
    {
        if (playerAttack != null)
        {
            playerAttack.AnimEvent_LaunchPlayer();
        }
    }
}
