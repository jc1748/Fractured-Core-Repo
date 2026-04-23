using System.Collections.Generic;
using UnityEngine;

public class ArenaTrigger : MonoBehaviour
{
    [Header("Arena Walls")]
    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject bottomWall;

    [Header("Enemies In This Arena")]
    public List<GameObject> enemiesInArena = new List<GameObject>();

    [Header("Camera")]
    public CameraFollow cameraFollow;

    private bool arenaActive = false;

    private void Start()
    {
        if (leftWall != null) leftWall.SetActive(false);
        if (rightWall != null) rightWall.SetActive(false);
        if (bottomWall  != null) bottomWall.SetActive(false);

        if (cameraFollow == null && Camera.main != null)
        {
            cameraFollow = Camera.main.GetComponent<CameraFollow>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (arenaActive) return;

        if (other.CompareTag("Player"))
        {
            StartArena();
        }
    }

    void StartArena()
    {
        arenaActive = true;

        if (leftWall != null) leftWall.SetActive(true);
        if (rightWall != null) rightWall.SetActive(true);
        if (bottomWall !=null) bottomWall.SetActive(true);

        if (cameraFollow != null)
        {
            cameraFollow.followEnabled = false;
        }
    }

    void Update()
    {
        if (!arenaActive) return;

        enemiesInArena.RemoveAll(enemy => enemy == null);

        if (enemiesInArena.Count == 0)
        {
            EndArena();
        }
    }

    void EndArena()
    {
        arenaActive = false;

        if (leftWall != null) leftWall.SetActive(false);
        if (rightWall != null) rightWall.SetActive(false);
        if (bottomWall!=null) bottomWall.SetActive(false);

        if (cameraFollow != null)
        {
            cameraFollow.followEnabled = true;
        }

        gameObject.SetActive(false);
    }
}