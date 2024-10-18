using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab; // Reference to the zombie prefab
    public int poolSize = 10; // Maximum number of zombies in the pool
    public float spawnIntervalMin = 2f; // Minimum time between spawns
    public float spawnIntervalMax = 5f; // Maximum time between spawns
    public Vector3 spawnArea = new Vector3(10f, 0f, 10f); // Size of the spawn area

    private List<GameObject> zombiePool; // List to store zombie pool

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the zombie pool
        zombiePool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject zombie = Instantiate(zombiePrefab);
            zombie.SetActive(false); // Deactivate initially
            zombiePool.Add(zombie);
        }

        StartCoroutine(SpawnZombies());
    }

    // Coroutine for random zombie spawning
    IEnumerator SpawnZombies()
    {
        while (true)
        {
            // Random spawn interval
            float spawnInterval = Random.Range(spawnIntervalMin, spawnIntervalMax);
            yield return new WaitForSeconds(spawnInterval);

            // Spawn a zombie from the pool
            SpawnZombieFromPool();
        }
    }

    // Function to spawn a zombie from the pool
    void SpawnZombieFromPool()
    {
        // Find an inactive zombie in the pool
        foreach (GameObject zombie in zombiePool)
        {
            if (!zombie.activeInHierarchy) // Check if the zombie is inactive
            {
                // Random spawn position within the defined spawn area
                Vector3 spawnPosition = new Vector3(
                    Random.Range(-spawnArea.x, spawnArea.x),
                    0f,  // Assuming flat terrain
                    Random.Range(-spawnArea.z, spawnArea.z)
                );

                // Activate the zombie and set its position
                zombie.transform.position = spawnPosition;
                zombie.transform.rotation = Quaternion.identity;
                zombie.SetActive(true);
                break; // Exit loop after finding an inactive zombie
            }
        }
    }

    // Draw the spawn area in the scene view (for visualization)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, spawnArea);
    }
}
