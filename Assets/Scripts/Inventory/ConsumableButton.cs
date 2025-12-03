using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsumableButton : ItemButton
{
    public int Quantity = 0;
    public TMP_Text QuantityText;

    public PotionSO PotionData;

    public override void InitializeItemButton(InventoryItemController inventoryItemController, ItemData itemData, string buttonID, PlayerContext playerContext, bool isEquipped = false)
    {
        InventoryItemController = inventoryItemController;
        ButtonID = buttonID;
        PlayerContext = playerContext;
        ItemData = itemData;
        PotionData = itemData.ItemSO as PotionSO;

        Button button = GetComponent<Button>();
        button.onClick.AddListener(ActivateButton);

        ItemName.text = PotionData.PotionName;
        ItemButtonImage.sprite = PotionData.PotionSprite;
        ItemValue.text = PotionData.GoldValue.ToString();
        ItemWeight.text = PotionData.Weight.ToString("0.0");
        CheckIfButtonCanBeActivated();
        UpdateQuantity(1);
    }

    public void InitializeItemButton(InventoryItemController inventoryItemController, PotionSO potionData, string buttonID, PlayerContext playerContext)
    {

    }

    void OnEnable()
    {
        CheckIfButtonCanBeActivated();
    }

    public void UpdateQuantity(int amount)
    {
        Quantity += amount;
        if (Quantity <= 0)
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
            if (ItemData.ItemSO.GoldValue > PlayerContext.PlayerController.PlayerStatsBlackboard.GoldAmount)
            {
                SetBackgroundColor(CannotBeUsedColor);
            }
            else
            {
                SetBackgroundColor(BaseColor);
            }
        }
    }

    public override void ActivateButton()
    {

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

        InventoryItemController.PlayerContext.PlayerController.PlayerStatsBlackboard.AddGold(-ItemData.ItemSO.GoldValue);
        InventoryItemController.CreateButtonForItem(ItemData);
        InventoryItemController.RemoveBuyButtonAtID(ButtonID);
    }

    public void OnLeftClickSellMode()
    {
        InventoryItemController.PlayerContext.PlayerController.PlayerStatsBlackboard.AddGold(ItemData.ItemSO.GoldValue);
        UpdateQuantity(-1);
    }

    public override void OnRightClick()
    {
        Vector3 spawnPos = NavMeshUtils.ReturnRandomPointOnXZ(PlayerContext.PlayerController.transform.position, 1f);
        spawnPos.y = 0f;
        Item item = Instantiate(PotionData.PotionPrefab, spawnPos, Quaternion.identity).GetComponent<Item>();
        item.itemData = ItemData;
        UpdateQuantity(-1);
    }
}
