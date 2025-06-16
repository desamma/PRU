using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    public GameObject[] statSlots;
    //public StatSlot[] stats;
    public CanvasGroup statsCanvas;
    public TMP_Text availablePointsText;

    private void OnEnable()
    {
        StatSlot.OnAbilityPointsSpent += HandleUpdateAbilityPoints;
    }

    private void OnDisable()
    {
        StatSlot.OnAbilityPointsSpent -= HandleUpdateAbilityPoints;
    }

    private void Start()
    {
        UpdateAllStats();
        //foreach (StatSlot slot in stats)
        //{
        //    slot.statButton.onClick.AddListener(slot.TryUpgradeStat);
        //}
        foreach(GameObject slot in statSlots)
        {
            StatSlot statSlot = slot.GetComponent<StatSlot>();
            if (statSlot != null)
            {
                statSlot.statButton.onClick.AddListener(statSlot.TryUpgradeStat);
            }
        }
        UpdateAbilityPoints();
    }

    private void Update()
    {
        if (Input.GetButtonDown("ToggleStats"))
        {
            // Pause game and show stats
            if (statsCanvas.alpha == 0)
            {
                Time.timeScale = 0; 
                UpdateAllStats(); 
                statsCanvas.alpha = 1; // Make stats visible
            }
            else
            {
                Time.timeScale = 1; 
                statsCanvas.alpha = 0;
            }
        }
    }

    public void UpdateAbilityPoints()
    {
        availablePointsText.text = "Points: " + StatManager.instance.upgradePoint.ToString();
    }

    private void HandleUpdateAbilityPoints(StatSlot slot)
    {
        if(StatManager.instance.upgradePoint > 0)
        {
            StatManager.instance.upgradePoint--;
            availablePointsText.text = "Points: " + StatManager.instance.upgradePoint.ToString();
            StatManager.instance.UpgradeStat(slot.statName, 1);
            UpdateAllStats();
        }
    }
    public void UpdateHealth()
    {
        statSlots[0].GetComponentInChildren<TMP_Text>().text = "Health: " + StatManager.instance.maxHealth.ToString();
    }

    public void UpdateDamage()
    {
        statSlots[1].GetComponentInChildren<TMP_Text>().text = "Damage: " + StatManager.instance.damage.ToString();
    }

    public void UpdateSpeed()
    {
        statSlots[2].GetComponentInChildren<TMP_Text>().text = "Speed: " + StatManager.instance.moveSpeed.ToString();
    }


    public void UpdateAllStats()
    {
        UpdateHealth();
        UpdateDamage();
        UpdateSpeed();
        UpdateAbilityPoints();
    }
}
