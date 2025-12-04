using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VendorPanelController : MonoBehaviour
{
    public PlayerContext PlayerContext;
    public UIButton[] Buttons;
    public List<InventoryItemView> ItemsForSale;

    public void TogglePanel()
    {
        TogglePanel(PlayerContext);
    }

    public void TogglePanel(PlayerContext playerContext, List<InventoryItemView> itemsForSale = null)
    {
        PlayerContext = playerContext;
        gameObject.SetActive(!gameObject.activeSelf);

        if (gameObject.activeSelf)
        {
            ItemsForSale = itemsForSale;
            playerContext.UserInterfaceController.eventSystem.SetSelectedGameObject(Buttons[0].gameObject);
            playerContext.InventoryManager.RequestPause();
        }
        else
        {
            playerContext.InventoryManager.RequestUnpause();
            foreach (UIButton button in Buttons)
            {
                button.ToggleHighlight(false);
            }
        }
    }

    public void EnterBuyMode()
    {
        //Opens the VENDOR UI, populated with vendor's items
        PlayerContext.UserInterfaceController.ToggleBuyInventory(ItemsForSale);
        TogglePanel(PlayerContext);
    }

    public void EnterSellMode()
    {
        PlayerContext.UserInterfaceController.ToggleInventory(InventoryMode.Sell);
        TogglePanel(PlayerContext);
        //Opens the PLAYER UI, with inventory item controllers set to SELL mode
    }
}
