using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public PlayerContext PlayerContext;

    public GameObject InventoryObjectsParent;

    public ControlsPanel ControlsPanel;

    public InventoryItemController WeaponInventory;
    public InventoryItemController ArmorInventory;
    public InventoryItemController ConsumablesInventory;

    public PlayerFeatsPanelController FeatsMenu;
    public GameObject PlayerStatsMenu;
    public GlossaryController GlossaryMenu;

    public GameObject[] inventoryPanelGameObjects;
    private int currentIndex = 0;

    private bool toggled = false;

    public InventoryMode InventoryMode = InventoryMode.Normal;

    public void Init(PlayerContext playerContext)
    {
        PlayerContext = playerContext;
        WeaponInventory.PlayerContext = PlayerContext;
        ArmorInventory.PlayerContext = PlayerContext;
        ConsumablesInventory.PlayerContext = PlayerContext;
        ControlsPanel.PlayerContext = playerContext;
        GlossaryMenu.PlayerContext = playerContext;
    }

    public void UpdateControlPanel(List<ControlData> controlData)
    {
        ControlsPanel.UpdateControls(PlayerContext, controlData);
    }

    public void ToggleInventory()
    {
        toggled = !toggled;
        ControlsPanel.gameObject.SetActive(toggled);
        InventoryObjectsParent.SetActive(toggled);
        if (toggled)
        {
            PlayerContext.InventoryManager.RequestPause();
            currentIndex = 0;
            OpenMenu(0);
        }
        else
        {
            PlayerContext.InventoryManager.RequestUnpause();
        }
    }

    public void AddItemToInventory(ItemData itemData, bool isEquipped = false)
    {
        PlayerContext.PlayerController.PlayerStatsBlackboard.AddCurrentWeight(itemData.Weight);
        FindCorrectInventory(itemData).CreateButtonForItem(itemData, isEquipped);
    }

    public void GoToNextMenu()
    {
        ControlsPanel.DisableAllControlPrompts();
        currentIndex++;
        if (currentIndex == 6)
            currentIndex = 0;
        OpenMenu(currentIndex);
    }

    public void GoToPreviousMenu()
    {
        ControlsPanel.DisableAllControlPrompts();
        currentIndex--;
        if (currentIndex == -1)
            currentIndex = 5;
        OpenMenu(currentIndex);
    }

    private void CloseMenus()
    {
        for (int i = 0; i < 6; i++)
        {
            inventoryPanelGameObjects[i].SetActive(false);
        }
    }

    private void OpenMenu(int index)
    {
        CloseMenus();
        inventoryPanelGameObjects[index].SetActive(true);
    }

    private InventoryItemController FindCorrectInventory(ItemData itemData)
    {
        switch (itemData.ItemSO.ItemType)
        {
            case ItemType.OneHanded:
                return WeaponInventory;
            case ItemType.TwoHanded:
                return WeaponInventory;
            case ItemType.Bow:
                return WeaponInventory;

            case ItemType.Head:
                return ArmorInventory;
            case ItemType.Body:
                return ArmorInventory;
            case ItemType.Legs:
                return ArmorInventory;
            case ItemType.Offhand:
                return ArmorInventory;

            case ItemType.Consumable:
                return ConsumablesInventory;
        }
        return null;
    }

    public void ChangeInventoryMode(InventoryMode inventoryMode)
    {
        InventoryMode = inventoryMode;
        WeaponInventory.SwapToInventoryMode(inventoryMode);
        ArmorInventory.SwapToInventoryMode(inventoryMode);
        ConsumablesInventory.SwapToInventoryMode(inventoryMode);
    }

    public void SetupBuyInventory(List<ItemData> itemsForSale)
    {
        foreach (ItemData itemData in itemsForSale)
        {
            FindCorrectInventory(itemData).CreateButtonForBuyItem(itemData);
        }
    }
}

public enum InventoryMode
{
    Normal,
    Buy,
    Sell
}
