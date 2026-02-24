using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public DeathMenuController deathMenu;
    public DeathFlowController deathFlow;
    public CanvasGroup fadeCanvasGroup;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ALWAYS unpause after scene loads
        Time.timeScale = 1f;

        // Hide the menu
        if (deathMenu != null)
            deathMenu.Hide();

        // Reset the death flow (clears isDying + coroutines + fade)
        if (deathFlow != null)
            deathFlow.ResetFlowState();


        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.currentHealth = health.maxHealth;
            }

            var xp = player.GetComponent<PlayerXP>();
            if(xp != null)
            {
                xp.ResetXPProgress();
            }

            var ult = player.GetComponent<PlayerUltimate>();
            if(ult != null)
            {
                ult.ResetUlt();
            }
        }
    }
}
