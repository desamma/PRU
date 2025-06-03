using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    public GameObject[] statSlots;
    public CanvasGroup statsCanvas;

    private void Start()
    {
        UpdateAllStats();
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

    public void UpdateDamage()
    {
        statSlots[0].GetComponentInChildren<TMP_Text>().text = "Damage: " + StatManager.instance.damage.ToString();
    }

    public void UpdateSpeed()
    {
        statSlots[1].GetComponentInChildren<TMP_Text>().text = "Speed: " + StatManager.instance.moveSpeed.ToString();
    }

    public void UpdateAllStats()
    {
        UpdateDamage();
        UpdateSpeed();
    }
}
