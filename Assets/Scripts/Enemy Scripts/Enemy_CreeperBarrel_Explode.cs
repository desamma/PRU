using System.Collections;
using UnityEngine;

public class Enemy_CreeperBarrel_Explode : MonoBehaviour
{
    public Rigidbody2D rb;
    public int damage = 1;
    public float knockbackForce = 10f;
    public float stunTime = 0.5f;
    public float explosionRadius = 2f; // Radius of the explosion effect
    private Animator animator;
    public float animationDelay = 0.3f;
    private bool isExploded = false; // Flag to prevent multiple explosions

    private Enemy_CreeperBarrel_Movement movement;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        movement = GetComponent<Enemy_CreeperBarrel_Movement>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isExploded) return; // Prevent further explosions if already exploded

        CollisionExplode();
    }

    public void Explode()
    {
        if (isExploded) return; // Prevent multiple explosions

        isExploded = true; // Set the flag to true to prevent further explosions

        // Detect objects within the explosion radius
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hitObject in hitObjects)
        {
            // Apply damage and knockback to the player
            if (hitObject.CompareTag("Player"))
            {
                hitObject.GetComponent<PlayerHealth>().ChangeHealth(-damage);
                hitObject.GetComponent<PlayerMovement>().KnockBack(transform, knockbackForce, stunTime);
            }

            // Apply damage and knockback to the enemy
            if (hitObject.CompareTag("Enemy"))
            {
                hitObject.GetComponent<Enemy_Health>().ChangeHealth(-damage);
                hitObject.GetComponent<Enemy_Knockback>().KnockBack(transform, knockbackForce, stunTime, stunTime);
            }
        }
        Destroy(gameObject);
    }

    public void CollisionExplode()
    {
        if (isExploded) return;

        isExploded = true;

        // Trigger explosion animation
        if (animator != null)
        {
            animator.SetBool("isExplode", true);
        }

        // Detect objects within the explosion radius
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hitObject in hitObjects)
        {
            if (hitObject.CompareTag("Player"))
            {
                hitObject.GetComponent<PlayerHealth>().ChangeHealth(-damage);
                hitObject.GetComponent<PlayerMovement>().KnockBack(transform, knockbackForce, stunTime);
            }

            if (hitObject.CompareTag("Enemy"))
            {
                hitObject.GetComponent<Enemy_Health>().ChangeHealth(-damage);
                hitObject.GetComponent<Enemy_Knockback>().KnockBack(transform, knockbackForce, stunTime, stunTime);
            }

            if (hitObject.CompareTag("Terrain"))
            {
                rb.velocity = Vector2.zero;
                /*
                animator.SetBool("isChasing", false);
                animator.SetBool("isExplode", true);*/
                movement.ChangeState(CreeperEnemyState.Explode);
                movement.chasingDisable = true; // Disable chasing to prevent further movement
                StartCoroutine(TerrainExplodeDelay());
            }
        }

        // Delay destroy to let animation play
        StartCoroutine(DestroyAfterDelay(animationDelay));
    }

    private IEnumerator TerrainExplodeDelay()
    {
        //Fix bug not stop entirely
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(animationDelay);
        Explode();
    }

    // Delay destroy to let animation play
    private IEnumerator DestroyAfterDelay(float delay)
    {
        StartCoroutine(EnemyStopDelay(0.2f));
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    //Stop enemy movement first then destroy for sometimes (animation issue)
    private IEnumerator EnemyStopDelay(float delay)
    {
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(delay);
        rb.velocity = Vector2.zero; // Ensure the enemy stops moving after the delay
    }

}