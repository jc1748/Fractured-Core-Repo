using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    //player current level
    public int level = 1;

    //how much XP the player currently has toward next level
    public int currentXP = 0;

    //how much XP is required to Level Up
    public int xpToNextLevel = 10;

    //reference to PlayerAttack.cs so we can upgrade damage on Level up
    private PlayerAttack playerAttack;

    void Awake()
    {
        //get the PlayerAttack component from the same GameObject
        playerAttack = GetComponent<PlayerAttack>();
    }

    //this method is called whenever the player gains XP
    public void AddXP(int amount)
    {
        //prevent adding invalid XP values
        if (amount <= 0)
        {
            return;
        }

        //add Xp to the player's total
        currentXP += amount;
        Debug.Log($"Gained {amount} XP. Total XP: {currentXP}");

        //check if the player has enough XP to level up
        CheckLevelUp();
    }

    void CheckLevelUp()
    {
        //using a while loop in case the player gains enough
        //xp to level up multiple times at once

        while (currentXP >= xpToNextLevel)
        {
            //subtract the Xp needed for this level
            currentXP -= xpToNextLevel;

            //increase player level
            level++;
            Debug.Log("LEVEL UP! Player is now level "+ level);

            //increase XP required for the next level
            //(simple scaling systm)
            xpToNextLevel += 5;

            //Apply a reward for leveling up
            ApplyLevelReward();
        }
    }

    void ApplyLevelReward()
    {
        //for now, leveling up increases attack damage
        if(playerAttack != null)
        {
            playerAttack.attackDamage += 1;
            Debug.Log("Attack damage increased to "+ playerAttack.attackDamage);
        }
    }


}
