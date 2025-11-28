using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquippableButton : ItemButton
{
    public enum ButtonState
    {
        Default,
        Activated,
        CannotActivate
    }
    private ButtonState buttonState = ButtonState.Default;
    public InventoryItemController InventoryItemController;
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

        ItemName.text = ItemData.itemName;
        ItemButtonImage.sprite = ItemData.sprite;
        ItemValue.text = ItemData.itemValue.ToString();
        ItemWeight.text = ItemData.itemWeight.ToString("00.0");
        if (ItemData.data is WeaponDataSO weaponData)
        {
            StatValue.text = weaponData.WeaponMinDamage.ToString() + "-" + weaponData.WeaponMaxDamage.ToString();
        }

        if (isEquipped)
        {
            OnLeftClick();
        }
        else
        {
            CheckIfItemCanBeEquipped();
        }

        armorController.OnArmorUnequipped += CheckIfItemUnequipped;
        weaponController.OnWeaponUnequipped += CheckIfItemUnequipped;
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
        if (IsItemArmor() && armorController.CanItemBeEquipped(ItemData))
        {
            SetBackgroundColor(BaseColor);
            buttonState = ButtonState.Default;
            return;
        }

        if (IsItemWeapon())
        {
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
            if (IsItemWeapon())
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
            if (IsItemWeapon())
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
        //If the item is equipped, unequip it
        //If the item isn't equipped, drop it
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
