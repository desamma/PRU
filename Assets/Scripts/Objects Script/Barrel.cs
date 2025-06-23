using UnityEngine;

public class Barrel : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public int damage = 1;
    public float knockbackForce = 10f;
    public float stunTime = 0.5f;
    public float destroyTime = 1.5f; // Time before the barrel is destroyed after explosion
    public float explosionRadius = 2f; // Radius of the explosion effect
    public float enemySpawnChance = 12f;
    public GameObject enemyPrefab;

    private bool isExploded = false; // Flag to prevent multiple explosions
    
    private void Awake()
    {
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics; // Ensure the animator updates in sync with physics
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("isIdle", true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isExploded) return; // Prevent further explosions if already exploded

        Explode(); // Trigger the explosion
    }

    public void Explode()
    {
        if (isExploded) return; // Prevent multiple explosions

        isExploded = true; // Set the flag to true to prevent further explosions

        // Play the explosion animation
        animator.SetBool("isIdle", false);
        animator.SetBool("isExplode", true);

        // Detect objects within the explosion radius
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hitObject in hitObjects)
        {
            // Apply damage and knockback to the player
            if (hitObject.CompareTag("Player"))
            {
                hitObject.GetComponent<PlayerHealth>().ChangeHealth(-damage);
                hitObject.GetComponent<PlayerMovement>().KnockBack(transform, knockbackForce, stunTime);
            }

            // Apply damage and knockback to the enemy
            if (hitObject.CompareTag("Enemy"))
            {
                hitObject.GetComponent<Enemy_Health>().ChangeHealth(-damage);
                hitObject.GetComponent<Enemy_Knockback>().KnockBack(transform, knockbackForce, stunTime, stunTime);
            }
        }

        SpawnEnemy(); // Attempt to spawn an enemy after explosion
        // Destroy the barrel after a short delay to allow the animation to play
        Destroy(gameObject, destroyTime);

    }

    public void SpawnEnemy()
    {
        // Check if the enemy prefab is assigned
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Enemy prefab is not assigned in the Barrel script.");
            return;
        }

        // Randomly spawn an enemy based on the defined chance
        if (Random.Range(0f, 100f) < enemySpawnChance)
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the explosion radius in the editor for visualization
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
