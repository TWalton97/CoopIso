using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerUserInterfaceController : MonoBehaviour
{
    public PlayerContext playerContext;

    public InventoryController inventoryController;
    public GameObject PlayerControlsPanel;
    public ResourcePanelController resourcePanelController;
    public AbilityScrollController AbilityScrollController;

    public VendorPanelController VendorPanelController;

    public TMP_Text GoldAmountText;
    public TMP_Text WeightText;

    public EventSystem eventSystem;

    public void Init(PlayerContext context)
    {
        eventSystem = GetComponent<EventSystem>();

        playerContext = context;

        resourcePanelController.Init(playerContext.PlayerController);
        inventoryController.Init(playerContext);

        UpdateGoldAmount(0);
        UpdateWeightAmount(0, 150);

        playerContext.PlayerController.PlayerInputController.OnDropItemPerformed += CallDropItemOnButton;
        playerContext.PlayerController.PlayerInputController.OnEquipOffhandPerformed += CallEquipOffhandOnButton;
    }

    void OnDestroy()
    {
        playerContext.PlayerController.PlayerInputController.OnDropItemPerformed -= CallDropItemOnButton;
        playerContext.PlayerController.PlayerInputController.OnEquipOffhandPerformed -= CallEquipOffhandOnButton;
    }

    private void CallDropItemOnButton(CallbackContext context)
    {
        if (eventSystem.currentSelectedGameObject.TryGetComponent(out ItemButton button))
        {
            button.OnDropItem(context);
        }
    }

    private void CallEquipOffhandOnButton(CallbackContext context)
    {
        if (eventSystem.currentSelectedGameObject.TryGetComponent(out ItemButton button))
        {
            button.OnEquipOffhand(context);
        }
    }

    public void ToggleInventory(InventoryMode inventoryMode = InventoryMode.Normal)
    {
        if (VendorPanelController.gameObject.activeSelf)
        {
            VendorPanelController.TogglePanel();
            return;
        }
        
        inventoryController.ChangeInventoryMode(inventoryMode);
        inventoryController.ToggleInventory();
    }

    public void ToggleBuyInventory(List<InventoryItemView> itemsForSale)
    {
        inventoryController.SetupBuyInventory(itemsForSale);
        inventoryController.ChangeInventoryMode(InventoryMode.Buy);
        inventoryController.ToggleInventory();
    }

    public void ToggleResourcePanel(bool value)
    {
        resourcePanelController.gameObject.SetActive(value);
    }

    public void TogglePlayerControlsPanel(bool value)
    {
        PlayerControlsPanel.SetActive(value);
    }

    public void ToggleVendorPanel(List<InventoryItemView> ItemsForSale)
    {
        VendorPanelController.TogglePanel(playerContext, ItemsForSale);
    }

    public void AddAbility(AbilitySO ability, AbilityBehaviourBase behaviour)
    {
        AbilityScrollController.AddAbility(ability, behaviour);
    }

    public void UpdateGoldAmount(int current)
    {
        GoldAmountText.text = current.ToString();
    }

    public void UpdateWeightAmount(float current, float max)
    {
        WeightText.text = current.ToString("0.0") + "/" + max.ToString("0.0");
    }

}
