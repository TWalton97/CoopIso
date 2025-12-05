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
    public void OpenInventory()
    {
        ControlsPanel.gameObject.SetActive(true);
        InventoryObjectsParent.SetActive(true);
        PlayerContext.InventoryManager.RequestPause();
        currentIndex = 0;
        OpenMenu(0);
    }

    public void CloseInventory()
    {
        ControlsPanel.gameObject.SetActive(false);
        InventoryObjectsParent.SetActive(false);
        PlayerContext.InventoryManager.RequestUnpause();
    }

    public void AddItemToInventory(ItemData itemData, bool isEquipped = false)
    {
        PlayerContext.PlayerController.PlayerStatsBlackboard.AddCurrentWeight(itemData.ItemSO.Weight);
        InventoryItemView inventoryItemView = new InventoryItemView(itemData.ItemSO, itemData, 1, itemData.Quality, false, itemData.ItemID);
        inventoryItemView.GoldValue = inventoryItemView.DisplayGoldValue;
        FindCorrectInventory(itemData.ItemSO).CreateButtonForItem(inventoryItemView, isEquipped);
    }

    public void AddConsumableToInventory(ItemSO itemSO, int quantity)
    {
        InventoryItemView inventoryItemView = new InventoryItemView(itemSO, null, quantity, ItemQuality.Normal, true);
        inventoryItemView.GoldValue = inventoryItemView.DisplayGoldValue;

        ItemButton button = ConsumablesInventory.FindSlotByItemSO(itemSO);
        if (button != null)
        {
            ConsumableButton cb = button as ConsumableButton;
            cb.UpdateQuantity(quantity);
            button.UpdateUI();
        }
        else
        {
            ConsumablesInventory.CreateButtonForItem(inventoryItemView);
        }

        PlayerContext.PlayerController.PlayerStatsBlackboard.AddCurrentWeight(itemSO.Weight * quantity);
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

    private InventoryItemController FindCorrectInventory(ItemSO itemSO)
    {
        switch (itemSO.ItemType)
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

    public void SetupBuyInventory(List<InventoryItemView> ItemsForSale)
    {
        foreach (InventoryItemView itemData in ItemsForSale)
        {
            FindCorrectInventory(itemData.ItemSO).CreateButtonForBuyItem(itemData);
        }
    }
}

public enum InventoryMode
{
    Normal,
    Buy,
    Sell
}
