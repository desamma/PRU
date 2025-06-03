using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public static StatManager instance;

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
    public float moveSpeed;

    [Header("Exp Stats")]
    public int currentExp;
    public int expToNextLevel;
    public int level;

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
}
