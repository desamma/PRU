using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryData
{
    public string savedItemName;
    public int savedItemQuantity;

    public InventoryData(string itemName, int itemQuantity)
    {
        savedItemName = itemName;
        savedItemQuantity = itemQuantity;
    }
}
