using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    //player current level
    public int level = 1;

    //how much XP the player currently has toward next level
    public int currentXP = 0;

    //how much XP is required to Level Up
    public int xpToNextLevel = 10;

    //reference to PlayerStats.cs so we can add stat points on Level up
    private PlayerStats playerStats;

    void Awake()
    {
        //get the PlayerStats component from the same GameObject
        playerStats = GetComponent<PlayerStats>();
    }

    //this method is called whenever the player gains XP
    public void AddXP(int amount)
    {
        //prevent adding invalid XP values
        if (amount <= 0)
        {
            return;
        }

        //add XP to the current amount
        currentXP += amount;
        Debug.Log($"XP +{amount} (Total: {currentXP}/{xpToNextLevel})");

        //if we have enough XP, level up--supports multiple levels at once
        while (currentXP >= xpToNextLevel)
        {
            currentXP-=xpToNextLevel;
            LevelUp();
        }
    }

   void LevelUp()
    {
        level++;

        //increase how much Xp we need next time(scaling)
        xpToNextLevel += 5;

        Debug.Log($"LEVEL UP! Level {level}. Next level requires {xpToNextLevel}XP.");

        //give the player 1 stat point on every level up
        if (playerStats != null)
        {
            playerStats.AddStatPoints(1);
        }
    }


}
