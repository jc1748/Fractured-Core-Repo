using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "PlayerTesting"; // your gameplay scene

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
        Debug.Log("Start button clicked");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
