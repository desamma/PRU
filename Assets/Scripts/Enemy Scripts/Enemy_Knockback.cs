using System.Collections;
using UnityEngine;

public class Enemy_Knockback : MonoBehaviour
{
    private Rigidbody2D rb;
    private Enemy_Movement enemy_Movement;
    public bool isRanged;

    private Enemy_TNT_Movement enemy_tnt_Movement;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (isRanged)
        {
            enemy_tnt_Movement = GetComponent<Enemy_TNT_Movement>();
        }
        else
        {
            enemy_Movement = GetComponent<Enemy_Movement>();
        }
    }

    public void KnockBack(Transform player, float knockbackForce, float knockbackTime, float stunTime)
    {
        if (isRanged)
        {
            enemy_tnt_Movement.ChangeState(EnemyState.Knockback);
        }
        else
        {
            enemy_Movement.ChangeState(EnemyState.Knockback);
        }
        StartCoroutine(KnockBackCounter(knockbackTime, stunTime));
        // Force will be the same regardless of the direction
        Vector2 knockbackDirection = (transform.position - player.position).normalized;
        rb.velocity = knockbackDirection * knockbackForce;
    }

    IEnumerator KnockBackCounter(float knockbackTime, float stunTime)
    {
        yield return new WaitForSeconds(knockbackTime);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(stunTime);
        if (isRanged)
        {
            enemy_tnt_Movement.ChangeState(EnemyState.Idle);
        }
        else
        {
            enemy_Movement.ChangeState(EnemyState.Idle);
        }
    }
}
