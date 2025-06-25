using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    public GameObject[] statSlots;
    //public StatSlot[] stats;
    public CanvasGroup statsCanvas;
    public TMP_Text availablePointsText;
    [SerializeField]
    private PlayerHealth playerHealth;

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

                // Disable navigation to prevent weird UI behavior
                Navigation nav = statSlot.statButton.navigation;
                nav.mode = Navigation.Mode.None;
                statSlot.statButton.navigation = nav;
            }
        }
        UpdateAbilityPoints();
        statsCanvas.alpha = 0;
        statsCanvas.interactable = false;
        statsCanvas.blocksRaycasts = false;
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
                statsCanvas.alpha = 1; // Make stats visible and enable interaction
                statsCanvas.interactable = true;
                statsCanvas.blocksRaycasts = true;
            }
            else
            {
                Time.timeScale = 1; 
                statsCanvas.alpha = 0;
                statsCanvas.interactable = false;
                statsCanvas.blocksRaycasts = false;
            }
        }
    }

    public void UpdateAbilityPoints()
    {
        availablePointsText.text = "Points: " + StatManager.instance.upgradePoint;
    }

    private void HandleUpdateAbilityPoints(StatSlot slot)
    {
        if(StatManager.instance.upgradePoint > 0)
        {
            StatManager.instance.upgradePoint--;
            availablePointsText.text = "Points: " + StatManager.instance.upgradePoint;
            StatManager.instance.UpgradeStat(slot.statName, 1);
        }
    }
    public void UpdateHealth()
    {
        statSlots[0].GetComponentInChildren<TMP_Text>().text = "Health: " + StatManager.instance.maxHealth;
        playerHealth.UpdateHealth();
    }

    public void UpdateDamage()
    {
        statSlots[1].GetComponentInChildren<TMP_Text>().text = "Damage: " + StatManager.instance.damage;
    }

    public void UpdateSpeed()
    {
        statSlots[2].GetComponentInChildren<TMP_Text>().text = "Speed: " + StatManager.instance.moveSpeed;
    }


    public void UpdateAllStats()
    {
        UpdateHealth();
        UpdateDamage();
        UpdateSpeed();
        UpdateAbilityPoints();
    }
}
