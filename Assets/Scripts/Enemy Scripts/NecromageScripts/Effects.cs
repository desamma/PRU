using System.Collections;
using UnityEngine;

public class Effects : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>().ChangeHealth(-damage);
            collision.GetComponent<PlayerMovement>().KnockBack(transform, 0, 0);
            StartCoroutine(WaitForSeconds(0.5f)); // wait for a short time before destroying the projectile
        }
        else if (collision.CompareTag("Barrel"))
        {
            Destroy(gameObject); // destroy the projectile after hitting barrel
            if (collision.TryGetComponent<Barrel>(out var barrel))
            {
                barrel.Explode(); // Trigger the barrel's explosion
            }
        }
    }
    IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject); // destroy the projectile after hitting the player
    }
}
