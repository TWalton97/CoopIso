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

    public void EquipGear(ItemData itemData, NewPlayerController playerController, int index = 0, bool deleteItemInSlot = false)
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
            playerController.WeaponController.EquipOneHandedWeapon(itemData.objectPrefab, itemData);
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
            playerController.WeaponController.EquipOffhand(itemData.objectPrefab, itemData);
            EquippedSlot mainHandSlot = inventoryController.FindEquippedSlotOfType(Slot.MainHand)[0];
            if (mainHandSlot.slotInUse)
            {
                if (mainHandSlot.itemData.itemType == ItemType.TwoHanded && !playerController.PlayerStatsBlackboard.TwoHandedMastery)
                {
                    mainHandSlot.UnequipGear();
                }
                else if (mainHandSlot.itemData.itemType == ItemType.Bow)
                {
                    mainHandSlot.UnequipGear();
                }
            }
            //NewWeaponController.Instance.EquipOffhand(weapon);
        }
        else if (itemData.itemType == ItemType.TwoHanded)
        {
            if (!inventoryController.controller.PlayerStatsBlackboard.TwoHandedMastery)
            {
                inventoryController.FindEquippedSlotOfType(Slot.OffHand)[0].UnequipGear();
                playerController.WeaponController.EquipTwoHandedWeapon(itemData.objectPrefab, itemData);
            }
            else
            {
                playerController.WeaponController.EquipOneHandedWeapon(itemData.objectPrefab, itemData);
            }
            //NewWeaponController.Instance.EquipTwoHandedWeapon(weapon);
        }
        else if (itemData.itemType == ItemType.Bow)
        {
            inventoryController.FindEquippedSlotOfType(Slot.OffHand)[0].UnequipGear();
            playerController.WeaponController.EquipTwoHandedWeapon(itemData.objectPrefab, itemData);
        }

        if (itemData.itemType == ItemType.Body)
        {
            playerController.ArmorController.EquipBodyArmor(itemData.objectPrefab, itemData);
        }
        else if (itemData.itemType == ItemType.Head)
        {
            playerController.ArmorController.EquipHelmet(itemData.objectPrefab, itemData);
        }
        else if (itemData.itemType == ItemType.Legs)
        {
            playerController.ArmorController.EquipLegs(itemData.objectPrefab, itemData);
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
        NewPlayerController playerController = PlayerJoinManager.Instance.GetPlayerControllerByIndex(inventoryController.playerIndex);
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
            case Slot.Body:
                playerController.ArmorController.UnequipBodyArmor();
                break;
            case Slot.Head:
                playerController.ArmorController.UnequipHelmet();
                break;
            case Slot.Legs:
                playerController.ArmorController.UnequipLegs();
                break;
        }
        HidePreview();
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
                if (slotType == Slot.MainHand || (slotType == Slot.OffHand && inventoryController.controller.PlayerStatsBlackboard.TwoHandedMastery))
                    return true;
                break;
            case ItemType.Offhand:
                if (slotType == Slot.OffHand)
                    return true;
                break;
            case ItemType.Bow:
                if (slotType == Slot.MainHand)
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
