using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatSlot : MonoBehaviour
{
    [SerializeField]
    private StatSO statSO;
    //[SerializeField]
    //private GameObject statSlot;
    public string statName;
    public Image statIcon;
    public Button statButton;
    public static event Action<StatSlot> OnAbilityPointsSpent;

    public TMP_Text statText;
    
    public void OnValidate()
    {
        if (statSO != null)
        {
            //statIcon.sprite = statSO.statIcon;
            statButton.interactable = true;
            statButton.navigation = new Navigation { mode = Navigation.Mode.None };
        }
    }

    public void TryUpgradeStat()
    {
        if (StatManager.instance.upgradePoint > 0)
        {
            OnAbilityPointsSpent?.Invoke(this); //null check to ensure the event is subscribed to
        }
        else
        {
            Debug.Log("Not enough points to upgrade " + statName);
        }
    }
}
