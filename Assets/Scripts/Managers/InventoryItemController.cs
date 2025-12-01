using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class InventoryItemController : MonoBehaviour
{
    public Button LeftTab;
    public Button RightTab;

    public PlayerContext PlayerContext;

    public Transform ItemButtonParent;
    public ItemButton ItemButtonPrefab;

    public ScrollRect scrollRect;

    private Dictionary<string, ItemButton> instantiatedItemButtons = new Dictionary<string, ItemButton>();

    public List<ControlData> ControlData;

    public ControlData EquipOffhandControlData; //We add this if the weapon can be equipped offhand

    private void OnEnable()
    {
        if (instantiatedItemButtons.Count != 0)
        {
            MoveEquippedButtonsToTop();
            SelectButton();
        }

        Navigation rightTabNavigation = new Navigation();
        rightTabNavigation.mode = Navigation.Mode.Explicit;
        rightTabNavigation.selectOnLeft = LeftTab;
        if (ItemButtonParent.childCount != 0)
        {
            rightTabNavigation.selectOnDown = ItemButtonParent.GetChild(0).GetComponent<Button>();
        }
        RightTab.navigation = rightTabNavigation;

        Navigation leftTabNavigation = new Navigation();
        leftTabNavigation.mode = Navigation.Mode.Explicit;
        leftTabNavigation.selectOnRight = RightTab;
        if (ItemButtonParent.childCount != 0)
        {
            leftTabNavigation.selectOnDown = ItemButtonParent.GetChild(0).GetComponent<Button>();
        }
        LeftTab.navigation = leftTabNavigation;
    }

    public void UpdateControlsPanel(ItemButton button)
    {
        var controls = new List<ControlData>(ControlData);
        if (button is EquippableButton equippableButton && equippableButton.CanEquipOffhand())
        {
            controls.Add(EquipOffhandControlData);
        }
        PlayerContext.UserInterfaceController.inventoryController.UpdateControlPanel(controls);
    }

    void OnDisable()
    {
        DeselectAllButtons();
    }

    public void SelectButton(int index = 0)
    {
        if (ItemButtonParent.childCount == 0) return;
        int clampedIndex = Mathf.Clamp(index, 0, ItemButtonParent.childCount - 1);
        GameObject firstButton = ItemButtonParent.GetChild(clampedIndex).gameObject;
        PlayerContext.UserInterfaceController.eventSystem.SetSelectedGameObject(firstButton);
        UpdateViewPosition(ItemButtonParent.GetChild(clampedIndex).GetComponent<RectTransform>());
        firstButton.GetComponent<ItemButton>().ToggleHighlight(true);
    }

    public void DeselectAllButtons()
    {
        for (int i = 0; i < ItemButtonParent.childCount; i++)
        {
            ItemButtonParent.GetChild(i).GetComponent<ItemButton>().ToggleHighlight(false);
        }
    }

    public void CreateButtonForItem(ItemData itemData, bool isEquipped = false)
    {
        ItemButton itemButton = Instantiate(ItemButtonPrefab, ItemButtonParent);

        itemButton.InitializeItemButton(this, itemData, itemData.itemID, PlayerContext, isEquipped);
        instantiatedItemButtons.Add(itemData.itemID, itemButton);
    }

    public void CreateButtonForItem(PotionSO potionData)
    {
        ItemButton itemButton = Instantiate(ItemButtonPrefab, ItemButtonParent);

        if (itemButton is ConsumableButton consumableButton)
        {
            string id = GUID.Generate().ToString();
            consumableButton.InitializeItemButton(this, potionData, id, PlayerContext);
            instantiatedItemButtons.Add(id, itemButton);
        }
    }

    public void RemoveButtonAtID(string id)
    {
        ItemButton itemButton = GetItemButtonByID(id);
        if (itemButton == null) return;

        if (ItemButtonParent.childCount == 1)
        {
            PlayerContext.PlayerController.PlayerStatsBlackboard.AddCurrentWeight(-itemButton.ItemData.itemWeight);
            instantiatedItemButtons.Remove(id);
            Destroy(itemButton.gameObject);
            PlayerContext.UserInterfaceController.eventSystem.SetSelectedGameObject(LeftTab.gameObject);
        }
        else
        {
            int childNumber = itemButton.transform.GetSiblingIndex();
            PlayerContext.PlayerController.PlayerStatsBlackboard.AddCurrentWeight(-itemButton.ItemData.itemWeight);
            instantiatedItemButtons.Remove(id);
            Destroy(itemButton.gameObject);
            if (childNumber == 0)
            {
                SelectButton(childNumber + 1);
            }
            else
            {
                SelectButton(childNumber - 1);
            }
        }
    }

    public ConsumableButton TryFindConsumableButtonOfType(PotionSO potionData)
    {
        if (instantiatedItemButtons.Count == 0) return null;

        for (int i = 0; i < ItemButtonParent.childCount; i++)
        {
            if (ItemButtonParent.GetChild(i).TryGetComponent(out ConsumableButton button))
            {
                if (button.PotionData == potionData)
                    return button;
            }
        }
        return null;
    }

    public ConsumableButton TryFindLargestPotionOfType(Resources.ResourceType resourceType)
    {
        if (instantiatedItemButtons.Count == 0) return null;

        ConsumableButton largestPotionButton = null;
        int restoreAmount = 0;

        for (int i = 0; i < ItemButtonParent.childCount; i++)
        {
            if (ItemButtonParent.GetChild(i).TryGetComponent(out ConsumableButton button))
            {
                if (button.PotionData.ResourceToRestore == resourceType && button.PotionData.AmountOfResourceToRestore > restoreAmount)
                {
                    largestPotionButton = button;
                    restoreAmount = button.PotionData.AmountOfResourceToRestore;
                }
            }
        }

        if (largestPotionButton == null)
        {
            Debug.Log($"Found no {resourceType} potions in inventory");
        }
        return largestPotionButton;
    }

    public ItemButton GetItemButtonByID(string id)
    {
        if (instantiatedItemButtons.TryGetValue(id, out ItemButton itemButton))
        {
            return itemButton;
        }
        return null;
    }

    public void MoveEquippedButtonsToTop()
    {
        List<GameObject> equippedButtons = new();
        for (int i = 0; i < ItemButtonParent.childCount; i++)
        {
            if (ItemButtonParent.GetChild(i).GetComponent<ItemButton>().buttonState == ItemButton.ButtonState.Activated)
            {
                equippedButtons.Add(ItemButtonParent.GetChild(i).gameObject);
            }
        }

        if (equippedButtons.Count == 0) return;

        foreach (GameObject obj in equippedButtons)
        {
            obj.transform.SetAsFirstSibling();
        }
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
