using System;
using UnityEngine;

public class Enemy_Dragon_Health : MonoBehaviour
{
    public int expReward = 3;
    public float deathAnimationDuration = 2f; // Duration to wait before destroying

    public delegate void MonsterDefeted(int exp);
    public static event MonsterDefeted OnMonsterDefeated;
    public event Action OnEnemyDestroyed;

    public int currentHealth;
    public int maxHealth;

    private Enemy_Dragon_Movement dragonMovement;
    private bool isDying = false;

    private void Start()
    {
        currentHealth = maxHealth;
        dragonMovement = GetComponent<Enemy_Dragon_Movement>();
    }

    public void ChangeHealth(int amount)
    {
        // Don't process health changes if already dying
        if (isDying) return;

        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartDeathSequence();
        }
    }

    private void StartDeathSequence()
    {
        isDying = true;

        // Change dragon to Die state to play death animation
        if (dragonMovement != null)
        {
            dragonMovement.ChangeState(EnemyDragonState.Die);
        }

        // Disable all colliders so dragon can't interact with anything
        DisableAllColliders();

        // Award experience immediately
        OnMonsterDefeated?.Invoke(expReward);
        OnEnemyDestroyed?.Invoke();

        // Destroy after animation completes
        Invoke(nameof(DestroyDragon), deathAnimationDuration);
    }

    private void DisableAllColliders()
    {
        // Get all colliders on this GameObject and disable them
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        Collider2D[] childColliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in childColliders)
        {
            collider.enabled = false;
        }
    }

    private void DestroyDragon()
    {
        Destroy(gameObject);
    }
}