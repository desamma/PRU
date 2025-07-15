using System.Collections;
using UnityEngine;

public enum Boss1State
{
    Idle,
    Patrol,
    Knockback,
    Chase,
    Attack,
    RangedAttack,
    Dying
}

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
        ChangeState(Boss1State.Idle);

        unstuckPatrolWaitTime *= patrolDistance / 2;
        unstuckPatrolWaitTimer = unstuckPatrolWaitTime;

        facingDirection = transform.localScale.x > 0 ? 1 : -1;
    }

    private void Update()
    {
        if (enemyState == Boss1State.Dying) return;

        // If health is zero, start dying
        if (health != null && health.IsDead)
        {
            rb.velocity = Vector2.zero;
            ChangeState(Boss1State.Dying);
            return;
        }

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
                case Boss1State.RangedAttack:
                    rb.velocity = Vector2.zero;
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
            ChangeState(Boss1State.Idle);
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
                ChangeState(Boss1State.Patrol);
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

            if (distanceToPlayer <= attackRangeMelee)
            {
                if (attackCooldownTimer <= 0)
                {
                    ChangeState(Boss1State.Attack);
                    attackCooldownTimer = attackCooldown;
                }
            }
            else if (distanceToPlayer <= attackRangeRanged)
            {
                if (attackCooldownTimer <= 0)
                {
                    ChangeState(Boss1State.RangedAttack);
                    attackCooldownTimer = attackCooldown;
                }
            }
            else
            {
                ChangeState(Boss1State.Chase);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            ChangeState(Boss1State.Patrol);
        }
    }

    public void ChangeState(Boss1State newState)
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isPatrolling", false);
        animator.SetBool("isChasing", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isRangedAttacking", false);
        animator.SetBool("isDying", false);

        enemyState = newState;

        switch (enemyState)
        {
            case Boss1State.Idle:
                animator.SetBool("isIdle", true);
                break;
            case Boss1State.Patrol:
                animator.SetBool("isPatrolling", true);
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
            case Boss1State.Dying:
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
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(attackPoint.position, attackRangeMelee);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint.position, attackRangeRanged);
        }
    }
}
