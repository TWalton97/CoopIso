using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class InventoryItemController : MonoBehaviour
{
    public PlayerContext PlayerContext;

    public Transform ItemButtonParent;
    public ItemButton ItemButtonPrefab;

    public ScrollRect scrollRect;

    private Dictionary<string, ItemButton> instantiatedItemButtons = new Dictionary<string, ItemButton>();

    public List<ControlData> ControlData;

    private void OnEnable()
    {
        if (instantiatedItemButtons.Count != 0)
        {
            SelectFirstButton();
        }

        PlayerContext.UserInterfaceController.inventoryController.UpdateControlPanel(ControlData);
    }

    void OnDisable()
    {
        DeselectAllButtons();
    }

    public void SelectFirstButton()
    {
        if (ItemButtonParent.childCount == 0) return;

        GameObject firstButton = ItemButtonParent.GetChild(0).gameObject;
        PlayerContext.UserInterfaceController.eventSystem.SetSelectedGameObject(firstButton);
        UpdateViewPosition(ItemButtonParent.GetChild(0).GetComponent<RectTransform>());
        firstButton.GetComponent<ItemButton>().HighlightIcon.SetActive(true);
    }

    public void DeselectAllButtons()
    {
        for (int i = 0; i < ItemButtonParent.childCount; i++)
        {
            ItemButtonParent.GetChild(i).GetComponent<EquippableButton>().ToggleHighlight(false);
        }
    }

    public void CreateButtonForItem(ItemData itemData, bool isEquipped = false)
    {
        ItemButton itemButton = Instantiate(ItemButtonPrefab, ItemButtonParent);

        itemButton.InitializeItemButton(this, itemData, itemData.itemID, PlayerContext, isEquipped);
        instantiatedItemButtons.Add(itemData.itemID, itemButton);
    }

    public void RemoveButtonAtID(string id)
    {
        ItemButton itemButton = GetItemButtonByID(id);
        if (itemButton == null) return;

        instantiatedItemButtons.Remove(id);
        Debug.Log(instantiatedItemButtons.Count);
        Destroy(itemButton.gameObject);
        SelectFirstButton();
    }

    public ItemButton GetItemButtonByID(string id)
    {
        if (instantiatedItemButtons.TryGetValue(id, out ItemButton itemButton))
        {
            return itemButton;
        }
        return null;
    }

    public void UpdateViewPosition(RectTransform target)
    {
        if (target == null) return;

        if (PlayerContext.PlayerController.PlayerInputController.playerInput.currentControlScheme == PlayerContext.PlayerController.KEYBOARD_SCHEME) return;
        scrollRect.content.localPosition = GetSnapToPositionToBringChildIntoView(scrollRect, target);
    }

    public Vector2 GetSnapToPositionToBringChildIntoView(ScrollRect instance, RectTransform child)
    {
        Canvas.ForceUpdateCanvases();
        Vector2 viewportLocalPosition = instance.viewport.localPosition;
        Vector2 childLocalPosition = child.localPosition;
        Vector2 result = new Vector2(
            0 - (viewportLocalPosition.x + childLocalPosition.x),
            0 - (viewportLocalPosition.y + childLocalPosition.y)
        );
        return result;
    }
}
