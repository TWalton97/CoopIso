using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour
{
    // public Button LeftTab;
    // public Button RightTab;

    public PlayerContext PlayerContext;

    public GameObject ItemButtonScrollRect;
    public Transform ItemButtonParent;
    public GameObject BuyItemButtonScrollRect;
    public Transform BuyItemButtonParent;
    public ItemButton ItemButtonPrefab;

    public ScrollRect scrollRect;

    public Dictionary<string, ItemButton> instantiatedItemButtons = new Dictionary<string, ItemButton>();
    public Dictionary<string, ItemButton> instantiatedBuyItemButtons = new Dictionary<string, ItemButton>();

    public List<ControlData> NormalModeControlData;
    public List<ControlData> SellModeControlData;
    public List<ControlData> BuyModeControlData;

    public ControlData EquipOffhandControlData; //We add this if the weapon can be equipped offhand

    public InventoryMode InventoryMode;

    private void OnEnable()
    {
        if (instantiatedItemButtons.Count != 0)
        {
            MoveEquippedButtonsToTop();
            SelectButton();
        }

        if (instantiatedBuyItemButtons.Count != 0)
        {
            SelectButton();
        }

        // Navigation rightTabNavigation = new Navigation();
        // rightTabNavigation.mode = Navigation.Mode.Explicit;
        // rightTabNavigation.selectOnLeft = LeftTab;
        // if (ItemButtonParent.childCount != 0)
        // {
        //     rightTabNavigation.selectOnDown = ItemButtonParent.GetChild(0).GetComponent<Button>();
        // }
        // RightTab.navigation = rightTabNavigation;

        // Navigation leftTabNavigation = new Navigation();
        // leftTabNavigation.mode = Navigation.Mode.Explicit;
        // leftTabNavigation.selectOnRight = RightTab;
        // if (ItemButtonParent.childCount != 0)
        // {
        //     leftTabNavigation.selectOnDown = ItemButtonParent.GetChild(0).GetComponent<Button>();
        // }
        // LeftTab.navigation = leftTabNavigation;
    }

    public void UpdateControlsPanel(ItemButton button = null)
    {
        if (InventoryMode == InventoryMode.Normal && button != null)
        {
            var controls = new List<ControlData>(NormalModeControlData);
            if (button is EquippableButton equippableButton && equippableButton.CanEquipOffhand())
            {
                controls.Add(EquipOffhandControlData);
            }
            PlayerContext.UserInterfaceController.inventoryController.UpdateControlPanel(controls);
        }
        else if (InventoryMode == InventoryMode.Sell)
        {
            var controls = new List<ControlData>(SellModeControlData);
            PlayerContext.UserInterfaceController.inventoryController.UpdateControlPanel(controls);
        }
        else
        {
            var controls = new List<ControlData>(BuyModeControlData);
            PlayerContext.UserInterfaceController.inventoryController.UpdateControlPanel(controls);
        }
    }

    public void SelectButton(int index = 0)
    {
        if (InventoryMode == InventoryMode.Normal || InventoryMode == InventoryMode.Sell)
        {
            if (ItemButtonParent.childCount == 0) return;
            int clampedIndex = Mathf.Clamp(index, 0, ItemButtonParent.childCount - 1);
            GameObject firstButton = ItemButtonParent.GetChild(clampedIndex).gameObject;
            PlayerContext.UserInterfaceController.eventSystem.SetSelectedGameObject(firstButton);
            UpdateViewPosition(ItemButtonParent.GetChild(clampedIndex).GetComponent<RectTransform>(), ItemButtonScrollRect.GetComponent<ScrollRect>());
            firstButton.GetComponent<ItemButton>().ToggleHighlight(true);
        }
        else if (InventoryMode == InventoryMode.Buy)
        {
            if (BuyItemButtonParent.childCount == 0) return;
            int clampedIndex = Mathf.Clamp(index, 0, BuyItemButtonParent.childCount - 1);
            GameObject firstButton = BuyItemButtonParent.GetChild(clampedIndex).gameObject;
            PlayerContext.UserInterfaceController.eventSystem.SetSelectedGameObject(firstButton);
            UpdateViewPosition(BuyItemButtonParent.GetChild(clampedIndex).GetComponent<RectTransform>(), BuyItemButtonScrollRect.GetComponent<ScrollRect>());
            firstButton.GetComponent<ItemButton>().ToggleHighlight(true);
        }
    }

    public void DeselectAllButtons()
    {
        for (int i = 0; i < ItemButtonParent.childCount; i++)
        {
            ItemButtonParent.GetChild(i).GetComponent<ItemButton>().ToggleHighlight(false);
        }
    }

    public void CreateButtonForItem(InventoryItemView inventoryItemView, bool isEquipped = false, bool equipToOffhand = false)
    {
        ItemButton itemButton = Instantiate(ItemButtonPrefab, ItemButtonParent);
        string id = "";
        if (inventoryItemView.ItemData != null)
        {
            id = inventoryItemView.ItemData.ItemID;
            instantiatedItemButtons.Add(id, itemButton);
        }
        else
        {
            id = GUID.Generate().ToString();
            instantiatedItemButtons.Add(id, itemButton);
            ItemData itemData = new ItemData();
            itemData.ItemID = id;
            itemData.ItemSO = inventoryItemView.ItemSO;
            itemData.Quantity = inventoryItemView.Quantity;
            inventoryItemView.ItemData = itemData;
        }

        inventoryItemView.SlotID = id;
        itemButton.InitializeItemButton(this, PlayerContext, inventoryItemView, isEquipped, InventoryMode.Normal, equipToOffhand);
    }

    public void RemoveButtonAtID(string id)
    {
        ItemButton itemButton = GetItemButtonByID(id);
        if (itemButton == null) return;

        if (ItemButtonParent.childCount == 1)
        {
            PlayerContext.PlayerController.PlayerStatsBlackboard.AddCurrentWeight(-itemButton.inventoryItemView.ItemSO.Weight);
            instantiatedItemButtons.Remove(id);
            Destroy(itemButton.gameObject);
            //PlayerContext.UserInterfaceController.eventSystem.SetSelectedGameObject(LeftTab.gameObject);
        }
        else
        {
            int childNumber = itemButton.transform.GetSiblingIndex();
            PlayerContext.PlayerController.PlayerStatsBlackboard.AddCurrentWeight(-itemButton.inventoryItemView.ItemSO.Weight);
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

    public void RemoveBuyButtonAtID(string id)
    {
        ItemButton itemButton = GetBuyItemButtonByID(id);
        if (itemButton == null) return;

        if (instantiatedBuyItemButtons.Count == 1)
        {
            Destroy(itemButton.gameObject);
            instantiatedBuyItemButtons.Remove(id);
            //PlayerContext.UserInterfaceController.eventSystem.SetSelectedGameObject(LeftTab.gameObject);
        }
        else
        {
            int childNumber = itemButton.transform.GetSiblingIndex();
            Destroy(itemButton.gameObject);
            instantiatedBuyItemButtons.Remove(id);
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

    public ItemButton FindSlotByItemSO(ItemSO itemSO)
    {
        if (instantiatedItemButtons.Count == 0) return null;

        for (int i = 0; i < ItemButtonParent.childCount; i++)
        {
            if (ItemButtonParent.GetChild(i).TryGetComponent(out ItemButton button))
            {
                if (button.inventoryItemView.ItemSO == itemSO)
                    return button;
            }
        }
        return null;
    }

    public ConsumableButton TryFindLargestPotionOfType(PlayerResource.ResourceType resourceType)
    {
        if (instantiatedItemButtons.Count == 0) return null;

        ConsumableButton largestPotionButton = null;
        int restoreAmount = 0;

        for (int i = 0; i < ItemButtonParent.childCount; i++)
        {
            if (ItemButtonParent.GetChild(i).TryGetComponent(out ConsumableButton button))
            {
                if (button.inventoryItemView.ItemSO is PotionSO potionSO)
                {
                    if (potionSO.ResourceToRestore == resourceType && potionSO.AmountOfResourceToRestore > restoreAmount)
                    {
                        largestPotionButton = button;
                        restoreAmount = potionSO.AmountOfResourceToRestore;
                    }
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

    public ItemButton GetBuyItemButtonByID(string id)
    {
        if (instantiatedBuyItemButtons.TryGetValue(id, out ItemButton itemButton))
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

    public void UpdateViewPosition(RectTransform target, ScrollRect _scrollRect)
    {
        if (target == null) return;

        if (PlayerContext.PlayerController.PlayerInputController.playerInput.currentControlScheme == PlayerContext.PlayerController.KEYBOARD_SCHEME) return;
        _scrollRect.content.localPosition = GetSnapToPositionToBringChildIntoView(_scrollRect, target);
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

    public void SwapToInventoryMode(InventoryMode inventoryMode)
    {
        InventoryMode = inventoryMode;
        if (inventoryMode == InventoryMode.Normal || inventoryMode == InventoryMode.Sell)
        {
            var sortedItemButtons = instantiatedItemButtons.Values.OrderBy(b => b.inventoryItemView.DisplayGoldValue).ToList();
            for (int i = 0; i < sortedItemButtons.Count; i++)
            {
                sortedItemButtons[i].transform.SetSiblingIndex(i);
            }
            ItemButtonScrollRect.gameObject.SetActive(true);
            BuyItemButtonScrollRect.gameObject.SetActive(false);
        }
        else if (inventoryMode == InventoryMode.Buy)
        {
            var sortedBuyItemButtons = instantiatedBuyItemButtons.Values.OrderBy(b => b.inventoryItemView.DisplayGoldValue).ToList();
            for (int i = 0; i < sortedBuyItemButtons.Count; i++)
            {
                sortedBuyItemButtons[i].transform.SetSiblingIndex(i);
            }
            ItemButtonScrollRect.gameObject.SetActive(false);
            BuyItemButtonScrollRect.gameObject.SetActive(true);
        }
    }

    public void CreateButtonForBuyItem(InventoryItemView inventoryItemView, bool isEquipped = false)
    {
        if (!instantiatedBuyItemButtons.ContainsKey(inventoryItemView.SlotID))
        {
            ItemButton itemButton = Instantiate(ItemButtonPrefab, BuyItemButtonParent);
            instantiatedBuyItemButtons.Add(inventoryItemView.SlotID, itemButton);
            itemButton.InitializeItemButton(this, PlayerContext, inventoryItemView, isEquipped, InventoryMode.Buy);
        }
        else
        {
            instantiatedBuyItemButtons[inventoryItemView.SlotID].gameObject.SetActive(true);
        }
    }

    public void CheckStatusOfButtons()
    {
        if (InventoryMode == InventoryMode.Normal || InventoryMode == InventoryMode.Sell)
        {
            foreach (ItemButton button in instantiatedItemButtons.Values)
            {
                button.CheckIfButtonCanBeActivated();
            }
        }
        else
        {
            foreach (ItemButton button in instantiatedBuyItemButtons.Values)
            {
                button.CheckIfButtonCanBeActivated();
            }
        }
    }
}

[Serializable]
public class InventoryItemView
{
    public ItemSO ItemSO;
    public ItemData ItemData;
    public int Quantity;
    public ItemQuality ItemQuality;
    public int GoldValue;
    public bool IsStackable;
    public string SlotID;

    public InventoryItemView(ItemSO _itemSO, ItemData _itemData = null, int _quantity = 1, ItemQuality _itemQuality = ItemQuality.Normal, bool _isStackable = false, string _slotID = "")
    {
        ItemSO = _itemSO;
        ItemData = _itemData;
        Quantity = _quantity;
        ItemQuality = _itemQuality;
        IsStackable = _isStackable;
        SlotID = _slotID;
    }

    public bool HasItemData => ItemData != null;

    public int DisplayGoldValue
    {
        get
        {
            if (HasItemData)
                return ItemData.GoldValue;
            else
                return Mathf.CeilToInt(ItemSO.GoldValue * QualityTables.QualitySellMultiplier[ItemQuality]);
        }
    }

    public int DisplayMinDamage
    {
        get
        {
            if (HasItemData)
                return ItemData.MinDamage;
            else
                return (ItemSO as WeaponSO)?.WeaponMinDamage != null ? Mathf.RoundToInt((ItemSO as WeaponSO).WeaponMinDamage * QualityTables.QualityStatMultiplier[ItemQuality]) : 0;
        }
    }

    public int DisplayMaxDamage
    {
        get
        {
            if (HasItemData)
                return ItemData.MaxDamage;
            else
                return (ItemSO as WeaponSO)?.WeaponMaxDamage != null ? Mathf.RoundToInt((ItemSO as WeaponSO).WeaponMaxDamage * QualityTables.QualityStatMultiplier[ItemQuality]) : 0;
        }
    }

    public WeaponAttackSpeed DisplayWeaponAttackSpeed
    {
        get
        {
            if (HasItemData)
                return ItemData.WeaponAttackSpeed;
            else
                return (ItemSO as WeaponSO)?.WeaponAttackSpeed != null ? (ItemSO as WeaponSO).WeaponAttackSpeed : WeaponAttackSpeed.Normal;
        }
    }

    public WeaponRangeType DisplayWeaponRangeType
    {
        get
        {
            if (HasItemData)
                return ItemData.WeaponRangeType;
            else
                return (ItemSO as WeaponSO)?.WeaponRangeType != null ? (ItemSO as WeaponSO).WeaponRangeType : WeaponRangeType.None;
        }
    }

    public int DisplayArmorAmount
    {
        get
        {
            if (HasItemData)
                return ItemData.ArmorAmount;
            else
                return (ItemSO as ArmorSO)?.ArmorAmount != null ? Mathf.RoundToInt((ItemSO as ArmorSO).ArmorAmount * QualityTables.QualityStatMultiplier[ItemQuality]) : 0;
        }
    }

    public ArmorType DisplayArmorType
    {
        get
        {
            if (HasItemData)
                return ItemData.ArmorType;
            else
                return (ItemSO as ArmorSO)?.ArmorType != null ? (ItemSO as ArmorSO).ArmorType : ArmorType.None;
        }
    }

    public int DisplayShieldArmorAmount
    {
        get
        {
            if (HasItemData)
                return ItemData.ShieldArmorAmount;
            else
                return (ItemSO as ShieldSO)?.ArmorAmount != null ? Mathf.RoundToInt((ItemSO as ShieldSO).ArmorAmount * QualityTables.QualityStatMultiplier[ItemQuality]) : 0;
        }
    }

    public PlayerResource.ResourceType DisplayResourceType
    {
        get
        {
            if (HasItemData)
                return ItemData.ResourceType;
            else
                return (ItemSO as PotionSO)?.ResourceToRestore ?? PlayerResource.ResourceType.Health;
        }
    }

    public int DisplayResourceAmount
    {
        get
        {
            if (HasItemData)
                return ItemData.ResourceAmount;
            else
                return (ItemSO as PotionSO)?.AmountOfResourceToRestore ?? 0;
        }
    }
}
