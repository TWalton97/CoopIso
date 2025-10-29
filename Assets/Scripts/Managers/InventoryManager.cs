using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public GameObject InventoryMenu;
    public GameObject EquipmentMenu;

    public ItemSlot[] itemSlot;
    public EquipmentSlot[] equipmentSlot;
    public ItemSO[] itemSOs;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Inventory();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Equipment();
        }
    }

    void Inventory()
    {
        if (InventoryMenu.activeSelf)
        {
            InventoryMenu.SetActive(false);
            EquipmentMenu.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            InventoryMenu.SetActive(true);
            EquipmentMenu.SetActive(false);
            Time.timeScale = 0;
        }
    }

    void Equipment()
    {
        if (EquipmentMenu.activeSelf)
        {
            InventoryMenu.SetActive(false);
            EquipmentMenu.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            InventoryMenu.SetActive(false);
            EquipmentMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void AddItem(string itemName, int quantity, Sprite sprite, string itemDescription, GameObject objectPrefab, ItemType itemType)
    {
        if (itemType == ItemType.Consumable)
        {
            for (int i = 0; i < itemSlot.Length; i++)
            {
                if (itemSlot[i].isFull == false)
                {
                    itemSlot[i].AddItem(itemName, quantity, sprite, itemDescription, objectPrefab, itemType);
                    return;
                }
            }
        }
        else
        {
            for (int i = 0; i < equipmentSlot.Length; i++)
            {
                if (equipmentSlot[i].isFull == false)
                {
                    equipmentSlot[i].AddItem(itemName, quantity, sprite, itemDescription, objectPrefab, itemType);
                    return;
                }
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
    }
}

public enum ItemType
{
    Consumable,
    Head,
    Body,
    Legs,
    Mainhand,
    OffHand
};
