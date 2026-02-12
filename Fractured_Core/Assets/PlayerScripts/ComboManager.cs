using UnityEngine;

public class ComboManager : MonoBehaviour
{
    public float comboResetTime = 1f;//time window between hits
    public float timeSinceLastHit = 0f;

    public int comboCount = 0; //numer of successful hits
    public float damageMultiplier = 1f;

    public float minHitInterval = 0.2f; //prevents button mashing
    private float lastHitTime = 0f;

    public static ComboManager instance;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        //count up the timer
        timeSinceLastHit += Time.deltaTime;

        //reset combo if too much time passed
        if (timeSinceLastHit >= comboResetTime)
        {
            ResetCombo();
        }
    }

    public void RegisterHit()
    {
        //prevents button-mashing (ignore hits too close together)
        if(Time.time - lastHitTime < minHitInterval)
        {
            return;
        }

        lastHitTime = Time.time;
        timeSinceLastHit = 0f;

        comboCount++;
        UpdateDamageMultiplier();
    }

    private void UpdateDamageMultiplier()
    {
        damageMultiplier = 1f + (comboCount * 0.1f);
    }

    public void ResetCombo()
    {
        comboCount = 0;
        damageMultiplier = 1f;
    }
}
