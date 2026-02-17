using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoaderDebug : MonoBehaviour
{
   
    void Update()
    {
        // Make sure these scene names match exactly what’s in Build Settings
        if (Input.GetKeyDown(KeyCode.F1)) SceneManager.LoadScene("PlayerTesting");
        if (Input.GetKeyDown(KeyCode.F2)) SceneManager.LoadScene("IronDistrict");
        if (Input.GetKeyDown(KeyCode.F3)) SceneManager.LoadScene("UndergroundSector");
        if (Input.GetKeyDown(KeyCode.F4)) SceneManager.LoadScene("CentralWard");
    }
}
