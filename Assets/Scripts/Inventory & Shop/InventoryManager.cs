using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] inventorySlots;
    public UseItem useItem;
    public GameObject itemPrefab;
    public Transform player;

    public int gold;
    public TMP_Text goldText;

    private void Start()
    {
        foreach (var slot in inventorySlots)
        {
            slot.UpdateUI();
        }
    }
    
    private void OnEnable()
    {
        Loot.OnItemLooted += AddItem;
    }

    private void OnDisable()
    {
        Loot.OnItemLooted -= AddItem;
    }

    public void AddItem(ItemSO item, int quantity)
    {
        if (item.isGold)
        {
            gold += quantity;
            goldText.text = gold.ToString();
            return;
        }

        foreach (var slot in inventorySlots)
        {
            if (slot.itemSO == item && slot.quantity < item.stackSize)
            {
                var availableSpace = item.stackSize - slot.quantity;
                var amountToAdd = Mathf.Min(availableSpace, quantity);
                
                slot.quantity += amountToAdd;
                quantity -= amountToAdd; // move excess to other slot
                
                slot.UpdateUI();
                
                if (quantity <= 0) return;
            }
        }
        
        foreach (var slot in inventorySlots)
        {
            if (slot.itemSO != null) continue;
            var amountToAdd = Mathf.Min(item.stackSize, quantity);
            slot.itemSO = item;
            slot.quantity = amountToAdd;
            slot.UpdateUI();
            return;
        }

        if (quantity > 0)
        {
            DropLoot(item, quantity);
        }
    }

    public void DropItem(InventorySlot slot)
    {
        DropLoot(slot.itemSO, 1);
        slot.quantity--;
        if (slot.quantity <= 0)
        {
            slot.itemSO = null;
        }
        slot.UpdateUI();
    }

    private void DropLoot(ItemSO itemSO, int quantity)
    {
       var loot = Instantiate(itemPrefab, player.position, Quaternion.identity).GetComponent<Loot>();
       loot.Initialize(itemSO, quantity);
    }

    public void UseItem(InventorySlot slot)
    {
        if (slot.itemSO != null && slot.quantity > 0)
        {
            useItem.ApplyItemEffect(slot.itemSO);
            
            slot.quantity--;
            if (slot.quantity <= 0)
            {
                slot.itemSO = null;
            }
            slot.UpdateUI();
        }
    }
}