using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float maxHealth, currentHealth;
    public int damage;
    public float weaponRange, knockbackForce, knockbackTime, stunTime, cooldown;
    public float maxStamina, currentStamina, staminaRegenRate, staminaCost, dashDelay, dashDuration, moveSpeed;
    public int currentExp, expToNextLevel, level, upgradePoint;
    public float checkpointX, checkpointY;
    public InventoryData[]  savedSlotsData;
    public int savedGold;
    public bool savedIsRed;

    public PlayerData(StatManager stats, Vector2 checkpointPos, InventorySlot[] slots, int gold, bool isRed)
    {
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

        savedSlotsData = slots
            .Where(slot => slot != null && slot.itemSO != null)
            .Select(slot => new InventoryData(slot.itemSO.itemName, slot.quantity))
            .ToArray();
        savedGold = gold;
        
        savedIsRed = isRed;
    }

    public Vector2 GetCheckpoint() => new Vector2(checkpointX, checkpointY);
}
