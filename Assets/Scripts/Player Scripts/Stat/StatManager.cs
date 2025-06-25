using UnityEngine;

public class StatManager : MonoBehaviour
{
    public static StatManager instance;
    [SerializeField]
    private StatsUI statsUI;

    [Header("Health Stats")]
    public float maxHealth;
    public float currentHealth;

    [Header("Combat Stats")]
    public int damage;
    public float weaponRange;
    public float knockbackForce;
    public float knockbackTime;
    public float stunTime;
    public float cooldown;

    [Header("Movement Stats")]
    public float maxStamina;
    public float currentStamina;
    public float staminaRegenRate;
    public float staminaCost;
    public float dashDelay;
    public float dashDuration;
    public float moveSpeed;

    [Header("Exp Stats")]
    public int currentExp;
    public int expToNextLevel;
    public int level;
    public int upgradePoint;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Later
    //private void Start()
    //{
    //    // Initialize stats
    //    maxHealth = 100f;
    //    currentHealth = maxHealth;
    //    damage = 10;
    //    weaponRange = 5f;
    //    knockbackForce = 10f;
    //    knockbackTime = 0.5f;
    //    stunTime = 1f;
    //    cooldown = 1f;
    //    moveSpeed = 5f;
    //    currentExp = 0;
    //    expToNextLevel = 10;
    //    level = 1;
    //    upgradePoint = 0;
    //}
    
    public void UpgradeStat(string statName, int amount)
    {
        switch (statName)
        {
            case "MaxHealth":
                AddMaxHealth(amount);
                break;
            case "Attack":
                AddAttack(amount);
                break;
            case "Speed":
                AddSpeed(amount);
                break;
            default:
                Debug.LogWarning("Unknown stat: " + statName);
                break;
        }
    }
    
    public void AddHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        statsUI.UpdateHealth();
    }

    public void AddMaxHealth(int amount)
    {
        maxHealth += amount;
        statsUI.UpdateHealth();
    }

    public void AddAttack(int amount)
    {
        damage += amount;
        statsUI.UpdateDamage();
    }

    public void AddSpeed(int amount)
    {
        moveSpeed += amount;
        statsUI.UpdateSpeed();
    }
}
