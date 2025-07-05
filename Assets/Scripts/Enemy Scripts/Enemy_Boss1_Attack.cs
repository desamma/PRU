using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss1_Combat : MonoBehaviour
{
    public int damage = 1;
    public Transform attackPoint;
    public float attackRangeMelee;

    public float knockbackForce;
    public float stunTime;
    public LayerMask playerLayer;
    public float attackRangeRanged;
    public float projectileSpeed = 5f;
    public float projectileLifeTime = 5f;
    public int maxProjectiles = 10;

    public GameObject projectilePrefab;
    private readonly List<GameObject> activeProjectiles = new();

    private void Update()
    {
        // Clean up destroyed projectiles from the list
        activeProjectiles.RemoveAll(projectile => projectile == null);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().ChangeHealth(-damage);
        }
    }

    public void Attack()
    {
        // Detect objects in the attack range
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRangeMelee);

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
    public void Shoot()
    {
        // Check if we've reached the projectile limit
        if (activeProjectiles.Count >= maxProjectiles)
        {
            //Debug.Log("Maximum projectiles reached, cannot shoot");
            return;
        }

        // Check if attack point is assigned
        if (attackPoint == null)
        {
            //Debug.LogWarning("Attack point not assigned!");
            return;
        }

        // Check for player in range
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRangeRanged, playerLayer);

        if (hits.Length > 0)
        {
            Transform player = hits[0].transform;

            if (player != null && projectilePrefab != null)
            {
                // Create projectile
                GameObject projectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
                activeProjectiles.Add(projectile);

                // Calculate direction to player
                Vector2 direction = (player.position - attackPoint.position).normalized;

                // Set projectile velocity
                Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
                if (projectileRb != null)
                {
                    projectileRb.velocity = direction * projectileSpeed;
                }

                // Rotate projectile to face movement direction
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

                // Destroy projectile after lifetime and remove from list
                StartCoroutine(DestroyProjectileAfterTime(projectile, projectileLifeTime));

                //Debug.Log($"Projectile fired at player! Active projectiles: {activeProjectiles.Count}");
            }
        }
        else
        {
            //Debug.Log("No player in attack range to shoot at");
        }
    }
    private System.Collections.IEnumerator DestroyProjectileAfterTime(GameObject projectile, float time)
    {
        yield return new WaitForSeconds(time);

        if (projectile != null)
        {
            activeProjectiles.Remove(projectile);
            Destroy(projectile);
        }
    }
}
