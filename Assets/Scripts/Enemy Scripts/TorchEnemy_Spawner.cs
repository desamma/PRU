using UnityEngine;

public class TorchEnemy_Spawner : MonoBehaviour
{
    [SerializeField] private GameObject enemy_prefabs;
    [SerializeField] private float minimumSpawnTime;
    [SerializeField] private float maximumSpawnTime;
    [SerializeField] public int maximumEnemy;
    [SerializeField] private float spawnRadius = 2f; // Smaller radius for spawning enemies

    private float timeUntilSpawn;
    private int currentEnemyCount;
    private float detectionRadius;
    private bool playerInRange = false;

    private void Awake()
    {
        SetTimeUntilSpawn();

        // Get the detection radius from the colliderlider
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        detectionRadius = collider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);

        // Optional: Clamp spawn radius to always be smaller than detection
        spawnRadius = Mathf.Min(spawnRadius, detectionRadius);
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
        if (!playerInRange) return;

        if (currentEnemyCount < maximumEnemy)
        {
            timeUntilSpawn -= Time.deltaTime;

            if (timeUntilSpawn <= 0)
            {
                Vector2 offset = Random.insideUnitCircle * spawnRadius;
                Vector3 spawnPosition = transform.position + new Vector3(offset.x, offset.y, 0f);

                GameObject newEnemy = Instantiate(enemy_prefabs, spawnPosition, Quaternion.identity);
                currentEnemyCount++;

                // Subscribe to death/destroy event
                if (newEnemy.TryGetComponent<Enemy_Health>(out var enemyHealth))
                {
                    enemyHealth.OnEnemyDestroyed += HandleEnemyDestroyed;
                }

                SetTimeUntilSpawn();
            }
        }
    }

    private void HandleEnemyDestroyed()
    {
        currentEnemyCount--;
    }

    private void SetTimeUntilSpawn()
    {
        timeUntilSpawn = Random.Range(minimumSpawnTime, maximumSpawnTime);
    }

    /// <summary>
    /// the method will scale the gizmo circle when:
    ///     scale the Spawner GameObject(Transform > Scale)
    ///     scale the parent of the Spawner(lossyScale includes parent scale)
    ///     move the collider's offset (circle shift its center accordingly)
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!TryGetComponent<CircleCollider2D>(out var collider)) return;

        // Calculate world-space center using offset and transform
        Vector3 offset = new(collider.offset.x * transform.lossyScale.x, collider.offset.y * transform.lossyScale.y, 0f);
        Vector3 worldCenter = transform.position + offset;

        // Draw spawn area (red), assuming it's always centered at worldCenter
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(worldCenter, spawnRadius);
    }
}
