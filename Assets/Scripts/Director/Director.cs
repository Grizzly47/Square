using System.Collections;
using UnityEngine;

namespace KP
{
    public class GameDirector : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab; // Reference to the enemy prefab
        [SerializeField] private Transform[] spawnPoints; // Array of spawn points
        [SerializeField] private float spawnInterval = 2f; // Time interval between spawns

        private void Start()
        {
            // Start the enemy spawning coroutine
            StartCoroutine(SpawnEnemies());
        }

        private IEnumerator SpawnEnemies()
        {
            while (true)
            {
                // Spawn an enemy at a random spawn point
                SpawnEnemy();

                // Wait for the next spawn interval
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        private void SpawnEnemy()
        {
            // Choose a random spawn point
            int spawnIndex = Random.Range(0, spawnPoints.Length);

            // Get the spawn point and its direction
            Transform spawnPoint = spawnPoints[spawnIndex];
            Vector2 direction = spawnPoint.GetComponent<SpawnPoint>().direction;

            // Instantiate the enemy prefab at the chosen spawn point's position and rotation
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            // Set the enemy's movement direction
            EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
            if (enemyMovement != null)
            {
                enemyMovement.SetDirection(direction);
            }
        }
    }
}

