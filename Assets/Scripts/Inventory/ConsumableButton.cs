using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConsumableButton : ItemButton
{
    public int Quantity = 0;
    public TMP_Text QuantityText;

    public override void InitializeItemButton(InventoryItemController inventoryItemController, PlayerContext playerContext, InventoryItemView inventoryItemView, bool isEquipped = false, InventoryMode mode = InventoryMode.Normal)
    {
        InventoryItemController = inventoryItemController;
        PlayerContext = playerContext;
        this.inventoryItemView = inventoryItemView;
        ButtonID = inventoryItemView.SlotID;
        ItemSO = inventoryItemView.ItemSO;
        ItemMode = mode;

        if (inventoryItemView.HasItemData)
            ItemData = inventoryItemView.ItemData;

        Button button = GetComponent<Button>();
        button.onClick.AddListener(ActivateButton);

        UpdateQuantity(inventoryItemView.Quantity);
        UpdateUI();

        CheckIfButtonCanBeActivated();

        PlayerContext.PlayerController.PlayerStatsBlackboard.OnGoldAmountChanged += CheckIfButtonCanBeActivated;
    }

    void OnDestroy()
    {
        PlayerContext.PlayerController.PlayerStatsBlackboard.OnGoldAmountChanged -= CheckIfButtonCanBeActivated;
    }

    public override void UpdateUI()
    {
        ItemName.text = ItemData.GetModifiedItemName();
        ItemButtonImage.sprite = inventoryItemView.ItemSO.ItemSprite;
        ItemWeight.text = inventoryItemView.ItemSO.Weight.ToString("0.0");

        if (ItemMode == InventoryMode.Buy)
        {
            ItemValue.text = (inventoryItemView.DisplayGoldValue * VendorPriceMultipliers.ConsumableItemPriceMultiplier).ToString();
        }
        else
        {
            ItemValue.text = inventoryItemView.DisplayGoldValue.ToString();
        }
    }
    void OnEnable()
    {
        CheckIfButtonCanBeActivated();
        if (InventoryItemController.InventoryMode == InventoryMode.Buy || Quantity <= 1)
        {
            QuantityText.enabled = false;
        }
        else
        {
            QuantityText.enabled = true;
        }
    }

    void OnDisable()
    {
        ToggleHighlight(false);
    }

    public void UpdateQuantity(int amount)
    {
        Quantity += amount;
        if (Quantity <= 0 && InventoryItemController.InventoryMode != InventoryMode.Buy)
        {
            InventoryItemController.RemoveButtonAtID(ButtonID);
            return;
        }
        QuantityText.text = "x" + Quantity.ToString();

        if (Quantity <= 1)
        {
            QuantityText.enabled = false;
        }
        else
        {
            QuantityText.enabled = true;
        }
    }

    private void SetBackgroundColor(Color color)
    {
        foreach (Image image in BackgroundImages)
        {
            image.color = color;
        }
    }

    public override void CheckIfButtonCanBeActivated()
    {
        if (InventoryItemController.InventoryMode == InventoryMode.Buy)
        {
            if (inventoryItemView.DisplayGoldValue * VendorPriceMultipliers.ConsumableItemPriceMultiplier > PlayerContext.PlayerController.PlayerStatsBlackboard.GoldAmount)
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
        else
        {
            SetBackgroundColor(BaseColor);
            buttonState = ButtonState.Default;
        }
    }

    public override void ActivateButton()
    {
        OnLeftClick();
    }

    public override void OnLeftClick()
    {
        if (InventoryItemController.InventoryMode == InventoryMode.Buy)
        {
            OnLeftClickBuyMode();
        }
        else if (InventoryItemController.InventoryMode == InventoryMode.Sell)
        {
            OnLeftClickSellMode();
        }
    }

    public void OnLeftClickBuyMode()
    {
        if (buttonState == ButtonState.CannotActivate)
            return;

        InventoryItemController.PlayerContext.PlayerController.PlayerStatsBlackboard.AddGold(-(int)(inventoryItemView.DisplayGoldValue * VendorPriceMultipliers.ConsumableItemPriceMultiplier));
        InventoryItemController.PlayerContext.InventoryController.AddConsumableToInventory(ItemSO, 1);

        InventoryItemController.CheckStatusOfButtons();
    }

    public void OnLeftClickSellMode()
    {
        InventoryItemController.PlayerContext.PlayerController.PlayerStatsBlackboard.AddGold(inventoryItemView.DisplayGoldValue);
        UpdateQuantity(-1);

        InventoryItemController.CheckStatusOfButtons();
    }

    public override void OnRightClick()
    {
        if (InventoryItemController.InventoryMode != InventoryMode.Normal) return;

        Vector3 spawnPos = NavMeshUtils.ReturnRandomPointOnXZ(PlayerContext.PlayerController.transform.position, 1f);
        spawnPos.y = 0f;
        Item item = Instantiate(inventoryItemView.ItemSO.GroundItemPrefab, spawnPos, Quaternion.identity).GetComponent<Item>();
        item.ItemSO = ItemSO;
        item.Quantity = 1;
        UpdateQuantity(-1);

        InventoryItemController.CheckStatusOfButtons();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        InventoryItemController.UpdateViewPosition(GetComponent<RectTransform>(), GetComponentInParent<ScrollRect>());
    }
}
