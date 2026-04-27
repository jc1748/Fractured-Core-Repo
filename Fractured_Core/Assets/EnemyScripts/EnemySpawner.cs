using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs; //drag prefabs here(elite, soldier, grunt)

    [Header("Spawn Points")]
    public Transform[] spawnPoints;  //empty objects where enemies appear

    [Header("Spawn Settings")]
    public int enemiesToSpawn = 3; //total enemies per wave

    private bool hasSpawned = false;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public void StartSpawning()
    {
        if (hasSpawned) return;

        hasSpawned = true;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // pick random enemy type
            GameObject enemyPrefab = enemyPrefabs[
                Random.Range(0, enemyPrefabs.Length)
            ];

            // pick random spawn point
            Transform spawnPoint = spawnPoints[
                Random.Range(0, spawnPoints.Length)
            ];

            // spawn enemy
            GameObject enemy = Instantiate(
                enemyPrefab,
                spawnPoint.position,
                Quaternion.identity
            );

            // add to list so we can track it
            spawnedEnemies.Add(enemy);
        }
    }

    public bool AllEnemiesDefeated()
    {
        //clean up destroyed enemies
        spawnedEnemies.RemoveAll(enemyPrefabs => enemyPrefabs == null);

        //if list is empty, all enemies are dead
        return hasSpawned && spawnedEnemies.Count == 0;
    }

}
