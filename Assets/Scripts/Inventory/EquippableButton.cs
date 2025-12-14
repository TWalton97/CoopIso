using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class EquippableButton : ItemButton
{
    public Color OneHandedEquippedColor;
    public Color TwoHandedEquippedColor;
    public Color RangedEquippedColor;

    private NewWeaponController weaponController;
    private ArmorController armorController;
    public TMP_Text StatValue;

    public GameObject DefaultInfoPanel;
    public GameObject AltInfoPanel;

    public GemEntry[] GemEntries;

    public override void InitializeItemButton(InventoryItemController inventoryItemController, PlayerContext playerContext, InventoryItemView inventoryItemView, bool isEquipped = false, InventoryMode mode = InventoryMode.Normal, bool equipToOffhand = false)
    {
        InventoryItemController = inventoryItemController;
        PlayerContext = playerContext;
        this.inventoryItemView = inventoryItemView;
        ItemSO = inventoryItemView.ItemSO;
        ButtonID = inventoryItemView.SlotID;
        ItemMode = mode;

        if (inventoryItemView.HasItemData)
            ItemData = inventoryItemView.ItemData;

        weaponController = playerContext.PlayerController.WeaponController;
        armorController = playerContext.PlayerController.ArmorController;

        Button button = GetComponent<Button>();
        button.onClick.AddListener(ActivateButton);

        UpdateUI();

        if (isEquipped)
        {
            if (equipToOffhand && CanEquipOffhand())
            {
                CallbackContext context = new CallbackContext();
                OnEquipOffhand(context);
            }
            else
            {
                OnLeftClick();
            }
        }
        else
        {
            CheckIfButtonCanBeActivated();
        }

        armorController.OnArmorUnequipped += CheckIfItemUnequipped;
        weaponController.OnWeaponUnequipped += CheckIfItemUnequipped;
    }

    public override void UpdateUI()
    {
        // ItemName.text = inventoryItemView.ItemQuality.ToString() + " " + inventoryItemView.ItemSO.ItemName;
        ItemName.text = ItemData.GetModifiedItemName();
        ItemButtonImage.sprite = inventoryItemView.ItemSO.ItemSprite;
        ItemWeight.text = inventoryItemView.ItemSO.Weight.ToString("0.0");

        if (ItemMode == InventoryMode.Buy)
        {
            ItemValue.text = (inventoryItemView.DisplayGoldValue * VendorPriceMultipliers.EquippableItemPriceMultiplier).ToString();
        }
        else
        {
            ItemValue.text = inventoryItemView.DisplayGoldValue.ToString();
        }

        if (inventoryItemView.ItemSO is WeaponSO)
        {
            StatValue.text = inventoryItemView.DisplayMinDamage + "-" + inventoryItemView.DisplayMaxDamage + " (" + inventoryItemView.DisplayWeaponAttackSpeed + ")";
        }
        else if (inventoryItemView.ItemSO is ArmorSO)
        {
            StatValue.text = inventoryItemView.DisplayArmorAmount.ToString();
        }
        else if (inventoryItemView.ItemSO is ShieldSO)
        {
            StatValue.text = inventoryItemView.DisplayShieldArmorAmount.ToString();
        }

        switch (inventoryItemView.ItemSO.ItemType)
        {
            case ItemType.OneHanded:
                EquippedColor = OneHandedEquippedColor;
                break;
            case ItemType.TwoHanded:
                EquippedColor = TwoHandedEquippedColor;
                break;
            case ItemType.Bow:
                EquippedColor = RangedEquippedColor;
                break;
        }
    }

    void OnEnable()
    {
        CheckIfButtonCanBeActivated();
        FillOutGemEntry();
        PlayerContext.UserInterfaceController.OnAltInfoPressed += () => ToggleAltInfo(true);
        PlayerContext.UserInterfaceController.OnAltInfoReleased += () => ToggleAltInfo(false);
    }

    void OnDisable()
    {
        ToggleHighlight(false);

        PlayerContext.UserInterfaceController.OnAltInfoPressed -= () => ToggleAltInfo(true);
        PlayerContext.UserInterfaceController.OnAltInfoReleased -= () => ToggleAltInfo(false);

        ToggleAltInfo(false);
    }

    void OnDestroy()
    {
        armorController.OnArmorUnequipped -= CheckIfItemUnequipped;
        weaponController.OnWeaponUnequipped -= CheckIfItemUnequipped;
        PlayerContext.PlayerController.PlayerStatsBlackboard.OnGoldAmountChanged -= CheckIfButtonCanBeActivated;
    }

    private void ToggleAltInfo(bool toggle)
    {
        if (toggle)
        {
            if (!IsSelected)
                return;
        }

        if (DefaultInfoPanel != null)
            DefaultInfoPanel.SetActive(!toggle);

        if (AltInfoPanel != null)
            AltInfoPanel.SetActive(toggle);
    }

    private void SetBackgroundColor(Color color)
    {
        foreach (Image image in BackgroundImages)
        {
            image.color = color;
        }
    }

    private void CheckIfItemUnequipped(string id)
    {
        if (ItemData == null) return;

        if (id == ItemData.ItemID)
        {
            SetBackgroundColor(BaseColor);
            buttonState = ButtonState.Default;
        }
    }

    public override void CheckIfButtonCanBeActivated()
    {
        if (InventoryItemController.InventoryMode == InventoryMode.Normal || InventoryItemController.InventoryMode == InventoryMode.Sell)
        {
            CheckIfButtonCanBeActivatedNormalMode();
        }
        else
        {
            CheckIfButtonCanBeActivatedBuyMode();
        }
    }

    private void CheckIfButtonCanBeActivatedNormalMode()
    {
        if (buttonState == ButtonState.Activated) return;

        if (IsItemArmor() && armorController.CanItemBeEquipped(ItemSO))
        {
            SetBackgroundColor(BaseColor);
            buttonState = ButtonState.Default;
            return;
        }

        if (IsItemWeapon())
        {
            SetBackgroundColor(BaseColor);
            buttonState = ButtonState.Default;
            return;
        }

        if (ItemSO.ItemType == ItemType.Offhand && PlayerContext.PlayerController.PlayerStatsBlackboard.armorType == ArmorType.Medium || PlayerContext.PlayerController.PlayerStatsBlackboard.armorType == ArmorType.Heavy)
        {
            SetBackgroundColor(BaseColor);
            buttonState = ButtonState.Default;
            return;
        }

        SetBackgroundColor(CannotBeUsedColor);
        buttonState = ButtonState.CannotActivate;
    }

    public void CheckIfButtonCanBeActivatedBuyMode()
    {
        if ((inventoryItemView.DisplayGoldValue * VendorPriceMultipliers.EquippableItemPriceMultiplier) > PlayerContext.PlayerController.PlayerStatsBlackboard.GoldAmount)
        {
            SetBackgroundColor(CannotBeUsedColor);
            buttonState = ButtonState.CannotActivate;
        }
        else
        {
            SetBackgroundColor(BaseColor);
            buttonState = ButtonState.Default;
        }
    }

    public override void OnLeftClick()
    {
        if (InventoryItemController.InventoryMode == InventoryMode.Normal)
        {
            OnLeftClickNormalMode();
        }
        else if (InventoryItemController.InventoryMode == InventoryMode.Sell)
        {
            OnLeftClickSellMode();
        }
        else
        {
            OnLeftClickBuyMode();
        }
    }

    public void OnLeftClickNormalMode()
    {
        if (buttonState == ButtonState.CannotActivate)
            return;

        if (!inventoryItemView.HasItemData) return;

        if (buttonState == ButtonState.Default)
        {
            if (IsItemWeapon() || ItemSO.ItemType == ItemType.Offhand)
            {
                SetBackgroundColor(EquippedColor);
                weaponController.EquipWeapon(ItemData);
                buttonState = ButtonState.Activated;
            }
            else if (IsItemArmor())
            {
                SetBackgroundColor(EquippedColor);
                armorController.EquipArmor(ItemData);
                buttonState = ButtonState.Activated;
            }

        }
        else if (buttonState == ButtonState.Activated)
        {
            if (IsItemWeapon() || ItemSO.ItemType == ItemType.Offhand)
            {
                SetBackgroundColor(BaseColor);
                weaponController.UnequipWeapon(ItemData);
                buttonState = ButtonState.Default;
            }
            else if (IsItemArmor())
            {
                SetBackgroundColor(BaseColor);
                armorController.UnequipArmor(ItemData);
                buttonState = ButtonState.Default;
            }
        }

        InventoryItemController.CheckStatusOfButtons();
    }

    public void OnLeftClickSellMode()
    {
        if (!inventoryItemView.HasItemData) return;

        if (buttonState == ButtonState.Activated)
        {
            PlayerContext.InventoryController.CallConfirmPanelForAction(UnequipAndSellItem, gameObject);
            return;
        }

        InventoryItemController.PlayerContext.PlayerController.PlayerStatsBlackboard.AddGold(inventoryItemView.DisplayGoldValue);
        InventoryItemController.CreateButtonForBuyItem(inventoryItemView);
        InventoryItemController.RemoveButtonAtID(ButtonID);

        InventoryItemController.CheckStatusOfButtons();
    }

    public void UnequipAndSellItem()
    {
        if (IsItemWeapon() || ItemSO.ItemType == ItemType.Offhand)
        {
            weaponController.UnequipWeapon(ItemData);
        }
        else if (IsItemArmor())
        {
            armorController.UnequipArmor(ItemData);
        }

        InventoryItemController.PlayerContext.PlayerController.PlayerStatsBlackboard.AddGold(inventoryItemView.DisplayGoldValue);
        InventoryItemController.CreateButtonForBuyItem(inventoryItemView);
        InventoryItemController.RemoveButtonAtID(ButtonID);

        InventoryItemController.CheckStatusOfButtons();
    }

    public void OnLeftClickBuyMode()
    {
        if (buttonState == ButtonState.CannotActivate)
            return;

        InventoryItemController.PlayerContext.PlayerController.PlayerStatsBlackboard.AddGold(-(int)(inventoryItemView.DisplayGoldValue * VendorPriceMultipliers.EquippableItemPriceMultiplier));
        VendorController.OnItemPurchased?.Invoke(inventoryItemView.ItemData.ItemID);
        InventoryItemController.CreateButtonForItem(inventoryItemView);
        InventoryItemController.RemoveBuyButtonAtID(ButtonID);

        InventoryItemController.CheckStatusOfButtons();
    }

    public bool CanEquipOffhand()
    {
        return weaponController.CanEquipOffhand(ItemSO);
    }

    public override void OnEquipOffhand(CallbackContext context)
    {
        if (buttonState == ButtonState.CannotActivate)
            return;

        if (!inventoryItemView.HasItemData) return;

        if (!CanEquipOffhand())
            return;

        if (buttonState == ButtonState.Default)
        {
            if (IsItemWeapon())
            {
                SetBackgroundColor(EquippedColor);
                weaponController.EquipWeapon(ItemData, Weapon.WeaponHand.OffHand);
                buttonState = ButtonState.Activated;
            }
        }
        else if (buttonState == ButtonState.Activated)
        {
            if (IsItemWeapon())
            {
                SetBackgroundColor(EquippedColor);
                weaponController.UnequipWeapon(ItemData);
                buttonState = ButtonState.Default;
            }
        }
    }

    public override void OnRightClick()
    {
        if (!inventoryItemView.HasItemData) return;

        if (InventoryItemController.InventoryMode != InventoryMode.Normal) return;

        if (buttonState == ButtonState.Activated)
        {
            if (IsItemWeapon() || ItemSO.ItemType == ItemType.Offhand)
            {
                SetBackgroundColor(EquippedColor);
                weaponController.UnequipWeapon(ItemData);
                buttonState = ButtonState.Default;
            }
            else if (IsItemArmor())
            {
                SetBackgroundColor(EquippedColor);
                armorController.UnequipArmor(ItemData);
                buttonState = ButtonState.Default;
            }
        }
        else
        {
            SpawnedItemDataBase.Instance.SpawnItemFromDatabase(ItemData.ItemID, ReturnSpawnPositionInRadius(), ItemData.GroundPrefab.transform.rotation);
            InventoryItemController.RemoveButtonAtID(ButtonID);
        }

        InventoryItemController.CheckStatusOfButtons();
    }

    private Vector3 ReturnSpawnPositionInRadius()
    {
        Vector3 insideUnitCircle = Random.insideUnitCircle;
        insideUnitCircle = new Vector3(insideUnitCircle.x, 0, insideUnitCircle.y);
        return PlayerContext.PlayerController.transform.position + insideUnitCircle;
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        InventoryItemController.UpdateViewPosition(GetComponent<RectTransform>(), GetComponentInParent<ScrollRect>());
        if (PlayerContext.UserInterfaceController.AltInfoPressed)
        {
            ToggleAltInfo(true);
        }
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        ToggleAltInfo(false);
    }

    public bool IsItemArmor()
    {
        if (ItemSO.ItemType == ItemType.Head)
            return true;

        if (ItemSO.ItemType == ItemType.Body)
            return true;

        if (ItemSO.ItemType == ItemType.Legs)
            return true;

        return false;
    }

    public bool IsItemWeapon()
    {
        if (ItemSO.ItemType == ItemType.OneHanded)
            return true;

        if (ItemSO.ItemType == ItemType.TwoHanded)
            return true;

        if (ItemSO.ItemType == ItemType.Bow)
            return true;

        return false;
    }

    public override void ActivateButton()
    {
        OnLeftClick();
    }

    public void FillOutGemEntry()
    {
        for (int i = 0; i < 3; i++)
        {
            GemEntries[i].GemImage.sprite = GemEntries[i].DefaultSprite;
            GemEntries[i].GemImage.color = Color.white;
            GemEntries[i].GemDescription.text = "";
        }

        int index = 0;
        foreach (GemSocket socket in ItemData.socketedGems)
        {
            GemEntries[index].GemImage.sprite = socket.Gem.GemSprite;
            GemEntries[index].GemImage.color = socket.Gem.GemColor;
            GemEntries[index].GemDescription.text = socket.Gem.GetDescriptionForSlot(ItemData.EquipmentSlotType);
            index++;
        }
    }
}

[System.Serializable]
public class GemEntry
{
    public Image GemImage;
    public Sprite DefaultSprite;
    public TMP_Text GemDescription;
}
