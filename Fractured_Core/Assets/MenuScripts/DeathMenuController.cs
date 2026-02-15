using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathMenuController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel; //drag death menu panel here
    public Text pointsText; //drag points text here

    [Header("Buttons")]
    public Button strengthButton;
    public Button defenseButton;
    public Button moveSpeedButton;
    public Button ultButton;
    public Button restartButton;

    //reference to players stats
    private PlayerStats playerStats;

    void Awake()
    {
        if(panel == null)
        {
            panel = gameObject;
        }
        panel.SetActive(false);
    }

    //call this when player dies
    public void Show(PlayerStats stats)
    {
        playerStats = stats;

        Time.timeScale = 0f;

        panel.SetActive(true);
        RefreshUI();
    }

    //updates the text + enables/disables buttons
    void RefreshUI()
    {
        if (playerStats == null) return;

        //update points text
        if (pointsText != null)
        {
            pointsText.text = "Stat Points: " + playerStats.statPoints;
        }

        //if no points, disable upgrade buttons
        bool canSpend = playerStats.statPoints > 0;

        if (strengthButton != null) strengthButton.interactable = canSpend;
        if (defenseButton != null) defenseButton.interactable = canSpend;
        if (moveSpeedButton != null) moveSpeedButton.interactable = canSpend;
        if (ultButton != null) ultButton.interactable = canSpend;
        
    }

    //button callbacks

    public void SpendStrength()
    {
        if (playerStats == null)
        {
            return;
        }
        playerStats.UpgradeStrength();
        RefreshUI();
    }

    public void SpendDefense()
    {
        if (playerStats == null)
        {
            return;
        }
        playerStats.UpgradeDefense();
        RefreshUI();
    }
    public void SpendMoveSpeed()
    {
        if (playerStats == null)
        {
            return;
        }
        playerStats.UpgradeMoveSpeed();
        RefreshUI();
    }
    public void SpendUltimate()
    {
        if (playerStats == null)
        {
            return;
        }
        playerStats.UpgradeUltimate();
        RefreshUI();
    }

    public void RestartLevel()
    {
        //unpause before reloading
        Time.timeScale = 1f;

        //reload the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}



