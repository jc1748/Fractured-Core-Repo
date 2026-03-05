using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs; //drag prefabs here(elite, soldier, grunt)

    [Header("Spawn Points")]
    public Transform[] spawnPoints;  //empty objects where enemies appear

    [Header("Spawn Settings")]
    public int enemiesToSpawn = 3; //how many enemies appear

    private bool hasSpawned = false;

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        if (hasSpawned) return;

        for(int i = 0; i < enemiesToSpawn; i++)
        {
            //pick random enemy type
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            //pick random location
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        }

        hasSpawned = true;
    }

}
