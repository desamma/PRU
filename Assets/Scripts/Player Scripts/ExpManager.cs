using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpManager : MonoBehaviour
{
    public Slider expSlider;
    public TMP_Text currentLevelText;

    private void Start()
    {
        UpdateUI();
        StatManager.instance.upgradePoint = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GainExp(5); // Gain 5 exp for testing
        }
    }

    private void OnEnable()
    {
        Enemy_Health.OnMonsterDefeated += GainExp;
    }

    public void GainExp(int amount)
    {
        StatManager.instance.currentExp += amount;
        if (StatManager.instance.currentExp >= StatManager.instance.expToNextLevel)
        {
            LevelUp();
        }
        UpdateUI();
    }

    private void LevelUp()
    {
        StatManager.instance.level++;
        StatManager.instance.upgradePoint++;
        StatManager.instance.currentExp -= StatManager.instance.expToNextLevel;
        StatManager.instance.expToNextLevel = Mathf.RoundToInt(StatManager.instance.expToNextLevel * 1.2f); // Increase difficulty for next level
    }

    public void UpdateUI()
    {
        expSlider.maxValue = StatManager.instance.expToNextLevel;
        expSlider.value = StatManager.instance.currentExp;
        currentLevelText.text = "Level: " + StatManager.instance.level.ToString();
    }

    private void OnDisable()
    {
        Enemy_Health.OnMonsterDefeated -= GainExp;
    }
}
