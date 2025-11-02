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
            if (slotInUse)
                inventoryController.UpdatePreviewWindow(itemData.sprite, itemData.itemName, itemData.itemType, itemData);

            if (inventoryController.selectedItemSlots.Count == 1)
            {
                isSelected = true;
                selectedShader.SetActive(true);
            }
        }
    }

    private void EquipGear()
    {
        if (itemData.itemType == ItemType.Consumable)
            potionSlotOne.EquipGear(itemData);
        if (itemData.itemType == ItemType.Head)
            headSlot.EquipGear(itemData);
        if (itemData.itemType == ItemType.Body)
            bodySlot.EquipGear(itemData);
        if (itemData.itemType == ItemType.Legs)
            legSlot.EquipGear(itemData);
        if (itemData.itemType == ItemType.TwoHanded)
        {
            mainHandSlot.EquipGear(itemData);
            offHandSlot.UnequipGear();
        }

        if (itemData.itemType == ItemType.OneHanded)
        {
            if (!mainHandSlot.slotInUse)
            {
                mainHandSlot.EquipGear(itemData);
            }
            else if (mainHandSlot.equippedWeaponType == ItemType.TwoHanded)
            {
                mainHandSlot.EquipGear(itemData);
            }
            else if (!offHandSlot.slotInUse)
            {
                offHandSlot.EquipGear(itemData);
            }
            else
            {
                mainHandSlot.EquipGear(itemData);
            }
        }
        if (itemData.itemType == ItemType.Offhand)
        {
            offHandSlot.EquipGear(itemData);
            if (mainHandSlot.equippedWeaponType == ItemType.TwoHanded)
            {
                mainHandSlot.UnequipGear();
            }
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

        Instantiate(itemData.objectPrefab, Vector3.zero, Quaternion.identity, itemToDrop.transform);

        //Add collider
        itemToDrop.AddComponent<SphereCollider>().isTrigger = true;

        GameObject player = GameObject.FindWithTag("Player");
        itemToDrop.transform.position = player.transform.position + (player.transform.forward * 2f);

        this.itemData.quantity -= 1;
        if (this.itemData.quantity <= 0)
        {
            EmptySlot();
            HidePreview();
        }

    }
}
