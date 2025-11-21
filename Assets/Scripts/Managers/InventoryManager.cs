using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{
    public static Action OnMenuOpened;
    public static Action OnMenuClosed;
    public GameObject EquipmentMenu;

    [Serializable]
    public class EquipmentMenus
    {
        public PlayerUserInterfaceController playerUserInterfaceController;
        public InventoryController controller;
        public GameObject EquipmentMenuObject;
        public int playerIndex;
    }
    public EquipmentMenus[] EquipmentMenuObjects = new EquipmentMenus[2];

    public EquipmentSlot[] equipmentSlot;
    public EquippedSlot[] equippedSlot;

    public bool IsInventoryOpened;
    public bool IsEquipmentMenuOpened;

    private bool player0MenuOpened;
    private bool player1MenuOpened;

    public void OpenInventory(int playerIndex)
    {
        if (EquipmentMenuObjects[playerIndex].playerUserInterfaceController.IsMenuOpened)
        {
            EquipmentMenuObjects[playerIndex].playerUserInterfaceController.CloseAllMenus();
            EquipmentMenuObjects[playerIndex].controller.OnMenuClosed?.Invoke();
            if (playerIndex == 0) player0MenuOpened = false;
            if (playerIndex == 1) player1MenuOpened = false;

            if (!player0MenuOpened && !player1MenuOpened)
            {
                EquipmentMenuObjects[playerIndex].controller.DeselectAllSlots();
                Time.timeScale = 1;
                NewPlayerController controller = PlayerJoinManager.Instance.GetPlayerControllerByIndex(1);
                if (controller != null)
                {
                    controller.WeaponController.canAttack = true;
                }
                OnMenuClosed?.Invoke();
            }
        }
        else
        {
            EquipmentMenuObjects[playerIndex].playerUserInterfaceController.OpenInventoryPanel();

            EquipmentMenuObjects[playerIndex].controller.OnMenuOpened?.Invoke();
            if (playerIndex == 0) player0MenuOpened = true;
            if (playerIndex == 1) player1MenuOpened = true;
            Time.timeScale = 0;
            OnMenuOpened?.Invoke();
        }
    }

    public void AddItemToCorrectPlayerInventory(ItemData itemData, int playerIndex)
    {
        EquipmentMenuObjects[playerIndex].controller.AddItemToFirstEmptySlot(itemData);
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            if (equipmentSlot[i].isSelected)
            {
                equipmentSlot[i].isSelected = false;
            }
        }

        for (int i = 0; i < equippedSlot.Length; i++)
        {
            if (equippedSlot[i].isSelected)
            {
                equippedSlot[i].isSelected = false;
            }
        }
    }

    public InventoryController GetInventoryControllerByIndex(int playerIndex)
    {
        return EquipmentMenuObjects[playerIndex].controller;
    }

    public PlayerUserInterfaceController GetPlayerUserInterfaceControllerByIndex(int playerIndex)
    {
        return EquipmentMenuObjects[playerIndex].playerUserInterfaceController;
    }
}

public enum ItemType
{
    Consumable,
    Head,
    Body,
    Legs,
    OneHanded,
    TwoHanded,
    Offhand,
    Bow
};
