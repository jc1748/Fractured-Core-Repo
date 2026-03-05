using UnityEngine;

[CreateAssetMenu(menuName= "FracturedCore/EnemyStats", fileName="EnemyStats_")]
public class EnemyStats : ScriptableObject
{
    [Header("Health")]
    public float maxHealth = 5f; //enemy total health

    [Header("Movement")]
    public float moveSpeed = 3f; //how fast enemy moves
    public float stopDistance = 1.4f; //how clse before it stops pushing into player

    [Header("Brain (Side Scroller X ranges)")]
    public float aggroRangeX = 8f;       //when enemy starts tracking player (x only)
    public float loseAggroRangeX = 10f;  //when enemy gives up
    public float engageDistanceX = 1.6f; //“pace” distance before stopping
    public float attackRangeX = 2.0f;    //attack attempt distance (x only)

    [Header("Vertical Tracking")]
    public float alignRangeY = 2.5f;     //only align Y if close enough
    public float alignDeadZoneY = 0.15f; //prevents micro jitter

    [Header("Attack")]
    public float damage = 1f;        //base damage
    public float cooldown = 1.0f;    //time between attacks
    public float windupTime = 0.15f; //delay before hit happens (animation sync)
    public float hitboxRadius = 0.6f;//hit area size

    [Header("Speed Feel")]
    public float chaseSpeedMultiplier = 1.0f;  //slow pacing
    public float windupSpeedMultiplier = 1.8f; //fast step in

    [Header("Stun/Lockout")]
    public float defaultHitstun = 0.12f;        //how long it freezes when hit
    public float attackLockoutAfterHit = 0.18f; //extra “can’t attack yet” window
}
