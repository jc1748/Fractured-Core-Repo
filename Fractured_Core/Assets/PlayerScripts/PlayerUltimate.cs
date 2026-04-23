using System.Collections;
using UnityEngine;

public class PlayerUltimate : MonoBehaviour
{
    [Header("Ulitimate Meter")]
    public float currentUltCharge = 0f; //current ult charge (0 to maxUlt)
    public float maxUlt = 100f; //full meter value

    [Header("Charge Settings")]
    public float ultPerHit = 5f;

    [Header("Flurry Settings")]
    public float flurryRadius = 2.5f; //radius of circular slashes
    public int flurryDamagePerHit = 2; //damage each slash deals
    public int flurryHits = 6; //number of slash pulses
    public float flurryInterval = 0.08f; //time between pulses

    [Header("VFX (Optional)")]
    public GameObject slashVfxPrefab; // assign ult slash vfx prefab
    public float vfxRadiusOffset = 1.5f;

    [Header("Camera Shake (Optional")]
    public CameraShake cameraShake;
    public float shakeDuration = 0.15f;
    public float shakeStrength = 0.12f;

    [Header("Hit Freeze (Optional)")]
    public bool useHitFreeze = true;
    public float freezeTime = 0.06f; //pause to make hit feel powerfull

    //which layers count as enemies
    public LayerMask enemyLayers;

    //Reference to PlayerStats so Ult stat can increase charge gain
    private PlayerStats stats;

    //prevents ult from being triggered multiple times at once
    private bool ultIsActive = false;

    void Awake()
    {
        //grab player stats from the same player object
        stats = GetComponent<PlayerStats>();

        if(cameraShake == null)
        {
            Camera mainCam = Camera.main;
            if(mainCam != null)
            {
                cameraShake = mainCam.GetComponent<CameraShake>();
            }
        }
    }

    void Update()
    {
        //use T to use when ult is full
        if (Input.GetKeyDown(KeyCode.R))
        {
            TryUseUltimate();
        }
    }

    public void ResetUlt()
    {
        currentUltCharge = 0f;
    }

    //call when player lands a hit
    public void GainUltFromHit()
    {
        float gain = ultPerHit;

        //if we have stats, scale ult gain using Ult multiplier
        //ex ultStat increases gain 1.04% per point
        if(stats != null)
        {
            gain *= stats.GetUltChargeMultiplier();
        }

        //add ult charge
        currentUltCharge += gain;

        //clamp it so it never goes past max
        currentUltCharge = Mathf.Clamp(currentUltCharge, 0f, maxUlt);
        Debug.Log($"Ult + {gain:F2} (Total: {currentUltCharge / maxUlt})");

    }

    //use ult when meter is full
    public void TryUseUltimate()
    {
        //only allow ult if the meter is full 
        if(currentUltCharge < maxUlt)
        {
            Debug.Log("Ult not ready yet");
            return;
        }

        //spend ult meter
        currentUltCharge = 0f;

        Debug.Log("ULT ACTIVATED");
        //later on trigger ult animation/attack/effect here

        //camera shake right when ult starts(big feedback)
        if(cameraShake != null)
        {
            cameraShake.Shake(shakeDuration, shakeStrength);
        }

        //Start the flurry attack
        StartCoroutine(FlurryAttack());
        
    }

    //swordsman flurry attack
    private IEnumerator FlurryAttack()
    {
        ultIsActive = true;

        Debug.Log("Swordsman Ult!");

        //strength increases ult damage
        float damageMultiplier = (stats != null)
            ? stats.GetDamageMultiplier(): 1f;

        //repeat slash pulses
        for (int i = 0; i < flurryHits; i++)
        {
            //vfx: spawn a slash around the player each pulse
            if(slashVfxPrefab != null)
            {
                //pick a random angle around the player so it looks like a circular flow
                float angle = Random.Range(0f, 360f);
                Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

                Vector3 spawnPos = transform.position + (Vector3)(dir * vfxRadiusOffset);

                //random rotation so VFX doesn't look repeated
                Quaternion rot = Quaternion.Euler(0f, 0f, angle);

                Instantiate(slashVfxPrefab, spawnPos, rot);
            }

            //find enemies in a circle around the player
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, flurryRadius, enemyLayers);


            //damage each enemy found
            foreach (Collider2D col in enemies)
            {
                EnemyHealth eh = col.GetComponent<EnemyHealth>();

                if (eh != null)
                {
                    int dmg = Mathf.RoundToInt(flurryDamagePerHit * damageMultiplier);
                    eh.TakeDamage(dmg);
                }
            }
            //hit freee tiny pause on first pulse only
            if (useHitFreeze && i == 0)
            {
                yield return StartCoroutine(HitFreezeCoroutine());
            }

            //wait before next slash pulse
            yield return new WaitForSeconds(flurryInterval);
        }
        ultIsActive = false;
    }

    private IEnumerator HitFreezeCoroutine()
    {
        //store orginal time scale
        float orginalTimeScale = Time.timeScale;

        //pause game time
        Time.timeScale = 0f;

        //wait in REAL time (not affected by timeScale)
        yield return new WaitForSecondsRealtime(freezeTime);

        //restore time
        Time.timeScale = orginalTimeScale;
    }

    //draw ult radius in scene view(debug help)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, flurryRadius);
    }

}
