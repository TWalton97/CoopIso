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
        public InventoryController controller;
        public GameObject EquipmentMenuObject;
        public int playerIndex;
    }
    public EquipmentMenus[] EquipmentMenuObjects = new EquipmentMenus[2];

    public EquipmentSlot[] equipmentSlot;
    public EquippedSlot[] equippedSlot;
    public ItemSO[] itemSOs;

    [SerializeField] private Image previewImage;
    [SerializeField] private TMP_Text itemNamePreText;
    [SerializeField] private TMP_Text itemTypePreText;
    [SerializeField] private TMP_Text attackPreText;
    [SerializeField] private TMP_Text movementSpeedPreText;

    public bool IsInventoryOpened;
    public bool IsEquipmentMenuOpened;

    private bool player0MenuOpened;
    private bool player1MenuOpened;

    public void Equipment(int playerIndex)
    {
        if (EquipmentMenuObjects[playerIndex].EquipmentMenuObject.activeSelf)   //If the corresponding menu is opened, close it
        {
            EquipmentMenuObjects[playerIndex].EquipmentMenuObject.SetActive(false);
            if (playerIndex == 0) player0MenuOpened = false;
            if (playerIndex == 1) player1MenuOpened = false;

            if (!player0MenuOpened && !player1MenuOpened)
            {
                EquipmentMenuObjects[playerIndex].controller.DeselectAllSlots();
                Time.timeScale = 1;
                //TODO: STILL FIX THIS
                //PlayerJoinManager.Instance.GetPlayerControllerByIndex(0).WeaponController.canAttack = true;
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
            EquipmentMenuObjects[playerIndex].EquipmentMenuObject.SetActive(true);
            EquipmentMenuObjects[playerIndex].controller.ClearPreviewWindow();
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

    public void UseItem(string itemName)
    {
        for (int i = 0; i < itemSOs.Length; i++)
        {
            if (itemSOs[i].name == itemName)
            {
                itemSOs[i].UseItem();
            }
        }
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            if (equipmentSlot[i].isSelected)
            {
                //equipmentSlot[i].selectedShader.SetActive(false);
                equipmentSlot[i].isSelected = false;
            }
        }

        for (int i = 0; i < equippedSlot.Length; i++)
        {
            if (equippedSlot[i].isSelected)
            {
                //equippedSlot[i].selectedShader.SetActive(false);
                equippedSlot[i].isSelected = false;
            }
        }
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
}

public enum ItemType
{
    Consumable,
    Head,
    Body,
    Legs,
    OneHanded,
    TwoHanded,
    Offhand
};
