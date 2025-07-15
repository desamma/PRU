using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public ItemSO itemSO;
    public TMP_Text itemNameText;
    public TMP_Text priceText;
    public Image itemImage;
    public Button shopButton;
    
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private ShopInfo shopInfo;

    public int price;
    
    public void Initialized(ItemSO newItemSO, int newPrice)
    {
        itemSO = newItemSO;
        itemImage.sprite = itemSO.itemIcon;
        itemNameText.text = itemSO.itemName;
        price =  newPrice;
        priceText.text = price.ToString();
        shopButton.interactable = true;
        shopButton.navigation = new Navigation { mode = Navigation.Mode.None };
    }

    public void OnBuyButtonClicked()
    {
        shopManager.TryBuyItem(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemSO != null)
        {
            shopInfo.ShowItemInfo(itemSO);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        shopInfo.HideItemInfo();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (itemSO != null)
        {
            shopInfo.FollowMouse();
        }
    }
}
