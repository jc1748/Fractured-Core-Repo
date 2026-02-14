using UnityEngine;

public class PlayerUltimate : MonoBehaviour
{
    [Header("Ulitimate Meter")]
    public float currentUlt = 0f; //current ult charge (0 to maxUlt)
    public float maxUlt = 100f; //full meter value

    [Header("Charge Settings")]
    public float ultPerHit = 5f;

    //Reference to PlayerStats so Ult stat can increase charge gain
    private PlayerStats stats;

    void Awake()
    {
        //grab player stats from the same player object
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        //use T to use when ult is full
        if (Input.GetKeyDown(KeyCode.T))
        {
            TryUseUltimate();
        }
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
        currentUlt += gain;

        //clamp it so it never goes past max
        currentUlt = Mathf.Clamp(currentUlt, 0f, maxUlt);
        Debug.Log($"Ult + {gain:F2} (Total: {currentUlt / maxUlt})");

    }

    //use ult when meter is full
    public void TryUseUltimate()
    {
        //only allow ult if the meter is full 
        if(currentUlt < maxUlt)
        {
            Debug.Log("Ult not ready yet");
            return;
        }

        //spend ult meter
        currentUlt = 0f;

        Debug.Log("ULT ACTIVATED");
        //later on trigger ult animation/attack/effect here
    }
}
