using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    public static ShopKeeper currentShopKeeper; //help differentiate shop content
    
    public Animator animator;
    public CanvasGroup shopCanvasGroup;
    public ShopManager shopManager;
    
    [SerializeField] private List<ShopItems> shopItems;
    [SerializeField] private List<ShopItems> shopWeapons;
    [SerializeField] private List<ShopItems> shopArmors;
    public static event Action<ShopManager, bool> OnShopStateChange;

    private bool playerInRange;
    private bool isShopOpen;

    // Update is called once per frame
    void Update()
    {
        if (!playerInRange) return;
        if (Input.GetButtonDown("Interact"))
        {
            if (!isShopOpen)
            {
                Time.timeScale = 0;
                currentShopKeeper = this;
                shopCanvasGroup.alpha = 1;
                OnShopStateChange?.Invoke(shopManager, true);
                OpenItemShop();
                shopCanvasGroup.interactable = true;
                shopCanvasGroup.blocksRaycasts = true;
                isShopOpen = true;
            }
            else
            {
                Time.timeScale = 1;
                currentShopKeeper = null;
                shopCanvasGroup.alpha = 0;
                OnShopStateChange?.Invoke(shopManager, false);
                shopCanvasGroup.interactable = false;
                shopCanvasGroup.blocksRaycasts = false;
                isShopOpen = false;
            }
        }
    }

    public void OpenItemShop()
    {
        shopManager.PopulateShopItems(shopItems);
    }

    public void OpenWeaponShop()
    {
        shopManager.PopulateShopItems(shopWeapons);
    }

    public void OpenArmourShop()
    {
        shopManager.PopulateShopItems(shopArmors);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            animator.SetBool("playerInRange", true);
            playerInRange = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            animator.SetBool("playerInRange", false);
            playerInRange = false;
        }
    }
}
