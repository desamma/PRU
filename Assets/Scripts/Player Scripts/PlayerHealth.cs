using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public TMP_Text healthText;
    public Animator hpTextAnim;

    public Slider healthSlider;

    private void Start()
    {
        StatManager.instance.currentHealth = StatManager.instance.maxHealth;
        healthText.text = "HP: " + StatManager.instance.currentHealth + "/" + StatManager.instance.maxHealth;
        healthSlider.maxValue = StatManager.instance.maxHealth;
        healthSlider.value = StatManager.instance.currentHealth;
    }

    public void UpdateHealth()
    {
        Debug.Log("Updating health");
        healthSlider.maxValue = StatManager.instance.maxHealth;
        healthSlider.value = StatManager.instance.currentHealth;
    }

    public void ChangeHealth(int amount)
    {
        Debug.Log("Changing health");
        StatManager.instance.currentHealth += amount;
        healthText.text = "HP: " + StatManager.instance.currentHealth + "/" + StatManager.instance.maxHealth;
        healthSlider.value = StatManager.instance.currentHealth;
        //hpTextAnim.Play("HPTextUpdate");

        if (StatManager.instance.currentHealth <= 0)
        {
            gameObject.SetActive(false); // Deactivate the player object when health is zero or less
        }
    }
}
