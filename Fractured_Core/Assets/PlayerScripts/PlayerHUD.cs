using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public PlayerXP playerXP;
    public PlayerHealth playerHealth;
    public PlayerUltimate playerUltimate;

    public Slider ultSlider;
    public Slider xpSlider;
    public Slider hpSlider;

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
            ultSlider.value = playerUltimate.currentUlt / playerUltimate.maxUlt;
        }
    }
}
