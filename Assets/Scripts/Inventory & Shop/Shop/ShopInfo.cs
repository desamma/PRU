using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopInfo : MonoBehaviour
{
    public CanvasGroup infoPanel;
    
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;
    
    [Header("Stat Field")]
    public TMP_Text[] statText;
    
    private RectTransform infoPanelRect;

    private void Awake()
    {
        infoPanelRect = GetComponent<RectTransform>();
    }

    public void ShowItemInfo(ItemSO itemSO)
    {
        infoPanel.alpha = 1;
        
        itemNameText.text = itemSO.name;
        itemDescriptionText.text = itemSO.itemDescription;
        
        var stats = new List<string>();
        
        if (itemSO.currentHealth > 0) stats.Add("Health: " + itemSO.currentHealth);
        if (itemSO.maxHealth > 0) stats.Add("Max Health: " + itemSO.maxHealth);
        if (itemSO.damage > 0) stats.Add("Damage: " + itemSO.damage);
        if (itemSO.speed > 0) stats.Add("Speed: " + itemSO.speed);
        if (itemSO.duration > 0) stats.Add("Duration: " + itemSO.duration);

        if (stats.Count <= 0) return;

        for (var i = 0; i < statText.Length; i++)
        {
            if (i < stats.Count)
            {
                statText[i].text = stats[i];
                statText[i].gameObject.SetActive(true);
            }
            else
            {
                statText[i].gameObject.SetActive(false);
            }
        }
    }
    
    public void HideItemInfo()
    {
        infoPanel.alpha = 0;
        
        itemNameText.text = "";
        itemDescriptionText.text = "";
    }

    public void FollowMouse()
    {
        var mousePos = Input.mousePosition;
        var offset = new Vector3(10, -10, 0);
        
        infoPanelRect.position = mousePos + offset;
    }
}
