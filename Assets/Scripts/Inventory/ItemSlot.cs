using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    //Item Data
    public string itemName;
    public int quantity;
    public Sprite sprite;
    public bool isFull;
    public string itemDescription;
    public GameObject objectPrefab;
    public Sprite emptySprite;
    public ItemType itemType;

    //Item Slot
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;

    //Item Description Slot
    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;

    public GameObject selectedShader;
    public bool isSelected;

    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = InventoryManager.Instance;
    }

    public void AddItem(string itemName, int quantity, Sprite sprite, string itemDescription, GameObject objectPrefab, ItemType itemType)
    {
        this.itemName = itemName;
        this.quantity = quantity;
        this.sprite = sprite;
        this.itemDescription = itemDescription;
        this.objectPrefab = objectPrefab;
        this.itemType = itemType;
        isFull = true;

        quantityText.text = quantity.ToString();
        quantityText.enabled = true;
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
        if (isSelected)
        {
            inventoryManager.UseItem(itemName);
            this.quantity -= 1;
            if (this.quantity <= 0)
                EmptySlot();
        }
        else
        {
            inventoryManager.DeselectAllSlots();
            selectedShader.SetActive(true);
            isSelected = true;
            itemDescriptionNameText.text = itemName;
            itemDescriptionText.text = itemDescription;
            itemDescriptionImage.sprite = sprite;
            if (itemDescriptionImage.sprite == null)
                itemDescriptionImage.sprite = emptySprite;
        }
    }

    private void EmptySlot()
    {
        quantityText.enabled = false;
        itemImage.sprite = emptySprite;

        itemDescriptionNameText.text = "";
        itemDescriptionText.text = "";
        itemDescriptionImage.sprite = emptySprite;
        isFull = false;
        isSelected = false;
    }

    public void OnRightClick()
    {
        //Create a new item
        GameObject itemToDrop = new GameObject(itemName);
        Item newItem = itemToDrop.AddComponent<Item>();
        Instantiate(objectPrefab, Vector3.zero, Quaternion.identity, itemToDrop.transform);
        newItem.quantity = 1;
        newItem.itemName = itemName;
        newItem.sprite = sprite;
        newItem.itemDescription = itemDescription;
        newItem.objectPrefab = objectPrefab;

        //Add collider
        itemToDrop.AddComponent<SphereCollider>().isTrigger = true;

        GameObject player = GameObject.FindWithTag("Player");
        itemToDrop.transform.position = player.transform.position + (player.transform.forward * 2f);

        this.quantity -= 1;
        quantityText.text = this.quantity.ToString();
        if (this.quantity <= 0)
            EmptySlot();
    }
}
