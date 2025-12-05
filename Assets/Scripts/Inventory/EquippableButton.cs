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

    public override void InitializeItemButton(InventoryItemController inventoryItemController, PlayerContext playerContext, InventoryItemView inventoryItemView, bool isEquipped = false)
    {
        InventoryItemController = inventoryItemController;
        PlayerContext = playerContext;
        this.inventoryItemView = inventoryItemView;
        ItemSO = inventoryItemView.ItemSO;
        ButtonID = inventoryItemView.SlotID;

        if (inventoryItemView.HasItemData)
            ItemData = inventoryItemView.ItemData;

        weaponController = playerContext.PlayerController.WeaponController;
        armorController = playerContext.PlayerController.ArmorController;

        Button button = GetComponent<Button>();
        button.onClick.AddListener(ActivateButton);

        UpdateUI();

        if (isEquipped)
        {
            OnLeftClick();
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
        ItemName.text = inventoryItemView.ItemQuality.ToString() + " " + inventoryItemView.ItemSO.ItemName;
        ItemButtonImage.sprite = inventoryItemView.ItemSO.ItemSprite;
        ItemValue.text = inventoryItemView.GoldValue.ToString();
        ItemWeight.text = inventoryItemView.ItemSO.Weight.ToString("0.0");

        if (inventoryItemView.ItemSO is WeaponSO)
        {
            StatValue.text = inventoryItemView.DisplayMinDamage + "-" + inventoryItemView.DisplayMaxDamage;
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
    }

    void OnDestroy()
    {
        armorController.OnArmorUnequipped -= CheckIfItemUnequipped;
        weaponController.OnWeaponUnequipped -= CheckIfItemUnequipped;
        PlayerContext.PlayerController.PlayerStatsBlackboard.OnGoldAmountChanged -= CheckIfButtonCanBeActivated;
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

    private void CheckIfButtonCanBeActivated()
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

    private void CheckIfButtonCanBeActivatedBuyMode()
    {
        if (inventoryItemView.GoldValue > PlayerContext.PlayerController.PlayerStatsBlackboard.GoldAmount)
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
    }

    public void OnLeftClickSellMode()
    {
        if (!inventoryItemView.HasItemData) return;

        if (buttonState == ButtonState.Activated)
        {
            if (IsItemWeapon() || ItemSO.ItemType == ItemType.Offhand)
            {
                weaponController.UnequipWeapon(ItemData);
            }
            else if (IsItemArmor())
            {
                armorController.UnequipArmor(ItemData);
            }
        }

        InventoryItemController.PlayerContext.PlayerController.PlayerStatsBlackboard.AddGold(inventoryItemView.GoldValue);
        InventoryItemController.RemoveButtonAtID(ButtonID);
    }

    public void OnLeftClickBuyMode()
    {
        if (buttonState == ButtonState.CannotActivate)
            return;

        InventoryItemController.PlayerContext.PlayerController.PlayerStatsBlackboard.AddGold(-inventoryItemView.GoldValue);
        ItemData itemData = SpawnedItemDataBase.Instance.CreateItemData(ItemSO, inventoryItemView.ItemQuality);
        inventoryItemView.ItemData = itemData;
        VendorController.OnItemPurchased?.Invoke(ButtonID);
        InventoryItemController.CreateButtonForItem(inventoryItemView);
        InventoryItemController.RemoveBuyButtonAtID(ButtonID);
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
}
