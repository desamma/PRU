using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    public int expReward = 3;

    public delegate void MonsterDefeted(int exp);
    public static event MonsterDefeted OnMonsterDefeated;
    public event Action OnEnemyDestroyed;

    public int currentHealth;
    public int maxHealth;

    private void Start()
    {
        currentHealth = maxHealth;
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
}
