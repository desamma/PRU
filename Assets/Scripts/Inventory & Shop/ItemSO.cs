using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
  public string itemName;
  [TextArea] public string itemDescription;
  public Sprite itemIcon;

  public bool isGold; // differentiate gold from other items
  public int stackSize = 10; // forgetting ahh moment

  [Header("Item Stats")] 
  public int currentHealth;
  public int maxHealth;
  public int speed;
  public int damage;

  [Header("For Temporary Items")] public float duration;
}