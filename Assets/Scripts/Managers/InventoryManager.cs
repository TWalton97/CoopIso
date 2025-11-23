using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static Action OnMenuOpened;
    public static Action OnMenuClosed;
    public GameObject PlayerPreview;

    [Serializable]
    public class EquipmentMenus
    {
        public PlayerUserInterfaceController playerUserInterfaceController;
        public InventoryController controller;
        public GameObject EquipmentMenuObject;
        public int playerIndex;
    }
    public EquipmentMenus[] EquipmentMenuObjects = new EquipmentMenus[2];

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
                foreach (EquipmentMenus equipmentMenus in EquipmentMenuObjects)
                {
                    if (equipmentMenus.controller != null)
                        equipmentMenus.playerUserInterfaceController.DisplayPlayerResourcePanel(true);
                }
                if (controller != null)
                {
                    controller.WeaponController.canAttack = true;
                }
                PlayerPreview.SetActive(false);
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
            foreach (EquipmentMenus equipmentMenus in EquipmentMenuObjects)
            {
                if (equipmentMenus.controller != null)
                    equipmentMenus.playerUserInterfaceController.DisplayPlayerResourcePanel(false);
            }
            PlayerPreview.SetActive(true);
            OnMenuOpened?.Invoke();
        }
    }

    public void AddItemToCorrectPlayerInventory(ItemData itemData, int playerIndex)
    {
        EquipmentMenuObjects[playerIndex].controller.AddItemToFirstEmptySlot(itemData);
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
