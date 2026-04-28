using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Stat Points")]
    public int statPoints
    {
        get => RunManager.Instance.stats.statPoints;
        set => RunManager.Instance.stats.statPoints = value;
    }

    [Header("Stats")]
    public int strength
    {
        get => RunManager.Instance.stats.strength;
        set => RunManager.Instance.stats.strength = value;
    }

    public int defense
    {
        get => RunManager.Instance.stats.defense;
        set => RunManager.Instance.stats.defense = value;
    }

    public int moveSpeed
    {
        get => RunManager.Instance.stats.moveSpeed;
        set => RunManager.Instance.stats.moveSpeed = value;
    }

    public int ultStat
    {
        get => RunManager.Instance.stats.ultStat;
        set => RunManager.Instance.stats.ultStat = value;
    }

    //values from stat sheet that was previously calculated
    private const float strengthPercent = 0.04f;
    private const float defensePercent = 0.02f;
    private const float moveSpeedPercent = 0.010f;
    private const float ultChargePercent = 0.015f;
   
    //called by PlayerXP when leveling up
    public void AddStatPoints(int amount)
    {
        statPoints += amount;
        Debug.Log("Stat Points: " + statPoints);
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