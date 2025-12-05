using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendorController : MonoBehaviour, IInteractable
{
    private string itemName = "Blacksmith";
    public string interactableName { get => itemName; set => itemName = value; }

    public InteractionType InteractionType;
    public InteractionType interactionType { get => InteractionType; set => InteractionType = value; }

    private bool _isInteractable = true;
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }

    [System.Serializable]
    public class VendorItem
    {
        public string itemID;
        public ItemSO itemSO;
        public ItemQuality itemQuality;
        public float priceMultiplier;
    }
    public List<VendorItem> ItemsForSale1;
    private List<InventoryItemView> vendorItemViews1 = new();

    public List<VendorItem> ItemsForSale2;
    private List<InventoryItemView> vendorItemViews2 = new();

    private bool spawnedItems = false;
    public float VendorWeaponPriceMultiplier;
    public float VendorArmorPriceMultiplier;
    public float ConsumablePriceMultiplier;

    public static Action<string> OnItemPurchased;


    public string GetInteractableName()
    {
        return interactableName;
    }

    void OnEnable()
    {
        OnItemPurchased += RemoveItemFromAvailableItems;
    }

    void OnDisable()
    {
        OnItemPurchased -= RemoveItemFromAvailableItems;
    }

    public void RemoveItemFromAvailableItems(string id)
    {
        foreach (var item in vendorItemViews1)
        {
            if (item.SlotID == id)
            {
                vendorItemViews1.Remove(item);
                return;
            }
        }

        foreach (var item in vendorItemViews2)
        {
            if (item.SlotID == id)
            {
                vendorItemViews2.Remove(item);
                return;
            }
        }
    }

    public void OnInteract(PlayerContext playerContext, int playerIndex)
    {
        if (!spawnedItems)
        {
            GenerateItems();
        }

        for (int i = 0; i < PlayerJoinManager.Instance.playerControllers.Count; i++)
        {
            if (i == 0)
            {
                PlayerJoinManager.Instance.playerControllers[0].PlayerContext.UserInterfaceController.VendorPanelController.ItemsForSale = vendorItemViews1;
            }
            else if (i == 1)
            {
                PlayerJoinManager.Instance.playerControllers[1].PlayerContext.UserInterfaceController.VendorPanelController.ItemsForSale = vendorItemViews2;
            }
        }

        playerContext.InventoryManager.OpenVendorPanel();
    }

    public void WrapVendorItemsForInventoryView()
    {
        foreach (var vendorItem in ItemsForSale1)
        {
            InventoryItemView view = new InventoryItemView(vendorItem.itemSO, null, (vendorItem.itemSO.ItemType == ItemType.Consumable) ? 1 : 1, vendorItem.itemQuality, false, vendorItem.itemID);
            view.GoldValue = (int)(view.DisplayGoldValue * vendorItem.priceMultiplier);
            vendorItemViews1.Add(view);
        }

        foreach (var vendorItem in ItemsForSale2)
        {
            InventoryItemView view = new InventoryItemView(vendorItem.itemSO, null, (vendorItem.itemSO.ItemType == ItemType.Consumable) ? 1 : 1, vendorItem.itemQuality, false, vendorItem.itemID);
            view.GoldValue = (int)(view.DisplayGoldValue * vendorItem.priceMultiplier);
            vendorItemViews2.Add(view);
        }
    }

    private void GenerateItems()
    {
        spawnedItems = true;
        foreach (VendorItem item in ItemsForSale1)
        {
            item.itemID = Guid.NewGuid().ToString();
            item.priceMultiplier = ConsumablePriceMultiplier;
        }

        for (int i = 0; i < 5; i++)
        {
            VendorItem vendorItem = new VendorItem();
            vendorItem.itemID = Guid.NewGuid().ToString();
            vendorItem.itemSO = SpawnedItemDataBase.Instance.ReturnRandomWeaponSO();
            vendorItem.itemQuality = LootCalculator.RollQuality();
            vendorItem.priceMultiplier = VendorWeaponPriceMultiplier;
            ItemsForSale1.Add(vendorItem);
        }

        for (int i = 0; i < 5; i++)
        {
            VendorItem vendorItem = new VendorItem();
            vendorItem.itemID = Guid.NewGuid().ToString();
            vendorItem.itemSO = SpawnedItemDataBase.Instance.ReturnRandomArmorSO();
            vendorItem.itemQuality = LootCalculator.RollQuality();
            vendorItem.priceMultiplier = VendorArmorPriceMultiplier;
            ItemsForSale1.Add(vendorItem);
        }

        foreach (VendorItem item in ItemsForSale2)
        {
            item.itemID = Guid.NewGuid().ToString();
            item.priceMultiplier = ConsumablePriceMultiplier;
        }

        for (int i = 0; i < 5; i++)
        {
            VendorItem vendorItem = new VendorItem();
            vendorItem.itemID = Guid.NewGuid().ToString();
            vendorItem.itemSO = SpawnedItemDataBase.Instance.ReturnRandomWeaponSO();
            vendorItem.itemQuality = LootCalculator.RollQuality();
            vendorItem.priceMultiplier = VendorWeaponPriceMultiplier;
            ItemsForSale2.Add(vendorItem);
        }

        for (int i = 0; i < 5; i++)
        {
            VendorItem vendorItem = new VendorItem();
            vendorItem.itemID = Guid.NewGuid().ToString();
            vendorItem.itemSO = SpawnedItemDataBase.Instance.ReturnRandomArmorSO();
            vendorItem.itemQuality = LootCalculator.RollQuality();
            vendorItem.priceMultiplier = VendorArmorPriceMultiplier;
            ItemsForSale2.Add(vendorItem);
        }
        WrapVendorItemsForInventoryView();
    }
}
