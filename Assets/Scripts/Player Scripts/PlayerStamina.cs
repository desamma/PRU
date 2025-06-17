using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    public Slider staminaSlider;

    private void Start()
    {
        StatManager.instance.currentStamina = StatManager.instance.maxStamina;
        staminaSlider.maxValue = StatManager.instance.maxStamina;
        staminaSlider.value = StatManager.instance.currentStamina;
    }

    void Update()
    {
        if (StatManager.instance.currentStamina < StatManager.instance.maxStamina)
        {
            // Regenerate stamina over time
            StatManager.instance.currentStamina += Time.deltaTime * 15f; // need to make this levelable somehow
            if (StatManager.instance.currentStamina > StatManager.instance.maxStamina)
            {
                StatManager.instance.currentStamina = StatManager.instance.maxStamina;
            }
            staminaSlider.value = StatManager.instance.currentStamina;
        }
    }

    public void UseStamina()
    {
        StatManager.instance.currentStamina -= StatManager.instance.staminaCost;
    }
}
