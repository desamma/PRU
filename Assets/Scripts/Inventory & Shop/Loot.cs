using System;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public ItemSO itemSO;
    public SpriteRenderer sr;
    public Animator animator;

    public bool canBePickedUp = true; //prevent dropping and picking up at the same time
    private bool hasBeenPickedUp; //prevent duplication when drop
    public int quantity;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || !canBePickedUp || hasBeenPickedUp) return;
        hasBeenPickedUp = true;
        animator.Play("LootPickup");
        OnItemLooted?.Invoke(itemSO, quantity);
        Destroy(gameObject, 0.5f);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        
        canBePickedUp = true;
    }

    private void OnValidate()
    {
        if (itemSO == null) return;
        UpdateSprite();
    }

    public void Initialize(ItemSO item, int itemQuantity)
    {
        itemSO = item;
        quantity = itemQuantity;
        canBePickedUp = false;
        hasBeenPickedUp = false;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        sr.sprite = itemSO.itemIcon;
        name = itemSO.name;
    }

    public static event Action<ItemSO, int> OnItemLooted;
}