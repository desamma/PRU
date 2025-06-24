using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItem : MonoBehaviour
{
    public void ApplyItemEffect(ItemSO itemSO)
    {
        if (itemSO.currentHealth > 0)
        {
            StatManager.instance.AddHealth(itemSO.currentHealth);
        }
        if (itemSO.maxHealth > 0)
        {
            StatManager.instance.AddMaxHealth(itemSO.maxHealth);
        }
        if (itemSO.speed > 0)
        {
            StatManager.instance.AddSpeed(itemSO.speed);
        }
        if (itemSO.damage > 0)
        {
            StatManager.instance.AddAttack(itemSO.damage);
        }

        if (itemSO.duration > 0)
        {
            StartCoroutine(EffectTimer(itemSO, itemSO.duration));
        }
    }
    
    private IEnumerator EffectTimer(ItemSO itemSO, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (itemSO.currentHealth > 0)
        {
            StatManager.instance.AddHealth(-itemSO.currentHealth);
        }
        if (itemSO.maxHealth > 0)
        {
            StatManager.instance.AddMaxHealth(-itemSO.maxHealth);
        }
        if (itemSO.speed > 0)
        {
            StatManager.instance.AddSpeed(-itemSO.speed);
        }
        if (itemSO.damage > 0)
        {
            StatManager.instance.AddAttack(-itemSO.damage);
        }
    }
}
