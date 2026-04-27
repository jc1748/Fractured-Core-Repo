using UnityEngine;

public class WinItem : MonoBehaviour
{
    public CanvasGroup winUI;

    private bool hasWon = false;

    void Start()
    {
        if (winUI == null)
        {
            winUI = GameObject.Find("WinPanel").GetComponent<CanvasGroup>();
        }

        HideWinUI();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasWon) return;

        if (other.CompareTag("Player"))
        {
            WinGame();
        }
    }

    void WinGame()
    {
        hasWon = true;

        Debug.Log("Player collected win item. Game won!");

        Time.timeScale = 0f;

        ShowWinUI();

        gameObject.SetActive(false);
    }

    void ShowWinUI()
    {
        if (winUI == null)
        {
            Debug.LogWarning("WinItem: Win UI not found.");
            return;
        }

        winUI.alpha = 1f;
        winUI.interactable = true;
        winUI.blocksRaycasts = true;
    }

    void HideWinUI()
    {
        if (winUI == null) return;

        winUI.alpha = 0f;
        winUI.interactable = false;
        winUI.blocksRaycasts = false;
    }
}