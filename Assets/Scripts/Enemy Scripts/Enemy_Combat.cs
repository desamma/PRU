using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    public int damage = 1;
    public Transform attackPoint;
    public float attackRange;
    public float knockbackForce;
    public float stunTime;
    public LayerMask playerLayer;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerHealth>().ChangeHealth(-damage);
        }
    }

    public void Attack()
    {
        // Detect objects in the attack range
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);

        foreach (Collider2D hit in hits)
        {
            // Check if the object is a player
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerHealth>().ChangeHealth(-damage);
                hit.GetComponent<PlayerMovement>().KnockBack(transform, knockbackForce, stunTime);
            }
            // Check if the object is a barrel
            else if (hit.CompareTag("Barrel"))
            {
                if (hit.TryGetComponent<Barrel>(out var barrel))
                {
                    barrel.Explode(); // Trigger the barrel's explosion
                }
            }
        }
    }
}
