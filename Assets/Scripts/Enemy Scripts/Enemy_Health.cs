using System;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    public int expReward = 3;

    public delegate void MonsterDefeted(int exp);
    public static event MonsterDefeted OnMonsterDefeated;
    public event Action OnEnemyDestroyed;
    public bool IsDead = false;
    public int currentHealth;
    public int maxHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        if (currentHealth <= 0)
        {
            IsDead = true;
        }
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            OnMonsterDefeated(expReward);
            OnEnemyDestroyed?.Invoke(); // Notify the spawner
            Destroy(gameObject);
        }
    }
    public void ChangeHealthNoDestroy(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            IsDead = true;
            OnMonsterDefeated(expReward);
            OnEnemyDestroyed?.Invoke();
            //animation and stuff
        }

    }
}
