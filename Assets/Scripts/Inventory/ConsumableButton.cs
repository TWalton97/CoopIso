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
        ItemName.text = inventoryItemView.ItemSO.ItemName;
        ItemButtonImage.sprite = inventoryItemView.ItemSO.ItemSprite;
        ItemWeight.text = inventoryItemView.ItemSO.Weight.ToString("0.0");

        if (ItemMode == InventoryMode.Buy)
        {
            ItemValue.text = (inventoryItemView.DisplayGoldValue * 2f).ToString();
        }
        else
        {
            ItemValue.text = inventoryItemView.DisplayGoldValue.ToString();
        }
    }
    void OnEnable()
    {
        CheckIfButtonCanBeActivated();
        if (InventoryItemController.InventoryMode == InventoryMode.Buy)
        {
            QuantityText.enabled = false;
        }
        else
        {
            QuantityText.enabled = true;
        }
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
    }

    private void SetBackgroundColor(Color color)
    {
        foreach (Image image in BackgroundImages)
        {
            image.color = color;
        }
    }

    private void CheckIfButtonCanBeActivated()
    {
        if (InventoryItemController.InventoryMode == InventoryMode.Buy)
        {
            if (inventoryItemView.DisplayGoldValue * 2f > PlayerContext.PlayerController.PlayerStatsBlackboard.GoldAmount)
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

        InventoryItemController.PlayerContext.PlayerController.PlayerStatsBlackboard.AddGold(-inventoryItemView.DisplayGoldValue * 2);
        InventoryItemController.PlayerContext.InventoryController.AddConsumableToInventory(ItemSO, 1);
    }

    public void OnLeftClickSellMode()
    {
        InventoryItemController.PlayerContext.PlayerController.PlayerStatsBlackboard.AddGold(inventoryItemView.DisplayGoldValue);
        UpdateQuantity(-1);
    }

    public override void OnRightClick()
    {
        Vector3 spawnPos = NavMeshUtils.ReturnRandomPointOnXZ(PlayerContext.PlayerController.transform.position, 1f);
        spawnPos.y = 0f;
        Item item = Instantiate(inventoryItemView.ItemSO.GroundItemPrefab, spawnPos, Quaternion.identity).GetComponent<Item>();
        item.ItemSO = ItemSO;
        item.Quantity = 1;
        UpdateQuantity(-1);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        InventoryItemController.UpdateViewPosition(GetComponent<RectTransform>(), GetComponentInParent<ScrollRect>());
    }
}
