using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private ShopSlot[] shopSlots;
    
    [SerializeField] private InventoryManager inventoryManager;

    public void PopulateShopItems(List<ShopItems> shopItems)
    {
        for (var i = 0; i < shopItems.Count && i < shopSlots.Length; i++)
        {
            var shopItem = shopItems[i];
            shopSlots[i].Initialized(shopItem.itemSO, shopItem.price);
            shopSlots[i].gameObject.SetActive(true);
        }

        //disable empty shop slot
        for (var i = shopItems.Count; i < shopSlots.Length; i++)
        {
            shopSlots[i].gameObject.SetActive(false);
        }
    }

    public void TryBuyItem(ItemSO itemSO, int price)
    {
        if (itemSO != null && inventoryManager.gold >= price)
        {
            if (HasSpaceForItem(itemSO))
            {
                inventoryManager.gold -= price;
                inventoryManager.goldText.text = inventoryManager.gold.ToString();
                inventoryManager.AddItem(itemSO, 1);
            }
        }
    }

    private bool HasSpaceForItem(ItemSO itemSO)
    {
        foreach (var slot in inventoryManager.inventorySlots)
        {
            if (slot.itemSO == itemSO && slot.quantity < itemSO.stackSize)
            {
                return true;
            }

            if(slot.itemSO == null)
            {
                return true;
            }
        }
        return false;
    }

    public void SellItem(ItemSO itemSO)
    {
        if (itemSO == null) return;

        foreach (var slot in shopSlots)
        {
            if (slot.itemSO == itemSO)
            {
                inventoryManager.gold += Mathf.FloorToInt(slot.price * 0.7f);
                inventoryManager.goldText.text = inventoryManager.gold.ToString();
                return;
            }
        }
    }
}

[System.Serializable]
public class ShopItems
{
    public ItemSO itemSO;
    public int price;
}