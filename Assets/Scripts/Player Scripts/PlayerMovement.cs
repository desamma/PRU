using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public int facingDirection = 1; // 1 for right, -1 for left

    public Rigidbody2D rb;
    public Animator animator;

    public Player_Combat combat;
    public PlayerStamina staminaManager;

    private Vector2 dashDirection;
    private bool isKnockedBack;
    private bool isDashing = false;
    private bool isDashingAvailable = true;
    private float dashDuration;
    private float dashSpeed;

    private float vertical;
    private float horizontal;

    private void Awake()
    {
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics; // Ensure the animator updates in sync with physics
    }

    private void Start()
    {
        staminaManager = GetComponent<PlayerStamina>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Slash"))
        {
            combat.Attack();
        }
        if (Input.GetButtonDown("Dash") && isDashingAvailable && StatManager.instance.currentStamina >= StatManager.instance.staminaCost)
        {
            Dash(new Vector2(horizontal, vertical));
        }
    }

    // FixedUpdate is called 50x per second by default in Unity, suitable for physics updates
    void FixedUpdate()
    {
        if (isKnockedBack || isDashing)
        {
            // If the player is knocked back, we can skip the movement logic
            return;
        }
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (horizontal > 0 && transform.localScale.x < 0 || horizontal < 0 && transform.localScale.x > 0 && !animator.GetBool("isAttacking"))
        {
            Flip();
        }

        animator.SetFloat("horizontal", horizontal);
        animator.SetFloat("vertical", vertical);

        animator.SetBool("isMoving", horizontal != 0 || vertical != 0);

        // Normalize the movement vector to ensure consistent speed in all directions
        Vector2 input = new Vector2(horizontal, vertical);
        rb.velocity = input.sqrMagnitude > 0 ? StatManager.instance.moveSpeed * input.normalized : Vector2.zero;
        //rb.MovePosition(rb.position + rb.velocity * Time.fixedDeltaTime);

    }

    void Dash(Vector2 direction)
    {
        staminaManager.UseStamina();

        // Normalize direction (fallback to facing direction if no input)
        dashDirection = direction.sqrMagnitude > 0 ? direction.normalized : new Vector2(transform.localScale.x, 0);

        dashSpeed = StatManager.instance.moveSpeed * 2.5f;
        dashDuration = StatManager.instance.dashDuration;
        isDashing = true;

        StartCoroutine(DashCooldown());
    }

    IEnumerator DashCooldown()
    {
        isDashingAvailable = false;
        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            rb.velocity = dashDirection * dashSpeed;
            yield return new WaitForFixedUpdate();
        }

        rb.velocity = Vector2.zero;
        isDashing = false;
        yield return new WaitForSeconds(StatManager.instance.dashDelay);
        isDashingAvailable = true;
    }

    void Flip()
    {
        facingDirection *= -1;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    public void KnockBack(Transform enemy, float force, float stunTime)
    {
        isKnockedBack = true;
        Vector2 knockbackDirection = (transform.position - enemy.position).normalized; // Calculate direction away from the enemy
        rb.velocity = knockbackDirection * force;
        StartCoroutine(KnockbackCounter(stunTime));
    }

    IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime); // Duration of the knockback effect
        rb.velocity = Vector2.zero; // Stop the knockback movement
        isKnockedBack = false;
    }
}
