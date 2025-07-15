using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask enemyLayer;

    public Animator animator;

    private float timer;
    private float attackRange = 1.5f;

    private void Update()
    {
        if (timer > 0)
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
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, StatManager.instance.weaponRange);

        foreach (Collider2D hitObject in hitObjects)
        {
            // Check if the object is an enemy
            if (hitObject.CompareTag("Enemy"))
            {
                hitObject.GetComponent<Enemy_Health>().ChangeHealth(-StatManager.instance.damage);
                hitObject.GetComponent<Enemy_Knockback>().KnockBack(transform, StatManager.instance.knockbackForce, StatManager.instance.knockbackTime, StatManager.instance.stunTime);
            }
            if (hitObject.CompareTag("Boss1"))
            {
                hitObject.GetComponent<Enemy_Health>().ChangeHealthNoDestroy(-StatManager.instance.damage);
                hitObject.GetComponent<Enemy_Boss1_Knockback>().KnockBack(transform, StatManager.instance.knockbackForce, StatManager.instance.knockbackTime, StatManager.instance.stunTime);
            }
            if (hitObject.CompareTag("Dragon"))
            {
                hitObject.GetComponent<Enemy_Dragon_Health>().ChangeHealth(-StatManager.instance.damage);
                //no knockback for the dragon
            }
            // Check if the object is a barrel
            else if (hitObject.CompareTag("Barrel"))
            {
                if (hitObject.TryGetComponent<Barrel>(out var barrel))
                {
                    barrel.Explode(); // Trigger the barrel's explosion
                }
            }
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
