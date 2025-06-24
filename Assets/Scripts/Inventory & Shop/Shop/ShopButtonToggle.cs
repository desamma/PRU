using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButtonToggle : MonoBehaviour
{
    public void OpenItemSHop()
    {
        if (ShopKeeper.currentShopKeeper != null)
        {
            ShopKeeper.currentShopKeeper.OpenItemShop();
        }
    }
    
    public void OpenWeaponSHop()
    {
        if (ShopKeeper.currentShopKeeper != null)
        {
            ShopKeeper.currentShopKeeper.OpenWeaponShop();
        }
    }
    
    public void OpenArmourSHop()
    {
        if (ShopKeeper.currentShopKeeper != null)
        {
            ShopKeeper.currentShopKeeper.OpenArmourShop();
        }
    }
    
}
