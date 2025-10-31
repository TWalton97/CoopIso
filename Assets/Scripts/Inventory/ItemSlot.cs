using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected Sprite emptySprite;
    [SerializeField] protected Image slotImage;
    public ItemData itemData;
    public GameObject selectedShader;
    public bool slotInUse;
    public bool isSelected;
    public int slotIndex;

    protected InventoryController inventoryController;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public virtual void OnLeftClick()
    {

    }

    public virtual void OnRightClick()
    {

    }

    public virtual void EmptySlot()
    {
        slotImage.sprite = emptySprite;

        slotInUse = false;
        isSelected = false;
    }

    public void DeselectButton()
    {
        isSelected = false;
        selectedShader.SetActive(false);
    }
}
