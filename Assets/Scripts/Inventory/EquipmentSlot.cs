using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class EquipmentSlot : ItemSlot
{
    //Item Slot
    [SerializeField] private Image itemImage;

    //Equipped Slots
    [SerializeField] private EquippedSlot headSlot, bodySlot, legSlot, mainHandSlot, offHandSlot, potionSlotOne, potionSlotTwo;

    public void ImportItemDataToEquipmentSlot(ItemData itemData)
    {
        this.itemData = itemData;
        slotInUse = true;

        itemImage.sprite = itemData.sprite;
    }

    public override void OnLeftClick()
    {
        //If nothing is selected, you can only select slots that have an item in them
        if (inventoryController.selectedItemSlots.Count == 0 && !slotInUse) return;

        if (isSelected)
        {
            EquipGear();
        }
        else
        {
            inventoryController.RegisterButtonSelection(this);

            if (inventoryController.selectedItemSlots.Count == 1)
            {
                isSelected = true;
                selectedShader.SetActive(true);
            }
        }
    }

    private void EquipGear()
    {
        if (!itemData.data.CheckItemRequirements(inventoryController.controller.PlayerStatsBlackboard)) return;

        if (itemData.itemType == ItemType.Consumable)
            potionSlotOne.EquipGear(itemData, inventoryController.playerUserInterfaceController.playerContext.PlayerController);
        if (itemData.itemType == ItemType.Head)
            headSlot.EquipGear(itemData, inventoryController.playerUserInterfaceController.playerContext.PlayerController);
        if (itemData.itemType == ItemType.Body)
            bodySlot.EquipGear(itemData, inventoryController.playerUserInterfaceController.playerContext.PlayerController);
        if (itemData.itemType == ItemType.Legs)
            legSlot.EquipGear(itemData, inventoryController.playerUserInterfaceController.playerContext.PlayerController);
        if (itemData.itemType == ItemType.TwoHanded)
        {
            if (!inventoryController.controller.PlayerStatsBlackboard.TwoHandedMastery)
            {
                offHandSlot.UnequipGear();
                mainHandSlot.EquipGear(itemData, inventoryController.playerUserInterfaceController.playerContext.PlayerController);
            }
            else
            {
                if (!mainHandSlot.slotInUse)
                {
                    mainHandSlot.EquipGear(itemData, inventoryController.playerUserInterfaceController.playerContext.PlayerController);
                    if (offHandSlot.slotInUse && offHandSlot.equippedWeaponType == ItemType.OneHanded)
                    {
                        offHandSlot.UnequipGear();
                    }
                }
                else if (mainHandSlot.equippedWeaponType == ItemType.OneHanded || mainHandSlot.equippedWeaponType == ItemType.Bow)
                {
                    offHandSlot.UnequipGear();
                    mainHandSlot.EquipGear(itemData, inventoryController.playerUserInterfaceController.playerContext.PlayerController);
                }
                else if (!offHandSlot.slotInUse)
                {
                    offHandSlot.EquipGear(itemData, inventoryController.playerUserInterfaceController.playerContext.PlayerController);
                }
                else
                {
                    mainHandSlot.EquipGear(itemData, inventoryController.playerUserInterfaceController.playerContext.PlayerController);
                }
            }
        }

        if (itemData.itemType == ItemType.OneHanded)
        {
            if (!mainHandSlot.slotInUse)
            {
                mainHandSlot.EquipGear(itemData, inventoryController.playerUserInterfaceController.playerContext.PlayerController);
            }
            else if (mainHandSlot.equippedWeaponType == ItemType.TwoHanded)
            {
                if (offHandSlot.slotInUse && offHandSlot.equippedWeaponType == ItemType.TwoHanded)
                {
                    offHandSlot.UnequipGear();
                }
                mainHandSlot.EquipGear(itemData, inventoryController.playerUserInterfaceController.playerContext.PlayerController);
            }
            else if (mainHandSlot.equippedWeaponType == ItemType.Bow)
            {
                mainHandSlot.EquipGear(itemData, inventoryController.playerUserInterfaceController.playerContext.PlayerController);
            }
            else if (!offHandSlot.slotInUse)
            {
                offHandSlot.EquipGear(itemData, inventoryController.playerUserInterfaceController.playerContext.PlayerController);
            }
            else
            {
                mainHandSlot.EquipGear(itemData, inventoryController.playerUserInterfaceController.playerContext.PlayerController);
            }
        }
        if (itemData.itemType == ItemType.Offhand)
        {
            if (mainHandSlot.equippedWeaponType == ItemType.TwoHanded && !inventoryController.controller.PlayerStatsBlackboard.TwoHandedMastery)
            {
                mainHandSlot.UnequipGear();
            }
            offHandSlot.EquipGear(itemData, inventoryController.playerUserInterfaceController.playerContext.PlayerController);
        }

        if (itemData.itemType == ItemType.Bow)
        {
            if (offHandSlot.slotInUse)
            {
                offHandSlot.UnequipGear();
            }
            mainHandSlot.EquipGear(itemData, inventoryController.playerUserInterfaceController.playerContext.PlayerController);
        }
        HidePreview();
        inventoryController.DeselectAllSlots();
        EmptySlot();
    }

    public override void EmptySlot()
    {
        itemImage.sprite = emptySprite;

        slotInUse = false;
        isSelected = false;
        selectedShader.SetActive(false);
    }

    public override void OnRightClick()
    {
        //Create a new item
        GameObject itemToDrop = new GameObject(itemData.itemName);
        Item newItem = itemToDrop.AddComponent<Item>();
        newItem.itemData = itemData;

        if (itemData.floorObjectPrefab == null)
        {
            Instantiate(itemData.objectPrefab, Vector3.zero, Quaternion.identity, itemToDrop.transform);
        }
        else
        {
            Instantiate(itemData.floorObjectPrefab, Vector3.zero, Quaternion.identity, itemToDrop.transform);
        }


        //Add collider
        itemToDrop.AddComponent<SphereCollider>().isTrigger = true;

        GameObject player = PlayerJoinManager.Instance.GetPlayerControllerByIndex(inventoryController.playerIndex).gameObject;
        itemToDrop.transform.position = player.transform.position + (player.transform.forward * 2f);

        EmptySlot();
        HidePreview();
    }
}
