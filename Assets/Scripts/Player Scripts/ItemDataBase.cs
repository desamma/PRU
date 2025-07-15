using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDataBase : MonoBehaviour
{
    public static ItemDataBase instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    [SerializeField]
    private ItemSO[] items;

    public ItemSO GetItem(string itemName)
    {
        return items.FirstOrDefault(item => item.itemName == itemName);
    }
}
