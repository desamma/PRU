using UnityEngine;

public class TorchEnemy_Spawner : MonoBehaviour
{
    [SerializeField] private GameObject enemy_prefabs;
    [SerializeField] private float minimumSpawnTime;
    [SerializeField] private float maximumSpawnTime;
    [SerializeField] private int maximumEnemy;
    [SerializeField] private float spawnRadius = 2f;

    private float timeUntilSpawn;
    private int currentEnemyCount;
    private float detectionRadius;
    private bool playerInRange = false;

    [SerializeField] private CircleCollider2D spawnCollider;

    private void Awake()
    {
        spawnCollider = GetComponent<CircleCollider2D>();
        SetTimeUntilSpawn();

        // Get the detection radius from the collider
        detectionRadius = spawnCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);

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

                // Instantiate enemy
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
