using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 1;
    public float knockbackForce;
    public float stunTime;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>().ChangeHealth(-damage);
            collision.GetComponent<PlayerMovement>().KnockBack(transform, knockbackForce, stunTime);
            Destroy(gameObject); // destroy the projectile after hitting the player
        }
        else if (collision.CompareTag("Terrain"))
        {
            Destroy(gameObject); // destroy the projectile after hitting terrain
        }
    }
}
