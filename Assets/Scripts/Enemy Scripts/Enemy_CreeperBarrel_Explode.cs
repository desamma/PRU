using UnityEngine;

public class Enemy_CreeperBarrel_Explode : MonoBehaviour
{
    public Rigidbody2D rb;
    public int damage = 1;
    public float knockbackForce = 10f;
    public float stunTime = 0.5f;
    public float explosionRadius = 2f; // Radius of the explosion effect

    private bool isExploded = false; // Flag to prevent multiple explosions

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isExploded) return; // Prevent further explosions if already exploded

        Explode(); // Trigger the explosion
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
}