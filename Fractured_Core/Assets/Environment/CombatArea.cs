using UnityEngine;

public class CombatArea : MonoBehaviour
{
    [Header("Camera")]
    public CameraController cameraController;
    public Transform cameraLockPoint;

    [Header("Enemies already placed in this combat area")]
    public GameObject[] enemies;

    [Header("Settings")]
    public bool startOnSceneLoad = false;

    private bool combatStarted = false;
    private bool combatFinished = false;

    [Header("Combat Walls")] 
    public GameObject leftWall; 
    public GameObject rightWall;

    void Start()
    {
        // Hide enemies until combat starts, unless this is the first area
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
                enemy.SetActive(false);
        }

        if (startOnSceneLoad)
        {
            StartCombat();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (combatStarted || combatFinished)
            return;

        if (other.CompareTag("Player"))
        {
            StartCombat();
        }
    }

    void StartCombat()
    {
        combatStarted = true;

        Debug.Log("Combat started. Turning walls ON.");

        if (cameraController == null)
        {
            cameraController = FindFirstObjectByType<CameraController>();
        }

        if (cameraController != null)
        {
            cameraController.LockCamera(cameraLockPoint.position);
        }
        else
        {
            Debug.LogWarning("CombatArea: No CameraController found.");
        }

        if (leftWall != null) leftWall.SetActive(true);
        if (rightWall != null) rightWall.SetActive(true);

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
                enemy.SetActive(true);
        }


    }

    void Update()
    {
        if (combatStarted && !combatFinished)
        {
            if (AllEnemiesDefeated())
            {
                EndCombat();
            }
        }
    }
    
    bool AllEnemiesDefeated()
    {
        foreach(GameObject enemy in enemies)
    {
            if (enemy != null && enemy.activeInHierarchy)
                return false;
        }

        return true;
    }
    

    void EndCombat()
    {
        combatFinished = true;

        Debug.Log("Combat ended. Turning walls OFF.");

        if (leftWall != null) leftWall.SetActive(false);
        if (rightWall != null) rightWall.SetActive(false);

        if (cameraController == null)
            cameraController = FindFirstObjectByType<CameraController>();

        if (cameraController != null)
        {
            cameraController.UnlockCamera();
            Debug.Log("Camera unlocked.");
        }
        else
        {
            Debug.LogWarning("CombatArea: No CameraController found when ending combat.");
        }

    }
}
