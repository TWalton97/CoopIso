using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EquippedSlot : ItemSlot
{
    //Slot appearance
    [SerializeField] private TMP_Text slotName;

    public Slot slotType;
    public ItemType equippedWeaponType { get; private set; }

    public override void OnLeftClick()
    {
        if (inventoryController.selectedItemSlots.Count == 0 && !slotInUse) return;

        if (isSelected)
        {
            UnequipGear();
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

    public override void OnRightClick()
    {
        UnequipGear();
        HidePreview();
    }

    public void EquipGear(ItemData itemData, int index = 0, bool deleteItemInSlot = false)
    {
        if (slotInUse)
            UnequipGear(index, deleteItemInSlot);

        this.itemData = itemData;
        slotImage.sprite = itemData.sprite;
        slotImage.enabled = true;
        slotName.enabled = false;
        equippedWeaponType = itemData.itemType;

        if (itemData.itemType == ItemType.OneHanded)
        {
            PlayerJoinManager.Instance.GetPlayerControllerByIndex(inventoryController.playerIndex).WeaponController.EquipOneHandedWeapon(itemData.objectPrefab, itemData);
            if (slotType == Slot.OffHand)
            {
                //Check if the slot in the main hand is two-handed and unequip it if it is
                EquippedSlot mainHandSlot = inventoryController.FindEquippedSlotOfType(Slot.MainHand)[0];
                if (mainHandSlot.slotInUse && mainHandSlot.itemData.itemType == ItemType.TwoHanded)
                {
                    mainHandSlot.UnequipGear();
                }
            }
            //NewWeaponController.Instance.EquipOneHandedWeapon(weapon);
        }
        else if (itemData.itemType == ItemType.Offhand)
        {
            PlayerJoinManager.Instance.GetPlayerControllerByIndex(inventoryController.playerIndex).WeaponController.EquipOffhand(itemData.objectPrefab, itemData);
            EquippedSlot mainHandSlot = inventoryController.FindEquippedSlotOfType(Slot.MainHand)[0];
            if (mainHandSlot.slotInUse && mainHandSlot.itemData.itemType == ItemType.TwoHanded)
            {
                mainHandSlot.UnequipGear();
            }
            //NewWeaponController.Instance.EquipOffhand(weapon);
        }
        else if (itemData.itemType == ItemType.TwoHanded)
        {
            inventoryController.FindEquippedSlotOfType(Slot.OffHand)[0].UnequipGear();
            PlayerJoinManager.Instance.GetPlayerControllerByIndex(inventoryController.playerIndex).WeaponController.EquipTwoHandedWeapon(itemData.objectPrefab, itemData);
            //NewWeaponController.Instance.EquipTwoHandedWeapon(weapon);
        }

        slotInUse = true;
    }

    public override void EmptySlot()
    {
        HidePreview();
        UnequipGear();
    }

    public void UnequipGear(int index = 0, bool deleteItemInSlot = false)
    {
        inventoryController.DeselectAllSlots();
        if (slotInUse && !deleteItemInSlot)
        {
            inventoryController.AddItemToSelectedSlot(itemData, index);
        }

        slotImage.sprite = emptySprite;
        slotImage.enabled = false;
        slotName.enabled = true;
        selectedShader.SetActive(false);
        slotInUse = false;

        switch (slotType)
        {
            case Slot.MainHand:
                PlayerJoinManager.Instance.GetPlayerControllerByIndex(inventoryController.playerIndex).WeaponController.UnequipWeapon(Weapon.WeaponHand.MainHand);
                //NewWeaponController.Instance.UnequipWeapon(Weapon.WeaponHand.MainHand);
                break;
            case Slot.OffHand:
                PlayerJoinManager.Instance.GetPlayerControllerByIndex(inventoryController.playerIndex).WeaponController.UnequipWeapon(Weapon.WeaponHand.OffHand);
                //NewWeaponController.Instance.UnequipWeapon(Weapon.WeaponHand.OffHand);
                break;
        }
        PlayerJoinManager.Instance.GetPlayerControllerByIndex(inventoryController.playerIndex).WeaponController.UpdateAnimator();
    }

    public bool ItemTypeValidForSlot(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Consumable:
                if (slotType == Slot.Potion)
                    return true;
                break;
            case ItemType.Head:
                if (slotType == Slot.Potion)
                    return true;
                break;
            case ItemType.Body:
                if (slotType == Slot.Body)
                    return true;
                break;
            case ItemType.Legs:
                if (slotType == Slot.Legs)
                    return true;
                break;
            case ItemType.OneHanded:
                if (slotType == Slot.MainHand || slotType == Slot.OffHand)
                    return true;
                break;
            case ItemType.TwoHanded:
                if (slotType == Slot.MainHand)
                    return true;
                break;
            case ItemType.Offhand:
                if (slotType == Slot.OffHand)
                    return true;
                break;
        }
        return false;
    }

}

public enum Slot
{
    Potion,
    Head,
    Body,
    Legs,
    MainHand,
    OffHand,
    Trinket
}
