using System.Collections.Generic;
using UnityEngine;

public enum EnemyPoint
{
    Melee = 1,
    Ranged = 2,
    TorchBoss = 5,
    Dragon = 10
}

public class TorchEnemy_Spawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemy_prefabs; // List of enemy prefabs
    [SerializeField] private float minimumSpawnTime;
    [SerializeField] private float maximumSpawnTime;
    [SerializeField] private int maximumPoint; // Maximum points allowed for spawning

    [SerializeField] private float spawnRadius = 2f;
    private bool spawningStopped = false; // Flag to stop spawning
    private float timeUntilSpawn;
    private int currentPoint; // Current total points of spawned enemies
    private float detectionRadius;
    private bool playerInRange = false;

    [SerializeField] private CircleCollider2D spawnCollider;

    // Dictionary to map enemy prefabs to their point values
    private Dictionary<GameObject, EnemyPoint> enemyPoints;

    private void Awake()
    {
        spawnCollider = GetComponent<CircleCollider2D>();
        SetTimeUntilSpawn();

        // Get the detection radius from the collider
        detectionRadius = spawnCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);

        // Clamp spawn radius to always be smaller than detection
        spawnRadius = Mathf.Min(spawnRadius, detectionRadius);

        // Initialize the enemy points dictionary
        InitializeEnemyPoints();
    }

    private void InitializeEnemyPoints()
    {
        enemyPoints = new Dictionary<GameObject, EnemyPoint>();

        // Assign points to each prefab in the list based on its index
        for (int i = 0; i < enemy_prefabs.Count; i++)
        {
            switch (i)
            {
                case 0: // Melee
                    enemyPoints[enemy_prefabs[i]] = EnemyPoint.Melee;
                    break;
                case 1: // Ranged
                    enemyPoints[enemy_prefabs[i]] = EnemyPoint.Ranged;
                    break;
                case 2: // TorchBoss
                    enemyPoints[enemy_prefabs[i]] = EnemyPoint.TorchBoss;
                    break;
                case 3: // Dragon
                    enemyPoints[enemy_prefabs[i]] = EnemyPoint.Dragon;
                    break;
                default:
                    Debug.LogWarning($"Enemy prefab at index {i} does not have a defined point value.");
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (!playerInRange || spawningStopped) return;

        if (currentPoint < maximumPoint)
        {
            timeUntilSpawn -= Time.deltaTime;

            if (timeUntilSpawn <= 0)
            {
                SpawnEnemy();
                SetTimeUntilSpawn();
            }
        }
        else
        {
            spawningStopped = true; // Stop spawning permanently
        }
    }

    private void SpawnEnemy()
    {
        // Compute spawn center using collider offset and scale
        Vector3 colliderOffset = new(
            spawnCollider.offset.x * transform.lossyScale.x,
            spawnCollider.offset.y * transform.lossyScale.y,
            0f
        );
        Vector3 spawnCenter = transform.position + colliderOffset;

        // Generate random offset within spawn radius
        Vector2 offset2D = Random.insideUnitCircle * spawnRadius;
        Vector3 offset3D = new(offset2D.x, offset2D.y, 0f);

        Vector3 spawnPosition = spawnCenter + offset3D;

        // Select a random enemy prefab that fits within the remaining points
        GameObject selectedEnemy = GetRandomEnemyPrefab();

        if (selectedEnemy != null)
        {
            // Instantiate the enemy
            GameObject newEnemy = Instantiate(selectedEnemy, spawnPosition, Quaternion.identity);

            // Get the enemy's point value and add it to the current total
            currentPoint += (int)enemyPoints[selectedEnemy];

            // Subscribe to the enemy's destroy event with lambda to pass the enemy reference
            if (newEnemy.TryGetComponent<Enemy_Health>(out var enemyHealth))
            {
                //when this enemy's OnEnemyDestroyed event happens, call HandleEnemyDestroyed(newEnemy).
                enemyHealth.OnEnemyDestroyed += () => HandleEnemyDestroyed(newEnemy);
            }
        }
    }

    private GameObject GetRandomEnemyPrefab()
    {
        // Filter the list of enemy prefabs to only include those that fit within the remaining points
        int remainingPoints = maximumPoint - currentPoint;

        List<GameObject> validEnemies = enemy_prefabs.FindAll(index =>
        {
            return (int)enemyPoints[index] <= remainingPoints;
        });

        // Return a random valid enemy, or null if no valid enemies are available
        if (validEnemies.Count > 0)
        {
            return validEnemies[Random.Range(0, validEnemies.Count)];
        }

        return null;
    }

    private void HandleEnemyDestroyed(GameObject destroyedEnemy)
    {
        // Find the prefab that matches this enemy instance
        foreach (var kvp in enemyPoints)
        {
            if (destroyedEnemy.name.StartsWith(kvp.Key.name)) // Unity adds "(Clone)" to instantiated objects
            {
                currentPoint -= (int)kvp.Value;
                break;
            }
        }
    }

    private void SetTimeUntilSpawn()
    {
        timeUntilSpawn = Random.Range(minimumSpawnTime, maximumSpawnTime);
    }

    private void OnDrawGizmosSelected()
    {
        if (!TryGetComponent<CircleCollider2D>(out var collider)) return;

        // Calculate world-space center using offset and transform
        Vector3 offset = new(
            spawnCollider.offset.x * transform.lossyScale.x,
            spawnCollider.offset.y * transform.lossyScale.y,
            0f
        );
        Vector3 worldCenter = transform.position + offset;

        // Draw spawn area (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(worldCenter, spawnRadius);
    }
}
