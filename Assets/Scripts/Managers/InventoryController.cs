using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public PlayerUserInterfaceController playerUserInterfaceController;
    public int playerIndex;

    public EquipmentSlot[] equipmentSlot;
    public EquippedSlot[] equippedSlot;

    [SerializeField] private TMP_Text PlayerClassText;
    [SerializeField] private TMP_Text PlayerHealthText;
    [SerializeField] private TMP_Text PlayerMovementSpeedText;
    [SerializeField] private TMP_Text PlayerAttacksPerSecondText;
    [SerializeField] private TMP_Text PlayerManaText;
    [SerializeField] private TMP_Text PlayerArmorText;

    public List<ItemSlot> selectedItemSlots = new();
    public ItemSlot CurrentlySelectedItemSlot;

    public NewPlayerController controller;
    public Action OnMenuOpened;
    public Action OnMenuClosed;
    private void Awake()
    {
        SetEquipmentSlotIndexes();
    }

    void OnEnable()
    {
        controller.WeaponController.OnWeaponUpdated += UpdatePlayerStats;
        controller.PlayerHealthController.OnMaximumHealthChanged += UpdatePlayerStats;
        controller.PlayerHealthController.OnArmorAmountChanged += UpdatePlayerStats;
        OnMenuOpened += controller.PlayerStatsBlackboard.UpdateArmorStats;
        OnMenuOpened += UpdatePlayerStats;
        UpdatePlayerStats();
    }

    private void OnDisable()
    {
        controller.WeaponController.OnWeaponUpdated -= UpdatePlayerStats;
        controller.PlayerHealthController.OnMaximumHealthChanged -= UpdatePlayerStats;
        controller.PlayerHealthController.OnArmorAmountChanged -= UpdatePlayerStats;
        OnMenuOpened -= controller.PlayerStatsBlackboard.UpdateArmorStats;
        OnMenuOpened -= UpdatePlayerStats;
        ResetButtonSelection();
        if (CurrentlySelectedItemSlot != null)
            CurrentlySelectedItemSlot.HidePreview();
    }

    public void UpdatePlayerStats()
    {
        PlayerClassText.text = controller.PlayerStatsBlackboard.ClassName;
        PlayerHealthText.text = controller.healthController.CurrentHealth.ToString() + "/" + controller.healthController.MaximumHealth.ToString();
        PlayerMovementSpeedText.text = controller._maximumMovementSpeed.ToString();
        PlayerAttacksPerSecondText.text = controller.PlayerStatsBlackboard.AttacksPerSecond.ToString("0.00");
        PlayerManaText.text = controller.PlayerStatsBlackboard.ResourceCurrent.ToString("00") + "/" + controller.PlayerStatsBlackboard.ResourceMax.ToString("00");
        PlayerArmorText.text = controller.PlayerStatsBlackboard.ArmorAmount.ToString();
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
        foreach (ItemSlot itemSlot in selectedItemSlots)
        {
            itemSlot.DeselectButton();
        }
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
                            mainSlot.EquipGear(itemData, playerUserInterfaceController.playerContext.PlayerController);
                        }
                        else if (mainSlot.equippedWeaponType == ItemType.TwoHanded)
                        {
                            if (selectedItemSlots[0].itemData.itemType == ItemType.OneHanded)
                            {
                                ItemData itemData = selectedItemSlots[0].itemData;
                                selectedItemSlots[0].EmptySlot();
                                equippedSlot.UnequipGear();
                                mainSlot.EquipGear(itemData, playerUserInterfaceController.playerContext.PlayerController);
                            }
                            else if (selectedItemSlots[0].itemData.itemType == ItemType.TwoHanded)
                            {
                                ItemData itemData = selectedItemSlots[0].itemData;
                                selectedItemSlots[0].EmptySlot();
                                equippedSlot.EquipGear(itemData, playerUserInterfaceController.playerContext.PlayerController);
                            }
                            else if (selectedItemSlots[0].itemData.itemType == ItemType.Bow)
                            {
                                ItemData itemData = selectedItemSlots[0].itemData;
                                selectedItemSlots[0].EmptySlot();
                                mainSlot.EquipGear(itemData, playerUserInterfaceController.playerContext.PlayerController);
                            }
                        }
                        else if (mainSlot.equippedWeaponType == ItemType.OneHanded)
                        {
                            if (selectedItemSlots[0].itemData.itemType == ItemType.TwoHanded)
                            {
                                ItemData itemData = selectedItemSlots[0].itemData;
                                selectedItemSlots[0].EmptySlot();
                                equippedSlot.UnequipGear();
                                mainSlot.EquipGear(itemData, playerUserInterfaceController.playerContext.PlayerController);
                            }
                            else if (selectedItemSlots[0].itemData.itemType == ItemType.OneHanded)
                            {
                                ItemData itemData = selectedItemSlots[0].itemData;
                                selectedItemSlots[0].EmptySlot();
                                equippedSlot.EquipGear(itemData, playerUserInterfaceController.playerContext.PlayerController);
                            }
                            else if (selectedItemSlots[0].itemData.itemType == ItemType.Bow)
                            {
                                ItemData itemData = selectedItemSlots[0].itemData;
                                selectedItemSlots[0].EmptySlot();
                                equippedSlot.EquipGear(itemData, playerUserInterfaceController.playerContext.PlayerController);
                            }
                        }
                        else if (mainSlot.equippedWeaponType == ItemType.Bow)
                        {
                            ItemData itemData = selectedItemSlots[0].itemData;
                            selectedItemSlots[0].EmptySlot();
                            equippedSlot.UnequipGear();
                            mainSlot.EquipGear(itemData, playerUserInterfaceController.playerContext.PlayerController);
                        }
                        else
                        {
                            ItemData itemData = selectedItemSlots[0].itemData;
                            selectedItemSlots[0].EmptySlot();
                            equippedSlot.EquipGear(itemData, playerUserInterfaceController.playerContext.PlayerController, selectedItemSlots[0].slotIndex);
                        }
                    }
                    else if (equippedSlot.slotType == Slot.MainHand)
                    {
                        EquippedSlot offhandSlot = FindEquippedSlotOfType(Slot.OffHand)[0];
                        ItemData itemData = selectedItemSlots[0].itemData;
                        selectedItemSlots[0].EmptySlot();
                        equippedSlot.EquipGear(itemData, playerUserInterfaceController.playerContext.PlayerController);
                        if (offhandSlot.slotInUse)
                        {
                            if (itemData.itemType == ItemType.OneHanded && offhandSlot.equippedWeaponType == ItemType.TwoHanded)
                            {
                                offhandSlot.UnequipGear();
                            }
                            else if (itemData.itemType == ItemType.TwoHanded && offhandSlot.equippedWeaponType == ItemType.OneHanded)
                            {
                                offhandSlot.UnequipGear();
                            }
                            else if (itemData.itemType == ItemType.Bow)
                            {
                                offhandSlot.UnequipGear();
                            }
                        }
                    }
                    else
                    {
                        ItemData itemData = selectedItemSlots[0].itemData;
                        selectedItemSlots[0].EmptySlot();
                        equippedSlot.EquipGear(itemData, playerUserInterfaceController.playerContext.PlayerController, selectedItemSlots[0].slotIndex);
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
                        equippedSlot.EquipGear(itemData, playerUserInterfaceController.playerContext.PlayerController, selectedItemSlots[1].slotIndex);
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
                        equippedSlot2.EquipGear(equippedSlot1.itemData, playerUserInterfaceController.playerContext.PlayerController);
                        equippedSlot1.EquipGear(itemData, playerUserInterfaceController.playerContext.PlayerController, 0, true);
                    }
                }
                else
                {
                    if (equippedSlot2.ItemTypeValidForSlot(equippedSlot1.itemData.itemType))
                    {
                        equippedSlot1.UnequipGear(0, true);
                        equippedSlot2.EquipGear(equippedSlot1.itemData, playerUserInterfaceController.playerContext.PlayerController);
                    }
                }
            }
            DeselectAllSlots();
            selectedItemSlots.Clear();
        }
    }
}
