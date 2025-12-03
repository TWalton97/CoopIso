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

    public List<ItemData> ItemDatasForSale;

    private bool spawnedItems = false;


    public string GetInteractableName()
    {
        return interactableName;
    }

    public void OnInteract(PlayerContext playerContext, int playerIndex)
    {
        if (!spawnedItems)
        {
            spawnedItems = true;
            for (int i = 0; i < Random.Range(10, 20); i++)
            {
                //ItemDatasForSale.Add(ItemsForSale[i].itemData.Clone());
                ItemDatasForSale.Add(SpawnedItemDataBase.Instance.CreateItemData());
            }
        }

        playerContext.UserInterfaceController.ToggleVendorPanel(ItemDatasForSale);
    }
}
