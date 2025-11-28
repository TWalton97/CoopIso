using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour
{
    public PlayerContext PlayerContext;

    public Transform ItemButtonParent;
    public ItemButton ItemButtonPrefab;

    public ScrollRect scrollRect;

    private Dictionary<string, ItemButton> instantiatedItemButtons = new Dictionary<string, ItemButton>();

    private void OnEnable()
    {
        if (instantiatedItemButtons.Count != 0)
        {
            GameObject firstButton = ItemButtonParent.GetChild(0).gameObject;
            PlayerContext.UserInterfaceController.eventSystem.SetSelectedGameObject(firstButton);
            UpdateViewPosition(ItemButtonParent.GetChild(0).GetComponent<RectTransform>());
            firstButton.GetComponent<ItemButton>().HighlightIcon.SetActive(true);
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
        instantiatedItemButtons.Remove(id);
        Destroy(itemButton);
    }

    public ItemButton GetItemButtonByID(string id)
    {
        return instantiatedItemButtons.GetValueOrDefault(id);
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
