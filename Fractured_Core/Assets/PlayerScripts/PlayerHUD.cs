using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHUD : MonoBehaviour
{
    public PlayerXP playerXP;
    public PlayerHealth playerHealth;
    public PlayerUltimate playerUltimate;

    public Slider ultSlider;
    public Slider xpSlider;
    public Slider hpSlider;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        FindPlayerReferences();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayerReferences();
    }

    void FindPlayerReferences()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if( player == null)
        {
            Debug.LogWarning("HUD: No player found in scene");
            return;
        }

        playerXP = player.GetComponent<PlayerXP>();
        playerHealth = player.GetComponent<PlayerHealth>();
        playerUltimate = player.GetComponent<PlayerUltimate>();
    }

    void Update()
    {
        if (playerXP != null && xpSlider != null)
        {
            xpSlider.value = (float)playerXP.currentXP / playerXP.xpToNextLevel;
        }

        if(playerHealth != null && hpSlider != null)
        {
            hpSlider.value = (float)playerHealth.currentHealth / playerHealth.maxHealth;
        }

        if(playerUltimate !=null && ultSlider != null)
        {
            ultSlider.value = playerUltimate.currentUltCharge / playerUltimate.maxUlt;
        }
    }
}
