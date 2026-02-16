using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class DeathMenuController : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup canvasGroup; //drag death menu panel here
    public TextMeshProUGUI pointsText; //drag points text here

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
        if(canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        //start hidden (but panel stays active)
        Hide();
    }

    public bool CanShowUpgradeMenu(PlayerStats stats)
    {
        return stats != null && stats.statPoints > 0;
    }

    //call this when player dies
    public void Show(PlayerStats stats)
    {
        playerStats = stats;


        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts= true;
        }

        //update text/buttons
        RefreshUI();
        Debug.Log("Death menu SHOW called");
    }

    public void Hide()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts= false;
        }
    }

    /// Update points text 
    private void RefreshUI()
    {
        if (playerStats == null) return;

        if (pointsText != null && playerStats != null)
        {
            pointsText.text = "Stat Points: " + playerStats.statPoints;

            bool canUpgrade = playerStats.statPoints > 0;

            if(strengthButton) strengthButton.interactable = canUpgrade;
            if(defenseButton) defenseButton.interactable = canUpgrade;
            if(moveSpeedButton) moveSpeedButton.interactable = canUpgrade;
            if(ultButton) ultButton.interactable = canUpgrade;
        }
    }

    //Button callbacks ----
    public void SpendStrength()
    {
        Debug.Log("SpendStrength clicked. playerStats=" + playerStats);
        if (playerStats == null) return;
        playerStats.UpgradeStrength();
        RefreshUI();
    }

    public void SpendDefense()
    {
        Debug.Log("SpendDefense clicked. playerStats=" + playerStats);
        if (playerStats == null) return;
        playerStats.UpgradeDefense();
        RefreshUI();
    }

    public void SpendMoveSpeed()
    {
        Debug.Log("SpendMoveSpeed clicked. playerStats=" + playerStats);
        if (playerStats == null) return;
        playerStats.UpgradeMoveSpeed();
        RefreshUI();
    }

    public void SpendUltimate()
    {
        Debug.Log("SpendUltimate clicked. playerStats=" + playerStats);
        if (playerStats == null) return;
        playerStats.UpgradeUltimate();
        RefreshUI();
    }

    public void RestartLevel()
    {
        Debug.Log("RestartLevel clicked");
        // Unpause before reload
        Time.timeScale = 1f;

        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}



