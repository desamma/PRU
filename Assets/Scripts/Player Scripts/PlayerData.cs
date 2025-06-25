using UnityEngine;

[System.Serializable]
public class PlayerData
{
    // Health
    public float maxHealth;
    public float currentHealth;

    // Combat
    public int damage;
    public float weaponRange;
    public float knockbackForce;
    public float knockbackTime;
    public float stunTime;
    public float cooldown;

    // Movement
    public float maxStamina;
    public float currentStamina;
    public float staminaRegenRate;
    public float staminaCost;
    public float dashDelay;
    public float dashDuration;
    public float moveSpeed;

    // Exp
    public int currentExp;
    public int expToNextLevel;
    public int level;
    public int upgradePoint;

    // Checkpoint position
    public float checkpointX;
    public float checkpointY;

    public PlayerData(StatManager stats, Vector2 checkpointPos)
    {
        // Copy all stats
        maxHealth = stats.maxHealth;
        currentHealth = stats.currentHealth;

        damage = stats.damage;
        weaponRange = stats.weaponRange;
        knockbackForce = stats.knockbackForce;
        knockbackTime = stats.knockbackTime;
        stunTime = stats.stunTime;
        cooldown = stats.cooldown;

        maxStamina = stats.maxStamina;
        currentStamina = stats.currentStamina;
        staminaRegenRate = stats.staminaRegenRate;
        staminaCost = stats.staminaCost;
        dashDelay = stats.dashDelay;
        dashDuration = stats.dashDuration;
        moveSpeed = stats.moveSpeed;

        currentExp = stats.currentExp;
        expToNextLevel = stats.expToNextLevel;
        level = stats.level;
        upgradePoint = stats.upgradePoint;

        checkpointX = checkpointPos.x;
        checkpointY = checkpointPos.y;
    }
}
