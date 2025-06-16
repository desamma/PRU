using UnityEngine;

public class Enemy_TNT_Movement : MonoBehaviour
{
    public float speed = 1f;
    public float attackRange = 2f;
    public float playerDetectionRange = 5f;
    public Transform detectionPoint;
    public Transform attackPoint;
    public LayerMask playerLayer;

    //patrolling
    private Vector3[] patrolPoints;
    public float patrolDistance;
    int currentPatrolIndex = 0;
    private bool isWaiting = false;
    [SerializeField] private float idleToPatrolWaitTime;
    private float waitTimer = 0f;

    private Vector3 originalPosition;

    public float attackCooldown = 2f; // Cooldown time for attack
    private float attackCooldownTimer = 2f; // timer for the attack

    private int facingDirection;
    private EnemyState enemyState;
    public float noPlayerTimer = 0f;

    private Rigidbody2D rb;
    private Transform player;
    private Animator animator;
    [SerializeField] private Enemy_Shooting shootingScript;

    private void Awake()
    {
        originalPosition = transform.position;

        patrolPoints = new Vector3[4];
        patrolPoints[0] = originalPosition + Vector3.up * patrolDistance;
        patrolPoints[1] = originalPosition + Vector3.down * patrolDistance;
        patrolPoints[2] = originalPosition + Vector3.left * patrolDistance;
        patrolPoints[3] = originalPosition + Vector3.right * patrolDistance;
    }

    void Start()
    {
        attackCooldownTimer = attackCooldown;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        shootingScript = GetComponent<Enemy_Shooting>();
        ChangeState(EnemyState.Idle);
        facingDirection = transform.localScale.x > 0 ? 1 : -1;
    }

    private void Update()
    {
        Debug.Log($"Attack cooldown timer: {attackCooldownTimer}");
        if (enemyState != EnemyState.Knockback)
        {
            if (attackCooldownTimer > 0)
            {
                attackCooldownTimer -= Time.deltaTime;
            }

            // Check for player first
            CheckForPlayer();

            // Handle different states
            if (enemyState == EnemyState.Chase)
            {
                Chase();
            }
            else if (enemyState == EnemyState.Attack)
            {
                if (attackCooldownTimer > 0)
                {
                    // If cooldown is not met, transition to Idle
                    ChangeState(EnemyState.Idle);
                    rb.velocity = Vector2.zero; // Stop movement
                }
                else
                {
                    rb.velocity = Vector2.zero; // Stop the enemy when attacking

                    // Face the player when attacking
                    if (player != null)
                    {
                        if ((player.position.x > transform.position.x && facingDirection == -1) ||
                            (player.position.x < transform.position.x && facingDirection == 1))
                        {
                            Flip();
                        }
                    }

                    // animation event handle attack
                }
            }
            else if (enemyState == EnemyState.Patrol)
            {
                Patrol();
            }
            else if (enemyState == EnemyState.Idle)
            {
                rb.velocity = Vector2.zero;
            }
        }
    }
    #region Player movement
    void Patrol()
    {
        if (isWaiting)
        {
            ChangeState(EnemyState.Idle);
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
                ChangeState(EnemyState.Patrol);
            }
            return;
        }

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

    void Chase()
    {
        if (player == null) return;

        if ((player.position.x > transform.position.x && facingDirection == -1) ||
            (player.position.x < transform.position.x && facingDirection == 1))
        {
            Flip();
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * speed;
    }

    private void Flip()
    {
        facingDirection *= -1;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
    public void ChangeState(EnemyState newState)
    {
        // Exit current state
        switch (enemyState)
        {
            case EnemyState.Idle:
                animator.SetBool("isIdle", false);
                break;
            case EnemyState.Chase:
                animator.SetBool("isChasing", false);
                break;
            case EnemyState.Attack:
                animator.SetBool("isAttacking", false);
                break;
            case EnemyState.Patrol:
                animator.SetBool("isPatrolling", false);
                break;
        }

        enemyState = newState;

        // Enter new state
        switch (enemyState)
        {
            case EnemyState.Idle:
                animator.SetBool("isIdle", true);
                break;
            case EnemyState.Chase:
                animator.SetBool("isChasing", true);
                break;
            case EnemyState.Attack:
                animator.SetBool("isAttacking", true);
                break;
            case EnemyState.Patrol:
                animator.SetBool("isPatrolling", true);
                break;
        }

        Debug.Log($"Enemy state changed to: {enemyState}");
    }

    public void Attack()
    {
        Debug.Log("Enemy attacks the player!");

        // Call the Shoot() method from the Enemy_Shooting script
        if (shootingScript != null)
        {
            shootingScript.Shoot();
        }
        else
        {
            Debug.LogWarning("Shooting script is not assigned!");
        }

        // Reset the attack cooldown timer
        attackCooldownTimer = attackCooldown;
    }
    #endregion

    private void CheckForPlayer()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectionRange, playerLayer);

        if (hitColliders.Length > 0)
        {
            player = hitColliders[0].transform;
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Priority: Attack > Chase
            if (distanceToPlayer <= attackRange)
            {
                rb.velocity = Vector2.zero;

                // Change to attack state if not already in it
                if (enemyState != EnemyState.Attack && attackCooldownTimer <= 0)
                {
                    ChangeState(EnemyState.Attack);
                }
            }
            else if (distanceToPlayer > attackRange)
            {
                // Player is in detection range but outside attack range - chase them
                //isAttacking = false; // Reset attacking flag when player moves out of range
                if (enemyState != EnemyState.Chase)
                {
                    ChangeState(EnemyState.Chase);
                }
            }
        }
        else
        {
            // No player detected - return to patrol
            Debug.Log("No player detected - returning to patrol");
            player = null;
            //isAttacking = false; // Reset attacking flag when no player detected
            if (enemyState != EnemyState.Patrol && enemyState != EnemyState.Idle)
            {
                rb.velocity = Vector2.zero;
                ChangeState(EnemyState.Patrol);
            }
        }
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
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
