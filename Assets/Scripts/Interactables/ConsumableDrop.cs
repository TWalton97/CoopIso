using System.Collections;
using UnityEngine;

public class ConsumableDrop : Item, IInteractable
{
    public override string interactableName { get => GetInteractableName(); set => ItemSO.ItemName = value; }
    public override InteractionType interactionType { get => InteractionType; set => InteractionType = value; }

    protected override void CollectItem(PlayerContext playerContext)
    {
        if (playerContext.PlayerController.PlayerStatsBlackboard.WeightCurrent + ItemSO.Weight > playerContext.PlayerController.PlayerStatsBlackboard.WeightMax)
        {
            StartCoroutine(RotateRandomly());
            return;
        }
        if (itemCollected) return;
        itemCollected = true;
        playerContext.InventoryController.AddConsumableToInventory(ItemSO, Quantity);
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
