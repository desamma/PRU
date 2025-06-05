using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask enemyLayer;

    public Animator animator;

    private float timer;
    private float attackRange = 1;

    private void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public void Attack()
    {
        if (timer <= 0)
        {
            animator.SetBool("isAttacking", true);
            timer = StatManager.instance.cooldown;
        }
    }

    public void DealDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, StatManager.instance.weaponRange, enemyLayer);

        if (enemies.Length > 0)
        {
            enemies[0].GetComponent<Enemy_Health>().ChangeHealth(-StatManager.instance.damage);
            enemies[0].GetComponent<Enemy_Knockback>().KnockBack(transform, StatManager.instance.knockbackForce, StatManager.instance.knockbackTime, StatManager.instance.stunTime);
        }
    }

    public void FinishAttacking()
    {
        animator.SetBool("isAttacking", false);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
