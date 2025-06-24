using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public enum CreeperEnemyState
{
    Idle,
    WakeUp,
    Sleep,
    Chase,
    Explode,
    Knockback
}

public class Enemy_CreeperBarrel_Movement : MonoBehaviour
{
    public float speed = 1f;
    public float playerDetectionRange = 5f;
    public Transform detectionPoint;
    public LayerMask playerLayer;
    private int facingDirection;

    public Transform explodePoint;
    public float explodeRange = 2f;
    private CreeperEnemyState enemyState;

    private Rigidbody2D rb;
    private Transform player;
    private Animator animator;
    public Enemy_CreeperBarrel_Explode barrel;

    private float playerLostTime = 1f; // Time after which enemy sleeps
    private float playerLostTimer = 0f;
    [SerializeField] private float animationDelay = 0.5f; // Delay for animations
    [SerializeField] private float explosionDelay = 0.3f; // Delay for explosion
    private bool playerDetected = false;
    private bool isWakingUp = false;
    private bool hasExploded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ChangeState(CreeperEnemyState.Idle);
        facingDirection = transform.localScale.x > 0 ? 1 : -1;
    }

    void Update()
    {
        if (enemyState != CreeperEnemyState.Knockback)
        {
            CheckForPlayer();

            if (!playerDetected && enemyState != CreeperEnemyState.Idle && enemyState != CreeperEnemyState.Sleep)
            {
                playerLostTimer += Time.deltaTime;
                if (playerLostTimer >= playerLostTime)
                {
                    StartCoroutine(SleepToIdle());
                }
            }

            if (enemyState == CreeperEnemyState.Chase)
            {
                Chase();
            }
            else if (enemyState == CreeperEnemyState.Explode)
            {
                if (!hasExploded)
                {
                    StartCoroutine(Explode());
                }
            }
        }
    }

    void Chase()
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
            playerDetected = true;
            playerLostTimer = 0f;
            player = hitColliders[0].transform;

            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= explodeRange)
            {
                rb.velocity = Vector2.zero;
                ChangeState(CreeperEnemyState.Explode);

                if (!hasExploded)
                {
                    StartCoroutine(Explode());
                }

            }
            else if (enemyState != CreeperEnemyState.Chase && !isWakingUp)
            {
                StartCoroutine(WakeUpThenChase());
            }
        }
        else
        {
            playerDetected = false;
            rb.velocity = Vector2.zero;
        }
    }

    IEnumerator Explode()
    {
        hasExploded = true;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(explosionDelay); // Let the explode animation play

        barrel.Explode();
        /*
                if (enemyState == CreeperEnemyState.Explode) // if it
                {
                    ChangeState(CreeperEnemyState.Idle);
                    rb.velocity = Vector2.zero;
        }*/
    }


    IEnumerator WakeUpThenChase()
    {
        isWakingUp = true;
        rb.velocity = Vector2.zero;
        ChangeState(CreeperEnemyState.WakeUp);
        yield return new WaitForSeconds(animationDelay); // Let the WakeUp animation play
        if (enemyState == CreeperEnemyState.WakeUp) // still waking
        {
            ChangeState(CreeperEnemyState.Chase);
        }
        isWakingUp = false;
    }

    IEnumerator SleepToIdle()
    {
        rb.velocity = Vector2.zero;

        if (enemyState != CreeperEnemyState.Sleep)
        {
            ChangeState(CreeperEnemyState.Sleep);
        }

        yield return new WaitForSeconds(animationDelay); // Play sleep animation

        // Fix animation stuck in Sleep
        if (enemyState == CreeperEnemyState.Sleep)
        {
            ChangeState(CreeperEnemyState.Idle);
        }
    }


    public void ChangeState(CreeperEnemyState newState)
    {
        if (enemyState == newState) return;

        // Disable current state animations
        switch (enemyState)
        {
            case CreeperEnemyState.Idle: animator.SetBool("isIdle", false); break;
            case CreeperEnemyState.Chase: animator.SetBool("isChasing", false); break;
            case CreeperEnemyState.Explode: animator.SetBool("isExplode", false); break;
            case CreeperEnemyState.WakeUp: animator.SetBool("isWaking", false); break;
            case CreeperEnemyState.Sleep: animator.SetBool("isSleeping", false); break;
        }

        enemyState = newState;

        // Enable new state animations
        switch (enemyState)
        {
            case CreeperEnemyState.Idle: animator.SetBool("isIdle", true); break;
            case CreeperEnemyState.Chase: animator.SetBool("isChasing", true); break;
            case CreeperEnemyState.Explode: animator.SetBool("isExplode", true); break;
            case CreeperEnemyState.WakeUp: animator.SetBool("isWaking", true); break;
            case CreeperEnemyState.Sleep: animator.SetBool("isSleeping", true); break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionPoint.position, playerDetectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(explodePoint.position, explodeRange);
    }
}
