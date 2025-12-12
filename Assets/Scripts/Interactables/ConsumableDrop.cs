using System.Collections;
using UnityEngine;

public class ConsumableDrop : Item, IInteractable
{
    public override string interactableName { get => GetInteractableName(); set => ItemSO.ItemName = value; }
    public override InteractionType interactionType { get => InteractionType; set => InteractionType = value; }

    protected override void CollectItem(PlayerContext playerContext)
    {
        if (ItemSO == null)
            ItemSO = ItemData.ItemSO;

        if (playerContext.PlayerController.PlayerStatsBlackboard.WeightCurrent + ItemSO.Weight > playerContext.PlayerController.PlayerStatsBlackboard.WeightMax)
        {
            StartCoroutine(RotateRandomly());
            return;
        }
        if (itemCollected) return;
        itemCollected = true;
        if (ItemSO.ItemDropType == ItemDropType.Consumable)
        {
            playerContext.InventoryController.AddConsumableToInventory(ItemSO, Quantity);
        }
        else
        {
            if (ItemData.ItemSO == null)
            {
                ItemData = SpawnedItemDataBase.Instance.CreateItemData(ItemSO, Quality, Quantity);
            }
            playerContext.InventoryController.AddItemToInventory(ItemData);
        }
        Destroy(gameObject);
    }

    public override string GetInteractableName()
    {
        if (ItemSO != null)
        {
            return ItemSO.ItemName;
        }
        return ItemData.ItemSO.ItemName;
    }

}
