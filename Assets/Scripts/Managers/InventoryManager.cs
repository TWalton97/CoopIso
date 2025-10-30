using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>, PlayerInputActions.IPlayerActions
{
    public static Action OnMenuOpened;
    public static Action OnMenuClosed;
    public GameObject InventoryMenu;
    public GameObject EquipmentMenu;

    public ItemSlot[] itemSlot;
    public EquipmentSlot[] equipmentSlot;
    public EquippedSlot[] equippedSlot;
    public ItemSO[] itemSOs;

    [SerializeField] private Image previewImage;
    [SerializeField] private TMP_Text itemNamePreText;
    [SerializeField] private TMP_Text itemTypePreText;
    [SerializeField] private TMP_Text attackPreText;
    [SerializeField] private TMP_Text movementSpeedPreText;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Inventory();
        }

        // if (Input.GetKeyDown(KeyCode.Tab))
        // {
        //     Equipment();
        // }
    }

    void Inventory()
    {
        if (InventoryMenu.activeSelf)
        {
            InventoryMenu.SetActive(false);
            EquipmentMenu.SetActive(false);
            OnMenuClosed?.Invoke();
            NewWeaponController.Instance.canAttack = true;
            Time.timeScale = 1;
        }
        else
        {
            InventoryMenu.SetActive(true);
            EquipmentMenu.SetActive(false);
            OnMenuOpened?.Invoke();
            Time.timeScale = 0;
        }
    }

    public void Equipment()
    {
        if (EquipmentMenu.activeSelf)
        {
            InventoryMenu.SetActive(false);
            EquipmentMenu.SetActive(false);
            OnMenuClosed?.Invoke();
            NewWeaponController.Instance.canAttack = true;
            Time.timeScale = 1;
        }
        else
        {
            InventoryMenu.SetActive(false);
            EquipmentMenu.SetActive(true);
            OnMenuOpened?.Invoke();
            Time.timeScale = 0;
        }
    }

    public void AddItem(string itemName, int quantity, Sprite sprite, string itemDescription, GameObject objectPrefab, ItemType itemType, WeaponDataSO weaponDataSO)
    {

        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            if (equipmentSlot[i].isFull == false)
            {
                equipmentSlot[i].AddItem(itemName, quantity, sprite, itemDescription, objectPrefab, itemType, weaponDataSO);
                return;
            }
        }
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
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].isSelected)
            {
                itemSlot[i].selectedShader.SetActive(false);
                itemSlot[i].isSelected = false;
            }
        }

        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            if (equipmentSlot[i].isSelected)
            {
                equipmentSlot[i].selectedShader.SetActive(false);
                equipmentSlot[i].isSelected = false;
            }
        }

        for (int i = 0; i < equippedSlot.Length; i++)
        {
            if (equippedSlot[i].isSelected)
            {
                equippedSlot[i].selectedShader.SetActive(false);
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

    public void OnMove(InputAction.CallbackContext context)
    {

    }

    public void OnLookMouse(InputAction.CallbackContext context)
    {

    }

    public void OnLookStick(InputAction.CallbackContext context)
    {

    }

    public void OnAttack(InputAction.CallbackContext context)
    {

    }

    public void OnJump(InputAction.CallbackContext context)
    {

    }

    public void OnAbility1(InputAction.CallbackContext context)
    {

    }

    public void OnBlock(InputAction.CallbackContext context)
    {

    }

    public void OnSwapWeapon(InputAction.CallbackContext context)
    {

    }

    public void OnOpenEquipmentMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Equipment();
        }
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
