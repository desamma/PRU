using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Patrol,
    Knockback,
    Chase,
    Attack
}

public class Enemy_Movement : MonoBehaviour
{
    public float speed = 1f;
    public float attackRange = 2f;
    public float attackCooldown = 2f; 
    public float playerDetectionRange = 5f;
    public Transform detectionPoint;
    public Transform attackPoint;
    public LayerMask playerLayer;

    private Vector3 originalPosition;
    private float attackCooldownTimer;
    private int facingDirection;
    //private AudioSource audioClip;
    private EnemyState enemyState;
    public float noPlayerTimer = 0f; // Timer for no player detection

    private Rigidbody2D rb;
    private Transform player;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //audioClip = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
        //originalPosition = transform.position; // Store the original position for potential patrol logic
        facingDirection = transform.localScale.x > 0 ? 1 : -1; // Determine initial facing direction based on local scale
    }

    private void Update()
    {
        if (enemyState != EnemyState.Knockback)
        {
            CheckForPlayer();
            if (attackCooldownTimer > 0)
            {
                attackCooldownTimer -= Time.deltaTime;
            }
            if (enemyState == EnemyState.Chase)
            {
                Debug.Log("Chasing the player");
                Chase();
            }
            else if (enemyState == EnemyState.Attack)
            {
                rb.velocity = Vector2.zero; // Stop the enemy when attacking
                //Debug.Log("Attacking the player");
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

    private void Flip()
    {
        facingDirection *= -1; // Change the direction
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Flip the sprite by changing the x scale
        transform.localScale = localScale;
    }

    private void CheckForPlayer()
    {
        //Debug.Log("Checking for player within detection range");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectionRange, playerLayer);
        if (hitColliders.Length > 0)
        {
            player = hitColliders[0].transform;

            if (Vector2.Distance(transform.position, player.position) <= attackRange)
            {
                Debug.Log("Player is within attack range");
                rb.velocity = Vector2.zero; // Stop the enemy when attacking
                if (attackCooldownTimer <= 0)
                {
                    ChangeState(EnemyState.Attack);
                    attackCooldownTimer = attackCooldown;
                }
            }
            else if (Vector2.Distance(transform.position, player.position) > attackRange && enemyState != EnemyState.Attack)
            {
                ChangeState(EnemyState.Chase);
            }
        }
        /*else if (Vector2.Distance(originalPosition, transform.position) > 0.1f && hitColliders.Length <= 0 && noPlayerTimer > 2f)
        {
            // If no player is detected, return to the original position (patrol logic can be added here)
            Vector2 direction = (originalPosition - transform.position).normalized;
            if (originalPosition.x > transform.position.x && facingDirection == -1 ||
            originalPosition.x < transform.position.x && facingDirection == 1)
            {
                Flip();
            }
            else
            {
                rb.velocity = direction * speed;
                ChangeState(EnemyState.Patrol);
            }
        }*/
        else
        {
            //noPlayerTimer += Time.deltaTime; // Increment the timer when no player is detected
            rb.velocity = Vector2.zero; // Stop the enemy when not chasing
            ChangeState(EnemyState.Idle);
        }
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        audioClip.Play();
    //        /*rb.velocity = Vector2.zero; // Stop the enemy when not chasing
    //        ChangeState(EnemyState.Idle);*/
    //    }
    //}   

    public void ChangeState(EnemyState newState)
    {
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
    }

    public void Attack()
    {
        Debug.Log("Enemy attacks the player!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionPoint.position, playerDetectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
