using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class EquipmentSlot : MonoBehaviour, IPointerClickHandler
{
    //Item Data
    public string itemName;
    public int quantity;
    public Sprite sprite;
    public bool isFull;
    public string itemDescription;
    public GameObject objectPrefab;
    public GameObject vfxPrefab;
    public Sprite emptySprite;
    public ItemType itemType;
    private WeaponDataSO data;

    //Item Slot
    [SerializeField] private Image itemImage;

    //Equipped Slots
    [SerializeField] private EquippedSlot headSlot, bodySlot, legSlot, mainHandSlot, offHandSlot;

    public GameObject selectedShader;
    public bool isSelected;

    private InventoryController inventoryController;

    void Start()
    {
        inventoryController = GetComponentInParent<InventoryController>();
    }

    public void AddItem(string itemName, int quantity, Sprite sprite, string itemDescription, GameObject objectPrefab, GameObject vfxPrefab, ItemType itemType, WeaponDataSO weaponDataSO)
    {
        this.itemName = itemName;
        this.quantity = quantity;
        this.sprite = sprite;
        this.itemDescription = itemDescription;
        this.objectPrefab = objectPrefab;
        this.vfxPrefab = vfxPrefab;
        this.itemType = itemType;
        this.data = weaponDataSO;
        isFull = true;

        itemImage.sprite = sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        if (!isFull) return;

        if (isSelected)
        {
            EquipGear();
            inventoryController.ClearPreviewWindow();
            isSelected = false;
        }
        else
        {
            inventoryController.DeselectAllSlots();
            isSelected = true;
            if (isFull)
                inventoryController.UpdatePreviewWindow(sprite, itemName, itemType, data);
        }
    }

    private void EquipGear()
    {
        if (itemType == ItemType.Head)
            headSlot.EquipGear(sprite, itemName, itemDescription, objectPrefab, vfxPrefab, data);
        if (itemType == ItemType.Body)
            bodySlot.EquipGear(sprite, itemName, itemDescription, objectPrefab, vfxPrefab, data);
        if (itemType == ItemType.Legs)
            legSlot.EquipGear(sprite, itemName, itemDescription, objectPrefab, vfxPrefab, data);
        if (itemType == ItemType.TwoHanded)
        {
            mainHandSlot.EquipGear(sprite, itemName, itemDescription, objectPrefab, vfxPrefab, data, ItemType.TwoHanded);
            offHandSlot.UnequipGear();
        }

        if (itemType == ItemType.OneHanded)
        {
            if (!mainHandSlot.slotInUse)
            {
                mainHandSlot.EquipGear(sprite, itemName, itemDescription, objectPrefab, vfxPrefab, data, ItemType.OneHanded);
            }
            else if (mainHandSlot.equippedWeaponType == ItemType.TwoHanded)
            {
                mainHandSlot.EquipGear(sprite, itemName, itemDescription, objectPrefab, vfxPrefab, data, ItemType.OneHanded);
            }
            else if (!offHandSlot.slotInUse)
            {
                offHandSlot.EquipGear(sprite, itemName, itemDescription, objectPrefab, vfxPrefab, data, ItemType.OneHanded);
            }
            else
            {
                mainHandSlot.EquipGear(sprite, itemName, itemDescription, objectPrefab, vfxPrefab, data, ItemType.OneHanded);
            }
        }
        if (itemType == ItemType.Offhand)
        {
            offHandSlot.EquipGear(sprite, itemName, itemDescription, objectPrefab, vfxPrefab, data, ItemType.Offhand);
            if (mainHandSlot.equippedWeaponType == ItemType.TwoHanded)
            {
                mainHandSlot.UnequipGear();
            }
        }
        EmptySlot();
    }

    private void EmptySlot()
    {
        itemImage.sprite = emptySprite;

        isFull = false;
        isSelected = false;
    }

    public void OnRightClick()
    {
        //Create a new item
        GameObject itemToDrop = new GameObject(itemName);
        Item newItem = itemToDrop.AddComponent<Item>();
        newItem.itemType = this.itemType;
        Instantiate(objectPrefab, Vector3.zero, Quaternion.identity, itemToDrop.transform);
        newItem.quantity = 1;
        newItem.itemName = itemName;
        newItem.sprite = sprite;
        newItem.itemDescription = itemDescription;
        newItem.objectPrefab = objectPrefab;
        newItem.vfxPrefab = vfxPrefab;

        //Add collider
        itemToDrop.AddComponent<SphereCollider>().isTrigger = true;

        GameObject player = GameObject.FindWithTag("Player");
        itemToDrop.transform.position = player.transform.position + (player.transform.forward * 2f);

        this.quantity -= 1;
        if (this.quantity <= 0)
            EmptySlot();
    }
}
