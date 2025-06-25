using System.Collections;
using UnityEngine;

public enum CreeperEnemyState
{
    Idle,
    WakeUp,
    Chase,
    Explode,
    Knockback
}

public class Enemy_CreeperBarrel_Movement : MonoBehaviour
{
    public float speed = 5f;
    public float playerDetectionRange = 5f;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    public Transform explodePoint;
    public float explodeRange = 2f;
    private CreeperEnemyState enemyState;

    private Rigidbody2D rb;
    public Transform player;
    private Animator animator;
    public Enemy_CreeperBarrel_Explode barrel;

    [SerializeField] private float animationDelay = 0.5f;
    [SerializeField] private float explosionDelay = 2f;

    //private bool playerDetected = false;
    private bool isWakingUp = false;
    private bool hasExploded = false;
    private Vector2 targetPosition; // Where the player was detected

    private int facingDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        animator = GetComponent<Animator>();
        ChangeState(CreeperEnemyState.Idle);
        facingDirection = transform.localScale.x > 0 ? 1 : -1;
    }

    void Update()
    {
        if (enemyState == CreeperEnemyState.Knockback || hasExploded)
            return;

        CheckForPlayer();

        if (enemyState == CreeperEnemyState.Chase)
        {
            MoveTowardsTarget();
        }
    }

    private void CheckForPlayer()
    {
        if (enemyState == CreeperEnemyState.Explode || enemyState == CreeperEnemyState.Chase)
            return;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectionRange, playerLayer);
        if (hitColliders.Length > 0)
        {
            Transform detectedPlayer = hitColliders[0].transform;

            if (!isWakingUp)
            {
                player = detectedPlayer;
                targetPosition = player.position; // lock position once

                StartCoroutine(WakeUpThenChase());
            }
        }
    }


    IEnumerator WakeUpThenChase()
    {
        isWakingUp = true;
        rb.velocity = Vector2.zero;
        ChangeState(CreeperEnemyState.WakeUp);

        yield return new WaitForSeconds(animationDelay); // Let wakeup animation finish

        if (enemyState == CreeperEnemyState.WakeUp)
        {
            ChangeState(CreeperEnemyState.Chase);
            StartCoroutine(CountdownToExplode());
        }

        isWakingUp = false;
    }

    void MoveTowardsTarget()
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        if (direction.x > 0 && facingDirection == -1 || direction.x < 0 && facingDirection == 1)
        {
            Flip();
        }

        rb.velocity = direction * speed;

        float hitDistance = Vector2.Distance(transform.position, targetPosition);

        if (hitDistance <= explodeRange)
        {
            rb.velocity = Vector2.zero;
            ChangeState(CreeperEnemyState.Explode);
            if (!hasExploded)
                StartCoroutine(Explode());
        }
    }


    IEnumerator CountdownToExplode()
    {
        yield return new WaitForSeconds(explosionDelay);

        if (enemyState == CreeperEnemyState.Chase && !hasExploded)
        {
            rb.velocity = Vector2.zero;
            ChangeState(CreeperEnemyState.Explode);
            StartCoroutine(Explode());
        }
    }

    IEnumerator Explode()
    {
        hasExploded = true;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(animationDelay);

        barrel.Explode();
    }

    private void Flip()
    {
        facingDirection *= -1;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    public void ChangeState(CreeperEnemyState newState)
    {
        if (enemyState == newState) return;

        switch (enemyState)
        {
            case CreeperEnemyState.Idle: animator.SetBool("isIdle", false); break;
            case CreeperEnemyState.Chase: animator.SetBool("isChasing", false); break;
            case CreeperEnemyState.Explode: animator.SetBool("isExplode", false); break;
            case CreeperEnemyState.WakeUp: animator.SetBool("isWaking", false); break;
        }

        enemyState = newState;

        switch (enemyState)
        {
            case CreeperEnemyState.Idle: animator.SetBool("isIdle", true); break;
            case CreeperEnemyState.Chase: animator.SetBool("isChasing", true); break;
            case CreeperEnemyState.Explode: animator.SetBool("isExplode", true); break;
            case CreeperEnemyState.WakeUp: animator.SetBool("isWaking", true); break;
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
