using UnityEngine;

public class RunManager : MonoBehaviour
{
   public static RunManager Instance { get; private set; }

    public PlayerStatsData stats = new PlayerStatsData();

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //call when starting a new run
    public void ResetRun()
    {
        stats = new PlayerStatsData();
    }
}
[System.Serializable]
public class PlayerStatsData
{
    public int statPoints = 0;
    public int strength = 0;
    public int defense = 0;
    public int moveSpeed = 0;
    public int ultStat = 0;
}
