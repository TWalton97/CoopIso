using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    //This is the controller for an individual isntance of an inventory
    //It stores references to all item slots
    //

    public int playerIndex;

    public EquipmentSlot[] equipmentSlot;
    public EquippedSlot[] equippedSlot;

    [SerializeField] private Image previewImage;
    [SerializeField] private TMP_Text itemNamePreText;
    [SerializeField] private TMP_Text itemTypePreText;
    [SerializeField] private TMP_Text attackPreText;
    [SerializeField] private TMP_Text movementSpeedPreText;

    public List<ItemSlot> selectedItemSlots = new();

    private void Start()
    {
        SetEquipmentSlotIndexes();
    }

    private void OnDisable()
    {
        ResetButtonSelection();
    }

    private void SetEquipmentSlotIndexes()
    {
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            equipmentSlot[i].slotIndex = i;
        }
    }

    public List<EquippedSlot> FindEquippedSlotOfType(Slot slotType)
    {
        List<EquippedSlot> equippedSlots = new List<EquippedSlot>();
        foreach (EquippedSlot slot in equippedSlot)
        {
            if (slot.slotType == slotType)
            {
                equippedSlots.Add(slot);
            }

        }

        return equippedSlots;
    }

    public void AddItemToFirstEmptySlot(ItemData itemData)
    {
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            if (equipmentSlot[i].slotInUse == false)
            {
                equipmentSlot[i].ImportItemDataToEquipmentSlot(itemData);
                return;
            }
        }
    }

    public void AddItemToSelectedSlot(ItemData itemData, int index)
    {
        if (index >= equipmentSlot.Length || equipmentSlot[index].slotInUse)
        {
            AddItemToFirstEmptySlot(itemData);
            return;
        }
        equipmentSlot[index].ImportItemDataToEquipmentSlot(itemData);
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            if (equipmentSlot[i].isSelected)
            {
                equipmentSlot[i].DeselectButton();
            }
        }

        for (int i = 0; i < equippedSlot.Length; i++)
        {
            if (equippedSlot[i].isSelected)
            {
                equippedSlot[i].DeselectButton();
            }
        }

        ResetButtonSelection();
    }

    public void UpdatePreviewWindow(Sprite sprite, string itemName, ItemType itemType, WeaponDataSO weaponDataSO)
    {
        previewImage.sprite = sprite;
        itemNamePreText.text = itemName;
        itemTypePreText.text = itemType.ToString();
        attackPreText.text = weaponDataSO.WeaponDamage.ToString();
        movementSpeedPreText.text = weaponDataSO.MovementSpeedDuringAttack.ToString();
    }

    public void ClearPreviewWindow()
    {
        previewImage.sprite = null;
        itemNamePreText.text = "";
        itemTypePreText.text = "";
        attackPreText.text = "";
        movementSpeedPreText.text = "";
    }

    public void ResetButtonSelection()
    {
        selectedItemSlots.Clear();
    }

    public void RegisterButtonSelection(ItemSlot itemSlot)
    {
        selectedItemSlots.Add(itemSlot);
        if (selectedItemSlots.Count == 2)
        {
            if (selectedItemSlots[0] is EquipmentSlot && selectedItemSlots[1] is EquipmentSlot)
            {
                if (selectedItemSlots[1].slotInUse)
                {
                    ItemData itemData = selectedItemSlots[1].itemData;
                    selectedItemSlots[1].EmptySlot();
                    AddItemToSelectedSlot(selectedItemSlots[0].itemData, selectedItemSlots[1].slotIndex);
                    selectedItemSlots[0].EmptySlot();
                    AddItemToSelectedSlot(itemData, selectedItemSlots[0].slotIndex);
                }
                else
                {
                    AddItemToSelectedSlot(selectedItemSlots[0].itemData, selectedItemSlots[1].slotIndex);
                    selectedItemSlots[0].EmptySlot();
                }
            }
            else if (selectedItemSlots[0] is EquipmentSlot && selectedItemSlots[1] is EquippedSlot)
            {
                //If I equip to a full slot, the previously equipped item gets deleted
                EquippedSlot equippedSlot = selectedItemSlots[1] as EquippedSlot;
                if (equippedSlot.ItemTypeValidForSlot(selectedItemSlots[0].itemData.itemType))
                {
                    ItemData itemData = selectedItemSlots[0].itemData;
                    selectedItemSlots[0].EmptySlot();
                    equippedSlot.EquipGear(itemData, selectedItemSlots[0].slotIndex);
                }
            }
            else if (selectedItemSlots[0] is EquippedSlot && selectedItemSlots[1] is EquipmentSlot)
            {
                EquippedSlot equippedSlot = selectedItemSlots[0] as EquippedSlot;
                if (!selectedItemSlots[1].slotInUse)
                {
                    equippedSlot.UnequipGear(selectedItemSlots[1].slotIndex);
                }
                else
                {
                    if (equippedSlot.ItemTypeValidForSlot(selectedItemSlots[1].itemData.itemType))
                    {
                        ItemData itemData = selectedItemSlots[1].itemData;
                        selectedItemSlots[1].EmptySlot();
                        equippedSlot.EquipGear(itemData, selectedItemSlots[1].slotIndex);
                    }
                }
            }
            else if (selectedItemSlots[0] is EquippedSlot && selectedItemSlots[1] is EquippedSlot)
            {
                EquippedSlot equippedSlot1 = selectedItemSlots[0] as EquippedSlot;
                EquippedSlot equippedSlot2 = selectedItemSlots[1] as EquippedSlot;

                //If the 2nd slot isn't in use, we just check if the item in our slot can go into that slot
                if (equippedSlot2.slotInUse)
                {
                    //We need to check that both weapons are valid for each other's slots
                    if (equippedSlot2.ItemTypeValidForSlot(equippedSlot1.itemData.itemType) && equippedSlot1.ItemTypeValidForSlot(equippedSlot2.itemData.itemType))
                    {
                        ItemData itemData = equippedSlot2.itemData;
                        equippedSlot2.UnequipGear(0, true);
                        equippedSlot2.EquipGear(equippedSlot1.itemData);
                        equippedSlot1.EquipGear(itemData, 0, true);
                    }
                }
                else
                {
                    //If item is valid
                    if (equippedSlot2.ItemTypeValidForSlot(equippedSlot1.itemData.itemType))
                    {
                        equippedSlot2.EquipGear(equippedSlot1.itemData);
                        equippedSlot1.UnequipGear(0, true);
                    }
                }

            }


            DeselectAllSlots();
            ClearPreviewWindow();
            selectedItemSlots.Clear();
        }
    }
}
