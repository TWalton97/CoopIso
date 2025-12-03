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

    public override void InitializeItemButton(InventoryItemController inventoryItemController, ItemData itemData, string buttonID, PlayerContext playerContext, bool isEquipped = false)
    {
        InventoryItemController = inventoryItemController;
        ItemData = itemData;
        ButtonID = buttonID;
        PlayerContext = playerContext;

        weaponController = playerContext.PlayerController.WeaponController;
        armorController = playerContext.PlayerController.ArmorController;

        Button button = GetComponent<Button>();
        button.onClick.AddListener(ActivateButton);

        ItemName.text = ItemData.Quality + " " + ItemData.Name;
        ItemButtonImage.sprite = ItemData.Sprite;
        ItemValue.text = ItemData.GoldValue.ToString();
        ItemWeight.text = ItemData.Weight.ToString("0.0");
        if (ItemData.ItemSO is WeaponSO weaponData)
        {
            StatValue.text = ItemData.MinDamage + "-" + ItemData.MaxDamage;
        }
        else if (ItemData.ItemSO is ArmorSO armorData)
        {
            StatValue.text = ItemData.ArmorAmount.ToString();
        }
        else if (ItemData.ItemSO is ShieldSO shieldData)
        {
            StatValue.text = ItemData.ShieldArmorAmount.ToString();
        }

        switch (ItemData.ItemSO.ItemType)
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

    void OnEnable()
    {
        CheckIfButtonCanBeActivated();
    }

    void OnDestroy()
    {
        armorController.OnArmorUnequipped -= CheckIfItemUnequipped;
        weaponController.OnWeaponUnequipped -= CheckIfItemUnequipped;
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

        if (IsItemArmor() && armorController.CanItemBeEquipped(ItemData))
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

        if (ItemData.ItemSO.ItemType == ItemType.Offhand && PlayerContext.PlayerController.PlayerStatsBlackboard.armorType == ArmorType.Medium || PlayerContext.PlayerController.PlayerStatsBlackboard.armorType == ArmorType.Heavy)
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
        if (ItemData.ItemSO.GoldValue > PlayerContext.PlayerController.PlayerStatsBlackboard.GoldAmount)
        {
            SetBackgroundColor(CannotBeUsedColor);
        }
        else
        {
            SetBackgroundColor(BaseColor);
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

        if (buttonState == ButtonState.Default)
        {
            if (IsItemWeapon() || ItemData.ItemSO.ItemType == ItemType.Offhand)
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
            if (IsItemWeapon() || ItemData.ItemSO.ItemType == ItemType.Offhand)
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
        if (buttonState == ButtonState.Activated)
        {
            if (IsItemWeapon() || ItemData.ItemSO.ItemType == ItemType.Offhand)
            {
                weaponController.UnequipWeapon(ItemData);
            }
            else if (IsItemArmor())
            {
                armorController.UnequipArmor(ItemData);

            }
        }

        InventoryItemController.PlayerContext.PlayerController.PlayerStatsBlackboard.AddGold(ItemData.ItemSO.GoldValue);
        InventoryItemController.RemoveButtonAtID(ButtonID);
    }

    public void OnLeftClickBuyMode()
    {
        if (buttonState == ButtonState.CannotActivate)
            return;

        InventoryItemController.PlayerContext.PlayerController.PlayerStatsBlackboard.AddGold(-ItemData.ItemSO.GoldValue);
        InventoryItemController.CreateButtonForItem(ItemData);
        InventoryItemController.RemoveBuyButtonAtID(ButtonID);
    }

    public bool CanEquipOffhand()
    {
        return weaponController.CanEquipOffhand(ItemData);
    }

    public override void OnEquipOffhand(CallbackContext context)
    {
        if (buttonState == ButtonState.CannotActivate)
            return;

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
        if (buttonState == ButtonState.Activated)
        {
            if (IsItemWeapon() || ItemData.ItemSO.ItemType == ItemType.Offhand)
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
        InventoryItemController.UpdateViewPosition(GetComponent<RectTransform>());
    }

    public bool IsItemArmor()
    {
        if (ItemData.ItemSO.ItemType == ItemType.Head)
            return true;

        if (ItemData.ItemSO.ItemType == ItemType.Body)
            return true;

        if (ItemData.ItemSO.ItemType == ItemType.Legs)
            return true;

        return false;
    }

    public bool IsItemWeapon()
    {
        if (ItemData.ItemSO.ItemType == ItemType.OneHanded)
            return true;

        if (ItemData.ItemSO.ItemType == ItemType.TwoHanded)
            return true;

        if (ItemData.ItemSO.ItemType == ItemType.Bow)
            return true;

        return false;
    }

    public override void ActivateButton()
    {
        OnLeftClick();
    }
}
