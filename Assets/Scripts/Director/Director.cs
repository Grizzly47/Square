using System.Collections;
using UnityEngine;

namespace KP
{
    public class Director : MonoBehaviour
    {
        [SerializeField] private GameObject triangleEnemyPrefab; // Reference to the triangle enemy prefab
        [SerializeField] private GameObject seekingEnemyPrefab; // Reference to the seeking enemy prefab
        [SerializeField] private GameObject warningSignalPrefab; // Reference to the warning signal prefab
        [SerializeField] private float initialSpawnInterval = 2f; // Initial time interval between spawns
        [SerializeField] private float warningTime = 1f; // Time to show warning before spawning
        [SerializeField] private int initialEnemiesPerWave = 5; // Initial number of enemies per wave
        [SerializeField] private float waveInterval = 10f; // Time interval between waves
        [SerializeField] private float speedIncreasePerWave = 0.5f; // Speed increase per wave
        [SerializeField] private float spawnIntervalDecreasePerWave = 0.1f; // Spawn interval decrease per wave

        // Define the boundaries of the spawning area
        [SerializeField] private float minX = -10f;
        [SerializeField] private float maxX = 10f;
        [SerializeField] private float minY = -5f;
        [SerializeField] private float maxY = 5f;

        private Camera mainCamera;
        private float currentEnemySpeed = 1.0f; // Initial enemy speed
        private float currentSpawnInterval;
        private int currentEnemiesPerWave;

        private void Start()
        {
            mainCamera = Camera.main;
            currentSpawnInterval = initialSpawnInterval;
            currentEnemiesPerWave = initialEnemiesPerWave;
            // Start the enemy spawning coroutine
            StartCoroutine(SpawnWaves());
        }

        private IEnumerator SpawnWaves()
        {
            while (true)
            {
                for (int i = 0; i < currentEnemiesPerWave; i++)
                {
                    // Spawn an enemy with a warning signal
                    StartCoroutine(SpawnEnemyWithWarning());

                    // Wait for the next spawn interval
                    yield return new WaitForSeconds(currentSpawnInterval);
                }

                // Wait for the next wave interval
                yield return new WaitForSeconds(waveInterval);

                // Increase difficulty
                currentEnemySpeed += speedIncreasePerWave;
                currentEnemiesPerWave += 1; // Increase enemies per wave
                currentSpawnInterval = Mathf.Max(0.5f, currentSpawnInterval - spawnIntervalDecreasePerWave); // Decrease spawn interval but not below 0.5s
            }
        }

        private IEnumerator SpawnEnemyWithWarning()
        {
            // Generate a random spawn point along the edge within the defined range
            Vector3 spawnPoint = GetRandomSpawnPoint();

            // Instantiate the warning signal at the spawn point
            GameObject warningSignal = Instantiate(warningSignalPrefab, spawnPoint, Quaternion.identity);

            // Wait for the warning time
            yield return new WaitForSeconds(warningTime);

            // Destroy the warning signal
            Destroy(warningSignal);

            // Choose a random enemy type (0 for triangle, 1 for seeking circle)
            int enemyType = Random.Range(0, 2);

            // Instantiate the enemy prefab based on the chosen type
            GameObject enemyPrefab = (enemyType == 0) ? triangleEnemyPrefab : seekingEnemyPrefab;
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);

            // Set the enemy's speed
            if (enemyType == 0)
            {
                // Triangle enemy movement
                Vector2 direction = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition) - (Vector2)spawnPoint;
                direction.Normalize();

                EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
                if (enemyMovement != null)
                {
                    enemyMovement.SetDirection(direction);
                    enemyMovement.SetSpeed(currentEnemySpeed);
                }
            }
            else
            {
                // Seeking enemy movement
                CircleEnemy enemyMovement = enemy.GetComponent<CircleEnemy>();
                if (enemyMovement != null)
                {
                    enemyMovement.SetSpeed(currentEnemySpeed);
                }
            }
        }

        private Vector3 GetRandomSpawnPoint()
        {
            float x, y;
            int edge = Random.Range(0, 4);

            switch (edge)
            {
                case 0: // Top
                    x = Random.Range(minX, maxX);
                    y = maxY;
                    break;
                case 1: // Bottom
                    x = Random.Range(minX, maxX);
                    y = minY;
                    break;
                case 2: // Left
                    x = minX;
                    y = Random.Range(minY, maxY);
                    break;
                case 3: // Right
                    x = maxX;
                    y = Random.Range(minY, maxY);
                    break;
                default:
                    x = Random.Range(minX, maxX);
                    y = Random.Range(minY, maxY);
                    break;
            }

            return new Vector3(x, y, 0);
        }
    }
}