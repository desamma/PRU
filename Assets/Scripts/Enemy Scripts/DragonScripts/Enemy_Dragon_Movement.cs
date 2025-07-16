using UnityEngine;

public enum EnemyDragonState
{
    Idle,
    Chase,
    Fly,
    Landing,
    LandAttack,
    FlyAttack,
    Die,
    Repositioning
}

public class Enemy_Dragon_Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 1f;
    public float flySpeed = 3f; // 2x faster flying
    public float repositionSpeed = 2f;

    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    public float attackRange = 2f;
    public float landAttackDuration = 3f;
    public float flyAttackDuration = 2f; // Separate duration for fly attacks
    public float damageInterval = 0.5f;
    public int landFireDamage = 10;
    public int flyFireDamage = 8; // Different damage for air attacks

    [Header("Detection Settings")]
    public float playerDetectionRange = 5f;
    public float repositionDistance = 3f;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    [Header("State Transition Delays")]
    public float attackStateDelay = 0.5f;
    public float chaseStateDelay = 0.3f;
    public float repositionDelay = 0.4f;
    public float flyAwayDelay = 0.6f;

    [Header("Fly Away Settings")]
    public float flyAwayHeight = 3f;
    public float flyAwayDuration = 2f;
    public float landingDelay = 0.5f;
    public float landingSpeed = 2f; // Speed of landing descent
    public float landingAnimationTime = 1f; // Time to wait for landing animation

    [Header("Flying Strategy")]
    public int landAttacksBeforeFlying = 2;
    public float airCombatDuration = 5f;
    public float flyAttackCooldown = 1.5f;

    // MODIFIED: Separate colliders for different attack types
    [Header("Attack Colliders")]
    public EdgeCollider2D LandFireCollider;  // For ground fire attacks
    public EdgeCollider2D AirFireCollider;   // For air fire attacks (like fireballs)

    [Header("Colliders")]
    public BoxCollider2D idleCollider;
    public BoxCollider2D flyingCollider;

    private int landAttacksCount = 0;
    private float airCombatTimer = 0f;
    private bool inAirCombatMode = false;
    private float flyAttackTimer = 0f;

    private Vector3 originalPosition;
    private Vector3 repositionTarget;
    private Vector3 flyAwayTarget;
    private float attackCooldownTimer;
    private float landAttackTimer;
    private float flyAttackTimer_Duration; // NEW: Separate timer for fly attack duration
    private float damageTimer;
    private float flyAwayTimer;
    private float landingAnimationTimer; // NEW: Timer for landing animation
    private int facingDirection;
    private bool isRepositioning = false;

    // MODIFIED: Separate fire states for each attack type
    private bool landFireActive = false;
    private bool airFireActive = false;
    private bool isFlyingAway = false;
    private bool isLandingAnimationPlaying = false; // NEW: Track if landing animation is playing

    private float stateTransitionTimer;
    private EnemyDragonState pendingState;
    private bool isWaitingForStateChange = false;

    private EnemyDragonState enemyState;
    private Rigidbody2D rb;
    private Transform player;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalPosition = transform.position;
        ChangeState(EnemyDragonState.Idle);
        facingDirection = transform.localScale.x > 0 ? 1 : -1;

        // MODIFIED: Ensure both fire colliders start disabled
        if (LandFireCollider != null)
            LandFireCollider.enabled = false;
        if (AirFireCollider != null)
            AirFireCollider.enabled = false;
    }

    private void Update()
    {
        if (enemyState == EnemyDragonState.Die)
        {
            rb.velocity = Vector2.zero;
            DeactivateAllFireColliders(); // MODIFIED: Deactivate all fire colliders
            return;
        }

        if (isWaitingForStateChange)
        {
            stateTransitionTimer -= Time.deltaTime;
            if (stateTransitionTimer <= 0)
            {
                isWaitingForStateChange = false;
                ChangeState(pendingState);
            }
            else
            {
                ExecuteCurrentState();
                UpdateTimers();
                return;
            }
        }

        CheckForPlayer();
        ExecuteCurrentState();
        UpdateTimers();
    }

    private void ExecuteCurrentState()
    {
        switch (enemyState)
        {
            case EnemyDragonState.Chase:
                Chase();
                break;
            case EnemyDragonState.Fly:
                Fly();
                break;
            case EnemyDragonState.Landing:
                Landing();
                break;
            case EnemyDragonState.LandAttack:
                LandAttack();
                break;
            case EnemyDragonState.FlyAttack:
                FlyAttack();
                break;
            case EnemyDragonState.Repositioning:
                Reposition();
                break;
            case EnemyDragonState.Idle:
                rb.velocity = Vector2.zero;
                break;
        }
    }

    // MODIFIED: Updated timer system for both attack types
    private void UpdateTimers()
    {
        if (attackCooldownTimer > 0)
            attackCooldownTimer -= Time.deltaTime;

        if (landAttackTimer > 0)
            landAttackTimer -= Time.deltaTime;

        if (flyAttackTimer_Duration > 0)
            flyAttackTimer_Duration -= Time.deltaTime;

        if (damageTimer > 0)
            damageTimer -= Time.deltaTime;

        if (flyAwayTimer > 0)
            flyAwayTimer -= Time.deltaTime;

        if (flyAttackTimer > 0)
            flyAttackTimer -= Time.deltaTime;

        if (landingAnimationTimer > 0)
            landingAnimationTimer -= Time.deltaTime;

        if (inAirCombatMode)
        {
            airCombatTimer -= Time.deltaTime;
            if (airCombatTimer <= 0)
            {
                inAirCombatMode = false;
                isFlyingAway = false;
                ScheduleStateChange(EnemyDragonState.Landing, landingDelay);
            }
        }
    }

    private void CheckForPlayer()
    {
        if (isWaitingForStateChange)
            return;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectionRange, playerLayer);
        if (hitColliders.Length > 0)
        {
            player = hitColliders[0].transform;
            float distance = Vector2.Distance(transform.position, player.position);

            if (inAirCombatMode && enemyState == EnemyDragonState.Fly && !isFlyingAway)
            {
                if (distance <= attackRange && flyAttackTimer <= 0)
                {
                    ScheduleStateChange(EnemyDragonState.FlyAttack, attackStateDelay);
                    flyAttackTimer = flyAttackCooldown;
                }
                return;
            }

            if (distance <= attackRange && !isRepositioning && !inAirCombatMode)
            {
                if (enemyState == EnemyDragonState.LandAttack || enemyState == EnemyDragonState.FlyAttack)
                    return;

                rb.velocity = Vector2.zero;

                if (attackCooldownTimer <= 0)
                {
                    bool playerInFront = IsPlayerInFront();

                    if (!playerInFront && enemyState != EnemyDragonState.LandAttack)
                    {
                        StartRepositioningWithDelay();
                    }
                    else
                    {
                        ScheduleStateChange(EnemyDragonState.LandAttack, attackStateDelay);
                        attackCooldownTimer = attackCooldown;
                    }
                }
            }
            else if (!isRepositioning && enemyState != EnemyDragonState.Chase && !inAirCombatMode)
            {
                if (enemyState != EnemyDragonState.LandAttack && enemyState != EnemyDragonState.FlyAttack)
                {
                    ScheduleStateChange(EnemyDragonState.Chase, chaseStateDelay);
                }
            }
        }
        else if (!isRepositioning && enemyState != EnemyDragonState.Idle && !inAirCombatMode)
        {
            if (enemyState != EnemyDragonState.LandAttack && enemyState != EnemyDragonState.FlyAttack)
            {
                ScheduleStateChange(EnemyDragonState.Idle, chaseStateDelay);
            }
        }
    }

    // MODIFIED: Land attack now uses only land fire collider
    private void LandAttack()
    {
        rb.velocity = Vector2.zero;

        if (!landFireActive)
        {
            ActivateLandFireCollider();
            landAttackTimer = landAttackDuration;
            damageTimer = 0f;
            landFireActive = true;
        }

        if (landAttackTimer <= 0)
        {
            DeactivateLandFireCollider();
            landFireActive = false;
            landAttacksCount++;

            if (landAttacksCount >= landAttacksBeforeFlying)
            {
                landAttacksCount = 0;
                StartAirCombat();
            }
            else
            {
                ChangeState(EnemyDragonState.Idle);
            }
        }
    }

    // MODIFIED: Fly attack now uses air fire collider (only when flying)
    private void FlyAttack()
    {
        rb.velocity = Vector2.zero;

        if (!airFireActive)
        {
            // Only activate air fire collider if we're actually flying
            if (enemyState == EnemyDragonState.FlyAttack)
            {
                ActivateAirFireCollider();
                flyAttackTimer_Duration = flyAttackDuration;
                damageTimer = 0f;
                airFireActive = true;
            }
        }

        if (flyAttackTimer_Duration <= 0)
        {
            DeactivateAirFireCollider();
            airFireActive = false;
            ChangeState(EnemyDragonState.Fly);
        }
    }

    private void ActivateLandFireCollider()
    {
        if (LandFireCollider != null)
        {
            LandFireCollider.enabled = true;
        }
    }

    private void DeactivateLandFireCollider()
    {
        if (LandFireCollider != null)
        {
            LandFireCollider.enabled = false;
        }
    }

    private void ActivateAirFireCollider()
    {
        // Only activate air fire collider if dragon is in a flying state
        if (AirFireCollider != null && (enemyState == EnemyDragonState.FlyAttack || enemyState == EnemyDragonState.Fly))
        {
            AirFireCollider.enabled = true;
        }
    }

    private void DeactivateAirFireCollider()
    {
        if (AirFireCollider != null)
        {
            AirFireCollider.enabled = false;
        }
    }

    private void DeactivateAllFireColliders()
    {
        DeactivateLandFireCollider();
        DeactivateAirFireCollider();
    }

    // MODIFIED: Handle damage for both attack types (air collider only works when flying)
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && damageTimer <= 0)
        {
            if (other.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                // Land fire damage (works when on ground)
                if (landFireActive && enemyState == EnemyDragonState.LandAttack)
                {
                    playerHealth.ChangeHealth(-landFireDamage);
                    damageTimer = damageInterval;
                }
                // Air fire damage (only works when flying)
                else if (airFireActive && enemyState == EnemyDragonState.FlyAttack)
                {
                    playerHealth.ChangeHealth(-flyFireDamage);
                    damageTimer = damageInterval;
                }
            }
        }
    }

    // Rest of the methods remain the same...
    private void ScheduleStateChange(EnemyDragonState newState, float delay)
    {
        if (isWaitingForStateChange && pendingState == newState)
            return;

        stateTransitionTimer = delay;
        pendingState = newState;
        isWaitingForStateChange = true;
    }

    private void StartRepositioningWithDelay()
    {
        ScheduleStateChange(EnemyDragonState.Repositioning, repositionDelay);

        Vector2 playerDirection = (player.position - transform.position).normalized;
        Vector2 sideOffset = new Vector2(-playerDirection.y, playerDirection.x) * repositionDistance;
        repositionTarget = player.position + (Vector3)sideOffset;

        if (player.position.x > transform.position.x && facingDirection == -1 ||
            player.position.x < transform.position.x && facingDirection == 1)
        {
            Flip();
        }
    }

    private bool IsPlayerInFront()
    {
        if (player == null) return false;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float dotProduct = Vector2.Dot(Vector2.right * facingDirection, directionToPlayer);

        return dotProduct > 0.3f;
    }

    private void Reposition()
    {
        Vector2 direction = (repositionTarget - transform.position).normalized;
        rb.velocity = direction * repositionSpeed;

        if (Vector2.Distance(transform.position, repositionTarget) < 0.5f)
        {
            isRepositioning = false;
            rb.velocity = Vector2.zero;
            ChangeState(EnemyDragonState.Idle);
        }
    }

    private void Chase()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x && facingDirection == -1 ||
            player.position.x < transform.position.x && facingDirection == 1)
        {
            Flip();
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * speed;
    }

    private void Fly()
    {
        if (enemyState != EnemyDragonState.Fly)
            return;

        if (isFlyingAway)
        {
            Vector2 flyAwayDirection = (flyAwayTarget - transform.position).normalized;

            if ((flyAwayTarget.x > transform.position.x && facingDirection == -1) ||
                (flyAwayTarget.x < transform.position.x && facingDirection == 1))
            {
                Flip();
            }

            float distanceToTarget = Vector2.Distance(transform.position, flyAwayTarget);

            if (distanceToTarget >= 0.5f)
            {
                rb.velocity =  flySpeed * speed * flyAwayDirection;
            }
            else
            {
                rb.velocity = Vector2.zero;
                isFlyingAway = false;
            }
            return;
        }

        if (inAirCombatMode && player != null)
        {
            Vector3 targetPosition = player.position + Vector3.up * flyAwayHeight;
            Vector2 direction = (targetPosition - transform.position).normalized;

            if ((player.position.x > transform.position.x && facingDirection == -1) ||
                (player.position.x < transform.position.x && facingDirection == 1))
            {
                Flip();
            }

            float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
            if (distanceToTarget > 1f)
            {
                rb.velocity = direction * flySpeed * speed * 0.5f;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
            return;
        }

        if (!player) return;

        if ((player.position.x > transform.position.x && facingDirection == -1) ||
            (player.position.x < transform.position.x && facingDirection == 1))
        {
            Flip();
        }

        Vector2 chaseDirection = (player.position - transform.position).normalized;
        rb.velocity = flySpeed * speed * chaseDirection;
    }

    private void Landing()
    {
        // First phase: Descend to ground level
        if (!isLandingAnimationPlaying)
        {
            rb.velocity = Vector2.down * landingSpeed;

            // Check if we've reached the ground
            if (transform.position.y <= originalPosition.y + 0.1f)
            {
                // Snap to ground position
                rb.velocity = Vector2.zero;
                transform.position = new Vector3(transform.position.x, originalPosition.y, transform.position.z);

                // Start landing animation phase
                isLandingAnimationPlaying = true;
                landingAnimationTimer = landingAnimationTime;
            }
        }
        else
        {
            // Second phase: Wait for landing animation to complete
            rb.velocity = Vector2.zero; // Stay still during animation

            if (landingAnimationTimer <= 0)
            {
                // Landing animation complete
                isLandingAnimationPlaying = false;
                isFlyingAway = false;
                ChangeState(EnemyDragonState.Idle);
            }
        }
    }

    private void StartAirCombat()
    {
        inAirCombatMode = true;
        airCombatTimer = airCombatDuration;
        flyAttackTimer = 0f;

        if (player != null)
        {
            Vector3 flyPosition = player.position + Vector3.up * flyAwayHeight;
            Vector3 sideOffset = facingDirection > 0 ? Vector3.left * 2f : Vector3.right * 2f;
            flyAwayTarget = flyPosition + sideOffset;
        }
        else
        {
            flyAwayTarget = transform.position + Vector3.up * flyAwayHeight;
        }

        isFlyingAway = true;
        ScheduleStateChange(EnemyDragonState.Fly, flyAwayDelay);
    }

    public void ChangeState(EnemyDragonState newState)
    {
        isWaitingForStateChange = false;

        if (newState == EnemyDragonState.Repositioning)
        {
            isRepositioning = true;
        }
        else if (enemyState == EnemyDragonState.Repositioning)
        {
            isRepositioning = false;
        }

        if (newState != EnemyDragonState.Fly && newState != EnemyDragonState.Landing)
        {
            isFlyingAway = false;
        }

        // Reset landing animation state when changing from Landing
        if (enemyState == EnemyDragonState.Landing && newState != EnemyDragonState.Landing)
        {
            isLandingAnimationPlaying = false;
        }

        // MODIFIED: Ensure air fire collider only works during flying states
        if (newState != EnemyDragonState.LandAttack)
        {
            DeactivateLandFireCollider();
            landFireActive = false;
        }

        if (newState != EnemyDragonState.FlyAttack)
        {
            DeactivateAirFireCollider();
            airFireActive = false;
        }

        // Extra safety: Deactivate air fire collider when not flying
        if (newState != EnemyDragonState.Fly && newState != EnemyDragonState.FlyAttack && newState != EnemyDragonState.Repositioning)
        {
            DeactivateAirFireCollider();
            airFireActive = false;
        }

        animator.SetBool("isIdle", false);
        animator.SetBool("isChasing", false);
        animator.SetBool("isFlying", false);
        animator.SetBool("isLanding", false);
        animator.SetBool("isLandAttacking", false);
        animator.SetBool("isFlyAttacking", false);
        animator.SetBool("isDying", false);

        enemyState = newState;

        switch (enemyState)
        {
            case EnemyDragonState.Idle:
                animator.SetBool("isIdle", true);
                break;
            case EnemyDragonState.Chase:
                animator.SetBool("isChasing", true);
                break;
            case EnemyDragonState.Fly:
                animator.SetBool("isFlying", true);
                break;
            case EnemyDragonState.Landing:
                animator.SetBool("isLanding", true);
                break;
            case EnemyDragonState.LandAttack:
                animator.SetBool("isLandAttacking", true);
                break;
            case EnemyDragonState.FlyAttack:
                animator.SetBool("isFlyAttacking", true);
                break;
            case EnemyDragonState.Repositioning:
                animator.SetBool("isFlying", true);
                break;
            case EnemyDragonState.Die:
                animator.SetBool("isDying", true);
                break;
        }

        UpdateColliders();
    }

    private void UpdateColliders()
    {
        bool shouldUseFlyingCollider =
            enemyState == EnemyDragonState.Fly ||
            enemyState == EnemyDragonState.FlyAttack ||
            enemyState == EnemyDragonState.Repositioning;

        if (idleCollider != null)
            idleCollider.enabled = !shouldUseFlyingCollider;

        if (flyingCollider != null)
            flyingCollider.enabled = shouldUseFlyingCollider;
    }

    private void Flip()
    {
        facingDirection *= -1;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (detectionPoint != null)
            Gizmos.DrawWireSphere(detectionPoint.position, playerDetectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (isRepositioning)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(repositionTarget, 0.5f);
            Gizmos.DrawLine(transform.position, repositionTarget);
        }

        if (isFlyingAway)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(flyAwayTarget, 0.5f);
            Gizmos.DrawLine(transform.position, flyAwayTarget);
        }

        if (isWaitingForStateChange)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 2, Vector3.one * 0.5f);
        }
    }
}