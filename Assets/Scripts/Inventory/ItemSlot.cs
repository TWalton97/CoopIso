using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ItemSlot : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    [SerializeField] protected Sprite emptySprite;
    [SerializeField] protected Image slotImage;
    public ItemData itemData;
    public GameObject selectedShader;
    public bool slotInUse;
    public bool isSelected;
    public int slotIndex;

    public InventoryController inventoryController;

    public GameObject previewObject;
    public PreviewWindow previewWindow;
    public Button selectable;

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

    public void OnSelect(BaseEventData eventData)
    {
        inventoryController.CurrentlySelectedItemSlot = this;
        DisplayPreview();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        HidePreview();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        selectable.Select();
    }

    private void DisplayPreview()
    {
        if (slotInUse && previewObject != null)
        {
            previewWindow.FillPreviewWindow(itemData);
            previewObject.SetActive(true);
        }

    }

    public void HidePreview()
    {
        if (previewObject != null)
            previewObject.SetActive(false);
    }
}
