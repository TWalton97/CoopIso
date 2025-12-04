using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendorController : MonoBehaviour, IInteractable
{
    //Has a function "Generate Items" which replaces the current List of items with new ones

    //When interacted with, displays a "Buy/Sell" UI menu

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
    public List<VendorItem> ItemsForSale;
    private List<InventoryItemView> vendorItemViews = new();

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
        foreach (var item in vendorItemViews)
        {
            if (item.SlotID == id)
            {
                vendorItemViews.Remove(item);
                return;
            }
        }
    }

    public void OnInteract(PlayerContext playerContext, int playerIndex)
    {
        if (!spawnedItems)
        {
            spawnedItems = true;
            foreach (VendorItem item in ItemsForSale)
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
                ItemsForSale.Add(vendorItem);
            }

            for (int i = 0; i < 5; i++)
            {
                VendorItem vendorItem = new VendorItem();
                vendorItem.itemID = Guid.NewGuid().ToString();
                vendorItem.itemSO = SpawnedItemDataBase.Instance.ReturnRandomArmorSO();
                vendorItem.itemQuality = LootCalculator.RollQuality();
                vendorItem.priceMultiplier = VendorArmorPriceMultiplier;
                ItemsForSale.Add(vendorItem);
            }
            WrapVendorItemsForInventoryView();
        }
        playerContext.UserInterfaceController.ToggleVendorPanel(vendorItemViews);
    }

    public void WrapVendorItemsForInventoryView()
    {
        foreach (var vendorItem in ItemsForSale)
        {
            InventoryItemView view = new InventoryItemView(vendorItem.itemSO, null, (vendorItem.itemSO.ItemType == ItemType.Consumable) ? 1 : 1, vendorItem.itemQuality, false, vendorItem.itemID);
            view.GoldValue = (int)(view.DisplayGoldValue * vendorItem.priceMultiplier);
            vendorItemViews.Add(view);
        }
    }
}
