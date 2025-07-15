using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public enum Enemy_Necromage_State
{
    Idle,
    Patrol,
    Knockback,
    Chase,
    Attack1,
    Attack2,
    Attack3,
    TakeDamage,
    Dying
}

public class Enemy_Necromage_Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 1f;
    public float attackCooldown = 2f;
    public float attackRange = 8f; // Single attack range for magical attacks
    public float playerDetectionRange = 10f; // Longer detection for a mage
    public Transform detectionPoint;
    public Transform attackPoint;
    public LayerMask playerLayer;

    [Header("Necromage Attack Settings")]
    public GameObject magicProjectile;
    public GameObject lightningPrefab;
    public GameObject curseOrb;
    public float projectileSpeed = 10f;
    public float projectileLifeTime = 5f; // Add lifetime for projectiles
    public int maxProjectiles = 10; // Add max projectile limit
    public float takeDamageDuration = 0.5f;
    public float attackStateDuration = 1f;

    [Header("Patrol Settings")]
    private Vector3[] patrolPoints;
    public float patrolDistance;
    int currentPatrolIndex = 0;
    private bool isWaiting = false;
    [SerializeField] private float idleToPatrolWaitTime;
    private float unstuckPatrolWaitTime = 1f;
    private float unstuckPatrolWaitTimer;
    private float waitTimer = 0f;

    // projectile management
    private readonly List<GameObject> activeProjectiles = new();

    private Vector3 originalPosition;
    private float attackCooldownTimer;
    private int facingDirection;
    private Enemy_Necromage_State enemyState;
    private float takeDamageTimer;
    private float attackStateTimer;

    private Rigidbody2D rb;
    private Transform player;
    private Animator animator;
    private Enemy_Health health;

    private void Awake()
    {
        originalPosition = transform.position;

        patrolPoints = new Vector3[4];
        patrolPoints[0] = originalPosition + Vector3.up * patrolDistance;
        patrolPoints[1] = originalPosition + Vector3.down * patrolDistance;
        patrolPoints[2] = originalPosition + Vector3.left * patrolDistance;
        patrolPoints[3] = originalPosition + Vector3.right * patrolDistance;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = GetComponent<Enemy_Health>();
        ChangeState(Enemy_Necromage_State.Idle);

        unstuckPatrolWaitTime *= patrolDistance / 2;
        unstuckPatrolWaitTimer = unstuckPatrolWaitTime;

        facingDirection = transform.localScale.x > 0 ? 1 : -1;
    }

    private void Update()
    {
        if (enemyState == Enemy_Necromage_State.Dying) return;

        // Clean up destroyed projectiles from the list
        activeProjectiles.RemoveAll(projectile => projectile == null);

        // If health is zero, start dying
        if (health != null && health.IsDead)
        {
            rb.velocity = Vector2.zero;
            ChangeState(Enemy_Necromage_State.Dying);
            return;
        }

        // Handle take damage state
        if (enemyState == Enemy_Necromage_State.TakeDamage)
        {
            takeDamageTimer -= Time.deltaTime;
            if (takeDamageTimer <= 0)
            {
                // Return to appropriate state after taking damage
                if (player != null)
                {
                    float distanceToPlayer = Vector2.Distance(transform.position, player.position);
                    if (distanceToPlayer <= attackRange)
                    {
                        ChangeState(Enemy_Necromage_State.Idle);
                    }
                    else
                    {
                        ChangeState(Enemy_Necromage_State.Patrol);
                    }
                }
                else
                {
                    ChangeState(Enemy_Necromage_State.Patrol);
                }
            }
            return;
        }

        // Handle attack state timers
        if (enemyState == Enemy_Necromage_State.Attack1 ||
            enemyState == Enemy_Necromage_State.Attack2 ||
            enemyState == Enemy_Necromage_State.Attack3)
        {
            attackStateTimer -= Time.deltaTime;
            if (attackStateTimer <= 0)
            {
                // Return to idle after attack animation
                ChangeState(Enemy_Necromage_State.Idle);
            }
            return;
        }

        if (enemyState != Enemy_Necromage_State.Knockback)
        {
            if (attackCooldownTimer > 0)
                attackCooldownTimer -= Time.deltaTime;

            CheckForPlayer();

            switch (enemyState)
            {
                case Enemy_Necromage_State.Chase:
                    Chase();
                    break;
                case Enemy_Necromage_State.Patrol:
                    Patrol();
                    break;
                case Enemy_Necromage_State.Idle:
                    rb.velocity = Vector2.zero;
                    break;
            }
        }
    }

    private void Chase()
    {
        if ((player.position.x > transform.position.x && facingDirection == -1) ||
            (player.position.x < transform.position.x && facingDirection == 1))
        {
            Flip();
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * speed;
    }

    private void Patrol()
    {
        if (isWaiting)
        {
            ChangeState(Enemy_Necromage_State.Idle);
            waitTimer += Time.deltaTime;

            if (waitTimer >= idleToPatrolWaitTime)
            {
                isWaiting = false;
                waitTimer = 0f;

                int newIndex;
                do
                {
                    newIndex = Random.Range(0, patrolPoints.Length);
                } while (newIndex == currentPatrolIndex);

                currentPatrolIndex = newIndex;
                ChangeState(Enemy_Necromage_State.Patrol);
            }
            return;
        }

        if (unstuckPatrolWaitTimer > 0)
        {
            unstuckPatrolWaitTimer -= Time.deltaTime;

            Vector3 targetPos = patrolPoints[currentPatrolIndex];
            Vector3 direction = (targetPos - transform.position).normalized;
            rb.velocity = direction * speed;

            if ((targetPos.x > transform.position.x && facingDirection == -1) ||
                (targetPos.x < transform.position.x && facingDirection == 1))
            {
                Flip();
            }

            if (Vector2.Distance(transform.position, targetPos) < 0.2f)
            {
                rb.velocity = Vector2.zero;
                isWaiting = true;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            unstuckPatrolWaitTimer = unstuckPatrolWaitTime;
            isWaiting = true;
        }
    }

    private void Flip()
    {
        facingDirection *= -1;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void CheckForPlayer()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectionRange, playerLayer);
        if (hitColliders.Length > 0)
        {
            player = hitColliders[0].transform;
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            rb.velocity = Vector2.zero;

            // If player is within attack range, randomly choose a magical attack
            if (distanceToPlayer <= attackRange)
            {
                if (attackCooldownTimer <= 0)
                {
                    // Randomly choose between the three magical attacks
                    int attackChoice = Random.Range(1, 4);
                    switch (attackChoice)
                    {
                        case 1:
                            ChangeState(Enemy_Necromage_State.Attack1); // FireBall
                            break;
                        case 2:
                            ChangeState(Enemy_Necromage_State.Attack2); // Void Circle
                            break;
                        case 3:
                            ChangeState(Enemy_Necromage_State.Attack3); // Lighting
                            break;
                    }
                    attackCooldownTimer = attackCooldown;
                    attackStateTimer = attackStateDuration;
                }
            }
            else
            {
                // If player is too far, chase them to get in range
                ChangeState(Enemy_Necromage_State.Chase);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            ChangeState(Enemy_Necromage_State.Patrol);
        }
    }

    // Call this method when the enemy takes damage
    public void TakeDamage()
    {
        if (enemyState != Enemy_Necromage_State.TakeDamage && enemyState != Enemy_Necromage_State.Dying)
        {
            ChangeState(Enemy_Necromage_State.TakeDamage);
            takeDamageTimer = takeDamageDuration;
        }
    }

    // Enhanced ExecuteAttack1 with projectile management
    public void ExecuteAttack1()
    {
        // Check if we've reached the projectile limit
        if (activeProjectiles.Count >= maxProjectiles)
        {
            return;
        }

        // Check for player in range (similar to Enemy_Shooting)
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        if (hits.Length > 0)
        {
            Transform targetPlayer = hits[0].transform;
            if (targetPlayer != null && magicProjectile != null)
            {
                // Create projectile
                GameObject projectile = Instantiate(magicProjectile, attackPoint.position, Quaternion.identity);
                activeProjectiles.Add(projectile);

                // Calculate direction to player
                Vector2 direction = (targetPlayer.position - attackPoint.position).normalized;

                // Set projectile velocity
                if (projectile.TryGetComponent<Rigidbody2D>(out var projectileRb))
                {
                    projectileRb.velocity = direction * projectileSpeed;
                }

                // Rotate projectile to face movement direction
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

                // Destroy projectile after lifetime and remove from list
                StartCoroutine(DestroyProjectileAfterTime(projectile, projectileLifeTime));

                Debug.Log($"Magic projectile fired! Active projectiles: {activeProjectiles.Count}");
            }
        }
        else
        {
            Debug.Log("No player in attack range to shoot at");
        }
    }

    // projectile cleanup
    private IEnumerator DestroyProjectileAfterTime(GameObject projectile, float time)
    {
        yield return new WaitForSeconds(time);
        if (projectile != null)
        {
            activeProjectiles.Remove(projectile);
            Destroy(projectile);
        }
    }

    public void ExecuteAttack1_AlternativeVersion()
    {
        // Check projectile limit
        if (activeProjectiles.Count >= maxProjectiles)
        {
            return;
        }

        // Use the existing player reference from your detection system
        if (magicProjectile != null && attackPoint != null && player != null)
        {
            GameObject projectile = Instantiate(magicProjectile, attackPoint.position, Quaternion.identity);
            activeProjectiles.Add(projectile);

            Vector2 direction = (player.position - attackPoint.position).normalized;

            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
            if (projectileRb != null)
            {
                projectileRb.velocity = direction * projectileSpeed;
            }

            // Optional rotation
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

            // Cleanup after lifetime
            StartCoroutine(DestroyProjectileAfterTime(projectile, projectileLifeTime));
        }
    }

    public void ExecuteAttack2()
    {
        var lightingPosition = player.position;
        lightingPosition.y += 1f; // Adjust Y position slightly above player

        // Lightning spell at player position
        if (lightningPrefab != null && player != null)
        {
            Instantiate(lightningPrefab, lightingPosition, Quaternion.identity);
        }
    }

    public void ExecuteAttack3()
    {
        // Curse orb at player position
        if (curseOrb != null && player != null)
        {
            Instantiate(curseOrb, player.position, Quaternion.identity);
        }
    }

    public void ChangeState(Enemy_Necromage_State newState)
    {
        // Reset all animator bools
        animator.SetBool("isIdle", false);
        animator.SetBool("isPatrolling", false);
        animator.SetBool("isChasing", false);
        animator.SetBool("isAttacking1", false);
        animator.SetBool("isAttacking2", false);
        animator.SetBool("isAttacking3", false);
        animator.SetBool("isTakingDamage", false);
        animator.SetBool("isDying", false);

        enemyState = newState;

        switch (enemyState)
        {
            case Enemy_Necromage_State.Idle:
                animator.SetBool("isIdle", true);
                rb.velocity = Vector2.zero;
                break;
            case Enemy_Necromage_State.Patrol:
                animator.SetBool("isPatrolling", true);
                break;
            case Enemy_Necromage_State.Chase:
                animator.SetBool("isChasing", true);
                break;
            case Enemy_Necromage_State.Attack1:
                animator.SetBool("isAttacking1", true);
                rb.velocity = Vector2.zero;
                break;
            case Enemy_Necromage_State.Attack2:
                animator.SetBool("isAttacking2", true);
                rb.velocity = Vector2.zero;
                break;
            case Enemy_Necromage_State.Attack3:
                animator.SetBool("isAttacking3", true);
                rb.velocity = Vector2.zero;
                break;
            case Enemy_Necromage_State.TakeDamage:
                animator.SetBool("isTakingDamage", true);
                rb.velocity = Vector2.zero; // Stop movement when taking damage
                break;
            case Enemy_Necromage_State.Dying:
                animator.SetBool("isDying", true);
                StartCoroutine(DestroyAfterAnimation());
                break;
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(1.4f); // Adjust this to your actual death animation length
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (detectionPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(detectionPoint.position, playerDetectionRange);
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}