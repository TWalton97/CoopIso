using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquippableButton : ItemButton
{
    public Color OneHandedEquippedColor;
    public Color TwoHandedEquippedColor;
    public Color RangedEquippedColor;

    public enum ButtonState
    {
        Default,
        Activated,
        CannotActivate
    }
    private ButtonState buttonState = ButtonState.Default;
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

        ItemName.text = ItemData.itemQuality + " " + ItemData.itemName;
        ItemButtonImage.sprite = ItemData.sprite;
        ItemValue.text = ItemData.itemValue.ToString();
        ItemWeight.text = ItemData.itemWeight.ToString("00.0");
        if (ItemData.data is WeaponDataSO weaponData)
        {
            StatValue.text = LootCalculator.CalculateQualityModifiedStat(weaponData.WeaponMinDamage, itemData.itemQuality).ToString() + "-" + LootCalculator.CalculateQualityModifiedStat(weaponData.WeaponMaxDamage, itemData.itemQuality).ToString();
        }
        else if (ItemData.data is BowSO bowData)
        {
            StatValue.text = LootCalculator.CalculateQualityModifiedStat(bowData.WeaponMinDamage, itemData.itemQuality).ToString() + "-" + LootCalculator.CalculateQualityModifiedStat(bowData.WeaponMaxDamage, itemData.itemQuality).ToString();
        }
        else if (ItemData.data is ArmorSO armorData)
        {
            StatValue.text = LootCalculator.CalculateQualityModifiedStat(armorData.ArmorAmount, itemData.itemQuality).ToString();
        }
        else if (ItemData.data is ShieldSO shieldData)
        {
            StatValue.text = LootCalculator.CalculateQualityModifiedStat(shieldData.ArmorAmount, itemData.itemQuality).ToString();
        }

        switch (ItemData.itemType)
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
            CheckIfItemCanBeEquipped();
        }

        PlayerContext.PlayerController.PlayerInputController.OnDropItemPerformed += OnDropItem;
        armorController.OnArmorUnequipped += CheckIfItemUnequipped;
        weaponController.OnWeaponUnequipped += CheckIfItemUnequipped;
    }

    void OnEnable()
    {
        CheckIfItemCanBeEquipped();
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
        if (id == ItemData.itemID)
        {
            SetBackgroundColor(BaseColor);
            buttonState = ButtonState.Default;
        }
    }

    private void CheckIfItemCanBeEquipped()
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

        if (ItemData.itemType == ItemType.Offhand && PlayerContext.PlayerController.PlayerStatsBlackboard.armorType == ArmorType.Medium || PlayerContext.PlayerController.PlayerStatsBlackboard.armorType == ArmorType.Heavy)
        {
            SetBackgroundColor(BaseColor);
            buttonState = ButtonState.Default;
            return;
        }

        SetBackgroundColor(CannotBeUsedColor);
        buttonState = ButtonState.CannotActivate;
    }

    public override void OnLeftClick()
    {
        if (buttonState == ButtonState.CannotActivate)
            return;

        if (buttonState == ButtonState.Default)
        {
            if (IsItemWeapon() || ItemData.itemType == ItemType.Offhand)
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
            if (IsItemWeapon() || ItemData.itemType == ItemType.Offhand)
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
    }

    public override void OnRightClick()
    {
        if (buttonState == ButtonState.Activated)
        {
            if (IsItemWeapon() || ItemData.itemType == ItemType.Offhand)
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
            SpawnedItemDataBase.Instance.SpawnItemFromDatabase(ItemData.itemID, PlayerContext.PlayerController.transform.position, Quaternion.identity);
            InventoryItemController.RemoveButtonAtID(ButtonID);
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        InventoryItemController.UpdateViewPosition(GetComponent<RectTransform>());
    }

    public bool IsItemArmor()
    {
        if (ItemData.itemType == ItemType.Head)
            return true;

        if (ItemData.itemType == ItemType.Body)
            return true;

        if (ItemData.itemType == ItemType.Legs)
            return true;

        return false;
    }

    public bool IsItemWeapon()
    {
        if (ItemData.itemType == ItemType.OneHanded)
            return true;

        if (ItemData.itemType == ItemType.TwoHanded)
            return true;

        if (ItemData.itemType == ItemType.Bow)
            return true;

        return false;
    }

    public override void ActivateButton()
    {
        OnLeftClick();
    }
}
