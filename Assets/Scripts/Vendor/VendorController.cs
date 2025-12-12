using System;
using System.Collections.Generic;
using UnityEngine;

public class VendorController : MonoBehaviour, IInteractable
{
    //------------------------------------------------------------
    // REQUIRED BY IInteractable (unchanged)
    //------------------------------------------------------------
    [SerializeField] private string itemName = "Blacksmith";
    public string interactableName { get => itemName; set => itemName = value; }

    public InteractionType InteractionType;
    public InteractionType interactionType { get => InteractionType; set => InteractionType = value; }

    private bool _isInteractable = true;
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }

    //------------------------------------------------------------
    // ORIGINAL STRUCTS (slightly upgraded)
    //------------------------------------------------------------
    [Serializable]
    public class VendorItem
    {
        public string itemID;
        public ItemData itemData;
    }

    public class VendorStock
    {
        public int playerIndex;
        public int playerLevel;        // Stored level to detect changes
        public List<VendorItem> VendorItems = new();
        public bool spawnedItems = false;
    }

    //------------------------------------------------------------
    // INTERNAL STORAGE
    //------------------------------------------------------------
    public List<VendorStock> VendorStocks = new();
    public List<VendorItem> ItemsForSale1 = new();   // Player 1
    public List<VendorItem> ItemsForSale2 = new();   // Player 2

    private List<InventoryItemView> vendorItemViews1 = new();
    private List<InventoryItemView> vendorItemViews2 = new();

    private bool createdVendorStocks = false;

    public PotionSO[] Potions;

    //------------------------------------------------------------
    // EVENT BINDINGS (unchanged)
    //------------------------------------------------------------
    public static Action<string> OnItemPurchased;

    void OnEnable()
    {
        OnItemPurchased += RemoveItemFromAvailableItems;
    }

    void OnDisable()
    {
        OnItemPurchased -= RemoveItemFromAvailableItems;
    }

    //------------------------------------------------------------
    // REMOVE PURCHASED ITEMS (unchanged)
    //------------------------------------------------------------
    public void RemoveItemFromAvailableItems(string id)
    {
        vendorItemViews1.RemoveAll(v => v.SlotID == id);
        vendorItemViews2.RemoveAll(v => v.SlotID == id);

        ItemsForSale1.RemoveAll(v => v.itemID == id);
        ItemsForSale2.RemoveAll(v => v.itemID == id);
    }

    //------------------------------------------------------------
    // INTERACTION ENTRY POINT (kept intact)
    //------------------------------------------------------------
    public void OnInteract(PlayerContext playerContext, int playerIndex)
    {
        if (!createdVendorStocks)
        {
            CreateVendorStocks();
            createdVendorStocks = true;
        }

        GenerateItemsPerPlayerIfNeeded();

        // ⬇ EXACT SAME UI ROUTING YOU USE
        for (int i = 0; i < PlayerJoinManager.Instance.playerControllers.Count; i++)
        {
            var controller = PlayerJoinManager.Instance.playerControllers[i];
            var vendorPanel = controller.PlayerContext.UserInterfaceController.VendorPanelController;

            if (i == 0) vendorPanel.ItemsForSale = vendorItemViews1;
            else vendorPanel.ItemsForSale = vendorItemViews2;
        }

        playerContext.InventoryManager.OpenVendorPanel();
    }

    //------------------------------------------------------------
    // CREATES ONE STOCK PER PLAYER (kept intact)
    //------------------------------------------------------------
    public void CreateVendorStocks()
    {
        PlayerJoinManager pm = PlayerJoinManager.Instance;

        foreach (var kvp in pm.playerControllers)
        {
            var controller = kvp.Value;

            VendorStock vs = new VendorStock
            {
                playerIndex = controller.PlayerContext.PlayerIndex,
                playerLevel = controller.ExperienceController.level,
                spawnedItems = false,
                VendorItems = new List<VendorItem>()
            };

            VendorStocks.Add(vs);
        }
    }

    //------------------------------------------------------------
    // MAIN LOGIC: GENERATE OR REGENERATE INVENTORY
    //------------------------------------------------------------
    private void GenerateItemsPerPlayerIfNeeded()
    {
        ItemsForSale1.Clear();
        ItemsForSale2.Clear();
        vendorItemViews1.Clear();
        vendorItemViews2.Clear();

        foreach (VendorStock vs in VendorStocks)
        {
            int currentLevel = PlayerJoinManager.Instance.playerControllers[vs.playerIndex].ExperienceController.level;

            bool levelChanged = currentLevel != vs.playerLevel;

            if (!vs.spawnedItems)
            {
                GenerateForStock(vs, currentLevel);
                vs.spawnedItems = true;
            }
            else if (levelChanged)
            {
                vs.playerLevel = currentLevel;
                GenerateForStock(vs, currentLevel);
            }

            // Assign based on player index
            if (vs.playerIndex == 0)
            {
                ItemsForSale1.AddRange(vs.VendorItems);
            }
            else if (levelChanged)
            {
                ItemsForSale2.AddRange(vs.VendorItems);
            }
        }

        WrapVendorItemsForInventoryView();
    }

    //------------------------------------------------------------
    // GENERATES 10 ITEMS FOR A PLAYER'S VENDOR LIST
    //------------------------------------------------------------
    private void GenerateForStock(VendorStock vs, int level)
    {
        vs.VendorItems.Clear();

        // 5 weapons + 5 armor (your original logic)
        for (int i = 0; i < 10; i++)
        {
            vs.VendorItems.Add(CreateVendorItem(SpawnedItemDataBase.Instance.ReturnRandomWeaponSO(), level));
        }

        for (int i = 0; i < 10; i++)
        {
            vs.VendorItems.Add(CreateVendorItem(SpawnedItemDataBase.Instance.ReturnRandomArmorSO(), level));
        }

        for (int i = 0; i < Potions.Length; i++)
        {
            vs.VendorItems.Add(CreateVendorItem(Potions[i], level));
        }
    }

    private VendorItem CreateVendorItem(ItemSO so, int level)
    {
        VendorItem v = new VendorItem();
        ItemData data = SpawnedItemDataBase.Instance.CreateItemData(so);
        data.Quality = LootCalculator.RollQualityForEnemyLevel(level);
        return new VendorItem
        {
            itemData = data,
            itemID = data.ItemID
        };
    }

    //------------------------------------------------------------
    // INVENTORY WRAPPING (kept intact)
    //------------------------------------------------------------
    public void WrapVendorItemsForInventoryView()
    {
        foreach (var v in ItemsForSale1)
        {
            vendorItemViews1.Add(new InventoryItemView(
                v.itemData.ItemSO,
                v.itemData,
                1,
                v.itemData.Quality,
                false,
                v.itemID
            ));
        }

        foreach (var v in ItemsForSale2)
        {
            vendorItemViews2.Add(new InventoryItemView(
                v.itemData.ItemSO,
                v.itemData,
                1,
                v.itemData.Quality,
                false,
                v.itemID
            ));
        }
    }

    //------------------------------------------------------------
    // REQUIRED BY IInteractable
    //------------------------------------------------------------
    public string GetInteractableName()
    {
        return interactableName;
    }
}
