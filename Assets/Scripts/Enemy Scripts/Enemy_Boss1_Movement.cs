using UnityEngine;

public enum Boss1State
{
    Idle,
    Patrol,
    Knockback,
    Chase,
    Attack,
    RangedAttack
};

public class Enemy_Boss1_Movement : MonoBehaviour
{
    public float speed = 1f;
    public float attackRangeMelee = 2f;
    public float attackCooldown = 2f;
    public float attackRangeRanged = 5f;
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
    private float unstuckPatrolWaitTime = 1f;
    private float unstuckPatrolWaitTimer;
    private float waitTimer = 0f;

    private Vector3 originalPosition;
    private float attackCooldownTimer;
    private int facingDirection;
    private Boss1State enemyState;

    private Rigidbody2D rb;
    private Transform player;
    private Animator animator;

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
        rb = GetComponent<Rigidbody2D>();
        //audioClip = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        ChangeState(Boss1State.Idle);
        unstuckPatrolWaitTime *= patrolDistance / 2;
        unstuckPatrolWaitTimer = unstuckPatrolWaitTime;
        //originalPosition = transform.position; // Store the original position for potential patrol logic
        facingDirection = transform.localScale.x > 0 ? 1 : -1; // Determine initial facing direction based on local scale
    }
    private void Update()
    {
        if (enemyState != Boss1State.Knockback)
        {
            if (attackCooldownTimer > 0)
                attackCooldownTimer -= Time.deltaTime;
            CheckForPlayer();

            switch (enemyState)
            {
                case Boss1State.Chase:
                    Chase();
                    break;
                case Boss1State.Attack:
                    rb.velocity = Vector2.zero;
                    break;
                case Boss1State.RangedAttack:
                    {
                        if (attackCooldownTimer > 0)
                        {
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
                        }
                    }
                    break;
                case Boss1State.Patrol:
                    Patrol();
                    break;
                case Boss1State.Idle:
                    rb.velocity = Vector2.zero;
                    break;
            }
        }
    }

    void Chase()
    {
        if (player.position.x > transform.position.x && facingDirection == -1 ||
                player.position.x < transform.position.x && facingDirection == 1)
        {
            Flip();
        }
        // Allow to use speed to control the enemy's movement speed
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * speed;
        //rb.MovePosition(rb.position + rb.velocity * Time.fixedDeltaTime);
    }

    void Patrol()
    {
        //if waiting, idle
        if (isWaiting)
        {
            ChangeState(Boss1State.Idle);
            waitTimer += Time.deltaTime;

            if (waitTimer >= idleToPatrolWaitTime)
            {
                isWaiting = false;
                waitTimer = 0f;

                // Pick a new random patrol index (different from current)
                int newIndex;
                do
                {
                    newIndex = Random.Range(0, patrolPoints.Length);
                } while (newIndex == currentPatrolIndex);

                currentPatrolIndex = newIndex;
                ChangeState(Boss1State.Patrol); // Resume patrolling
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

            //imprecision in floating-point distance and movement
            if (Vector2.Distance(transform.position, targetPos) < 0.2f)
            {
                rb.velocity = Vector2.zero;
                isWaiting = true;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            unstuckPatrolWaitTimer = unstuckPatrolWaitTime; // Reset the timer
            isWaiting = true;
        }
    }

    private void Flip()
    {
        facingDirection *= -1; // Change the direction
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Flip the sprite by changing the x scale
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

            if (distanceToPlayer <= attackRangeMelee)
            {
                // Player is within melee range — prioritize this
                if (attackCooldownTimer <= 0)
                {
                    ChangeState(Boss1State.Attack);
                    attackCooldownTimer = attackCooldown;
                }
            }
            else if (distanceToPlayer <= attackRangeRanged)
            {
                // Player is not in melee range, but is in ranged range
                if (attackCooldownTimer <= 0)
                {
                    ChangeState(Boss1State.RangedAttack);
                    attackCooldownTimer = attackCooldown;
                }
            }
            else
            {
                // Player is far away, start chasing
                ChangeState(Boss1State.Chase);
            }
        }
        else
        {
            // No player detected, resume patrolling
            rb.velocity = Vector2.zero;
            ChangeState(Boss1State.Patrol);
        }
    }

    public void ChangeState(Boss1State newState)
    {
        // Disable animator parameters for the current state
        switch (enemyState)
        {
            case Boss1State.Idle:
                animator.SetBool("isIdle", false);
                break;
            case Boss1State.Patrol:
                animator.SetBool("isPatrolling", false);
                break;
            case Boss1State.Knockback:
                animator.SetBool("isKnockback", false);
                break;
            case Boss1State.Chase:
                animator.SetBool("isChasing", false);
                break;
            case Boss1State.Attack:
                animator.SetBool("isAttacking", false);
                break;
            case Boss1State.RangedAttack:
                animator.SetBool("isRangedAttacking", false);
                break;
        }

        // Update the state
        enemyState = newState;

        // Enable animator parameters for the new state
        switch (enemyState)
        {
            case Boss1State.Idle:
                animator.SetBool("isIdle", true);
                break;
            case Boss1State.Patrol:
                animator.SetBool("isPatrolling", true);
                break;
            case Boss1State.Knockback:
                animator.SetBool("isKnockback", true);
                break;
            case Boss1State.Chase:
                animator.SetBool("isChasing", true);
                break;
            case Boss1State.Attack:
                animator.SetBool("isAttacking", true);
                break;
            case Boss1State.RangedAttack:
                animator.SetBool("isRangedAttacking", true);
                break;
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
            Gizmos.DrawWireSphere(attackPoint.position, attackRangeMelee);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint.position, attackRangeRanged);
        }
    }
}
