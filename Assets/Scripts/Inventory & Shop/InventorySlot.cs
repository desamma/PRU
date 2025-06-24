using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public ItemSO itemSO;
    public int quantity;

    public Image itemImage;
    public TMP_Text quantityText;
    
    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GetComponentInParent<InventoryManager>();
    }

    /*private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            foreach (var result in results)
            {
                Debug.Log("Raycast hit: " + result.gameObject.name);
            }
        }
    }*/

    public void UpdateUI()
    {
        if (itemSO != null)
        {
            itemImage.sprite = itemSO.itemIcon;
            itemImage.gameObject.SetActive(true); //If there is an item, turn on the slot's Image game object
            quantityText.text = quantity.ToString();
        }
        else
        {
              itemImage.gameObject.SetActive(false);
              quantityText.text = "";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (quantity > 0)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (itemSO.currentHealth > 0 && StatManager.instance.currentHealth >= StatManager.instance.maxHealth) return;
                inventoryManager.UseItem(this);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                inventoryManager.DropItem(this);
            }
        }
    }
}