using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Stat Points")]
    public int statPoints = 0; //points you can spend

    [Header("Stats")]
    public int strength = 0; //+damage
    public int defense = 0; //+damage reduction
    public int moveSpeed = 0; //move speed
    public int ultStat = 0; //+ult charge per hit

    //values from stat sheet that was previously calculated
    private const float strengthPercent = 0.0386f;
    private const float defensePercent = 0.0138f;
    private const float moveSpeedPercent = 0.007f;
    private const float ultChargePercent = 0.0104f;

    //called by PlayerXP when leveling up
    public void AddStatPoints(int amount)
    {
        statPoints += amount;
        Debug.Log("Stat Points: " + statPoints);
    }

    //temp test controls
    //These let you spend stat points without making UI yet
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) UpgradeStrength();
        if (Input.GetKeyDown(KeyCode.Alpha2)) UpgradeDefense();
        if (Input.GetKeyDown(KeyCode.Alpha3)) UpgradeMoveSpeed();
        if (Input.GetKeyDown(KeyCode.Alpha4)) UpgradeUltimate();
    }

    //spend one point on strength
    public void UpgradeStrength()
    {
        if (statPoints <= 0) return;

        statPoints--;
        strength++;

        Debug.Log("Strength: " + strength);
    }

    //spend one point on defense
    public void UpgradeDefense()
    {
        if (statPoints <= 0) return;

        statPoints--;
        defense++;

        Debug.Log("Defense: " + defense);
    }

    //spend one point on move speed
    public void UpgradeMoveSpeed()
    {
        if (statPoints <= 0) return;

        statPoints--;
        moveSpeed++;

        Debug.Log("Move Speed: " + moveSpeed);
    }

    //spend one point on ult charge
    public void UpgradeUltimate()
    {
        if (statPoints <= 0) return;

        statPoints--;
        ultStat++;

        Debug.Log("Ultimate Stat: " + ultStat);
    }

    //the functions below convert stat values into gameplay effects
    //other scripts (player attack, player health, etc.) will call these

    //strength increases damage
    public float GetDamageMultiplier()
    {
        return 1f + (strength * strengthPercent);
    }

    //defense reduces incoming damage
    public float GetDefenseMultiplier()
    {
        return 1f - (defense * defensePercent);
    }

    public float GetMoveSpeedMultiplier()
    {
        return 1f+ (moveSpeed * moveSpeedPercent);
    }

    public float GetUltChargeMultiplier()
    {
        return 1f + (ultStat * ultChargePercent);
    }
}