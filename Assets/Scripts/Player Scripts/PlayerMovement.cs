using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int facingDirection = 1; // 1 for right, -1 for left

    public Rigidbody2D rb;
    public Animator animator;

    public Player_Combat combat;

    private bool isKnockedBack;

    private void Awake()
    {
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics; // Ensure the animator updates in sync with physics
    }

    private void Update()
    {
        if (Input.GetButtonDown("Slash"))
        {
            combat.Attack();
        }
    }

    // FixedUpdate is called 50x per second by default in Unity, suitable for physics updates
    void FixedUpdate()
    {
        if (isKnockedBack)
        {
            // If the player is knocked back, we can skip the movement logic
            return;
        }
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal > 0 && transform.localScale.x < 0 || horizontal < 0 && transform.localScale.x > 0)
        {
            Flip();
        }

        animator.SetFloat("horizontal", horizontal);
        animator.SetFloat("vertical", vertical);

        animator.SetBool("isMoving", horizontal != 0 || vertical != 0);

        // Normalize the movement vector to ensure consistent speed in all directions
        Vector2 input = new Vector2(horizontal, vertical);
        rb.velocity = input.sqrMagnitude > 0 ? moveSpeed * input.normalized : Vector2.zero;
        //rb.MovePosition(rb.position + rb.velocity * Time.fixedDeltaTime);

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
