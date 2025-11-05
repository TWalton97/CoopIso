using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public int playerIndex;

    public EquipmentSlot[] equipmentSlot;
    public EquippedSlot[] equippedSlot;

    [SerializeField] private TMP_Text PlayerHealthText;
    [SerializeField] private TMP_Text PlayerMovementSpeedText;
    [SerializeField] private TMP_Text PlayerAttacksPerSecondText;
    [SerializeField] private TMP_Text PlayerManaText;

    public List<ItemSlot> selectedItemSlots = new();
    public ItemSlot CurrentlySelectedItemSlot;

    private NewPlayerController controller;
    public Action OnMenuOpened;
    public Action OnMenuClosed;
    private void Start()
    {
        //Check what items the player has equipped into main and offhand
        //Register them
        //Add them to their equipped slots
        
    }

    void OnEnable()
    {
        controller = PlayerJoinManager.Instance.GetPlayerControllerByIndex(playerIndex);
        controller.WeaponController.OnWeaponUpdated += UpdatePlayerStats;
        OnMenuOpened += UpdatePlayerStats;
        SetEquipmentSlotIndexes();
    }

    private void OnDisable()
    {
        controller.WeaponController.OnWeaponUpdated -= UpdatePlayerStats;
        OnMenuOpened -= UpdatePlayerStats;
        ResetButtonSelection();
    }

    private void UpdatePlayerStats()
    {
        PlayerHealthText.text = controller.healthController.CurrentHealth.ToString() + "/" + controller.healthController.MaximumHealth.ToString();
        PlayerMovementSpeedText.text = controller._maximumMovementSpeed.ToString();
        PlayerAttacksPerSecondText.text = controller.PlayerStatsBlackboard.AttacksPerSecond.ToString("0.00");
        PlayerManaText.text = controller.PlayerStatsBlackboard.ResourceCurrent.ToString() + "/" + controller.PlayerStatsBlackboard.ResourceMax.ToString();
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

    public void ResetButtonSelection()
    {
        selectedItemSlots.Clear();
    }

    public void RegisterButtonSelection(ItemSlot itemSlot)
    {
        if (selectedItemSlots.Contains(itemSlot))
        {
            ResetButtonSelection();
            return;
        }
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
                EquippedSlot equippedSlot = selectedItemSlots[1] as EquippedSlot;
                if (equippedSlot.ItemTypeValidForSlot(selectedItemSlots[0].itemData.itemType))
                {
                    if (equippedSlot.slotType == Slot.OffHand)
                    {
                        EquippedSlot mainSlot = FindEquippedSlotOfType(Slot.MainHand)[0];
                        if (!mainSlot.slotInUse && mainSlot.ItemTypeValidForSlot(selectedItemSlots[0].itemData.itemType))
                        {
                            ItemData itemData = selectedItemSlots[0].itemData;
                            selectedItemSlots[0].EmptySlot();
                            mainSlot.EquipGear(itemData);
                        }
                        else
                        {
                            ItemData itemData = selectedItemSlots[0].itemData;
                            selectedItemSlots[0].EmptySlot();
                            equippedSlot.EquipGear(itemData, selectedItemSlots[0].slotIndex);
                        }
                    }
                    else
                    {
                        ItemData itemData = selectedItemSlots[0].itemData;
                        selectedItemSlots[0].EmptySlot();
                        equippedSlot.EquipGear(itemData, selectedItemSlots[0].slotIndex);
                    }
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

                if (equippedSlot2.slotInUse)
                {
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
                    if (equippedSlot2.ItemTypeValidForSlot(equippedSlot1.itemData.itemType))
                    {
                        equippedSlot2.EquipGear(equippedSlot1.itemData);
                        equippedSlot1.UnequipGear(0, true);
                    }
                }
            }
            DeselectAllSlots();
            selectedItemSlots.Clear();
        }
    }
}
