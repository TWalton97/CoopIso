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

    }

    public void InitializeItemButton(InventoryItemController inventoryItemController, PotionSO potionData, string buttonID, PlayerContext playerContext)
    {
        InventoryItemController = inventoryItemController;
        ButtonID = buttonID;
        PlayerContext = playerContext;
        PotionData = potionData;

        Button button = GetComponent<Button>();
        button.onClick.AddListener(ActivateButton);

        ItemName.text = potionData.PotionName;
        ItemButtonImage.sprite = potionData.PotionSprite;
        ItemValue.text = potionData.GoldValue.ToString();
        ItemWeight.text = potionData.Weight.ToString("0.0");
        UpdateQuantity(1);

        PlayerContext.PlayerController.PlayerInputController.OnDropItemPerformed += OnDropItem;
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

    public override void ActivateButton()
    {

    }

    public override void OnLeftClick()
    {

    }

    public override void OnRightClick()
    {
        Vector3 spawnPos = NavMeshUtils.ReturnRandomPointOnXZ(PlayerContext.PlayerController.transform.position, 1f);
        spawnPos.y = 0f;
        Instantiate(PotionData.PotionPrefab, spawnPos, Quaternion.identity);
        UpdateQuantity(-1);
    }
}
