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

    [Header("Stat Value Text")]
    public TextMeshProUGUI strengthValueText;
    public TextMeshProUGUI defenseValueText;
    public TextMeshProUGUI moveSpeedValueText;
    public TextMeshProUGUI ultimateValueText;

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

        if (pointsText) pointsText.text = "Stat Points: " + playerStats.statPoints;

        if (strengthValueText) strengthValueText.text = "Strength: " + playerStats.strength;
        if (defenseValueText) defenseValueText.text = "Defense: " + playerStats.defense;
        if (moveSpeedValueText) moveSpeedValueText.text = "Move Speed: " + playerStats.moveSpeed;
        if (ultimateValueText) ultimateValueText.text = "Ultimate: " + playerStats.ultStat;

        bool canUpgrade = playerStats.statPoints > 0;

        if (strengthButton) strengthButton.interactable = canUpgrade;
        if (defenseButton) defenseButton.interactable = canUpgrade;
        if (moveSpeedButton) moveSpeedButton.interactable = canUpgrade;
        if (ultButton) ultButton.interactable = canUpgrade;
    }

    //Button callbacks ----
    public void SpendStrength()
    {
        if (playerStats == null) return;

        playerStats.UpgradeStrength();

        if (RunManager.Instance != null)
            RunManager.Instance.stats.strength = playerStats.strength;

        if (RunManager.Instance != null)
            RunManager.Instance.stats.statPoints = playerStats.statPoints;

        RefreshUI();
    }

    public void SpendDefense()
    {
        if (playerStats == null) return;

        playerStats.UpgradeDefense();

        if (RunManager.Instance != null)
        {
            RunManager.Instance.stats.defense = playerStats.defense;
            RunManager.Instance.stats.statPoints = playerStats.statPoints;
        }

        RefreshUI();
    }

    public void SpendMoveSpeed()
    {
        if (playerStats == null) return;

        playerStats.UpgradeMoveSpeed();

        if (RunManager.Instance != null)
        {
            RunManager.Instance.stats.moveSpeed = playerStats.moveSpeed;
            RunManager.Instance.stats.statPoints = playerStats.statPoints;
        }

        RefreshUI();
    }

    public void SpendUltimate()
    {
        if (playerStats == null) return;

        playerStats.UpgradeUltimate();

        if (RunManager.Instance != null)
        {
            RunManager.Instance.stats.ultStat = playerStats.ultStat;
            RunManager.Instance.stats.statPoints = playerStats.statPoints;
        }

        RefreshUI();
    }

    public void RestartLevel()
    {
        Debug.Log("RestartLevel clicked");

        Hide();
        // Unpause before reload
        Time.timeScale = 1f;

        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}



