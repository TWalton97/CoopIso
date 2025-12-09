using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
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

    public MultiplayerEventSystem eventSystem;

    public PlayerUIState playerUIState = PlayerUIState.None;

    public void Init(PlayerContext context)
    {
        eventSystem = GetComponent<MultiplayerEventSystem>();

        playerContext = context;

        resourcePanelController.Init(playerContext.PlayerController);
        inventoryController.Init(playerContext);

        UpdateGoldAmount(0);
        UpdateWeightAmount(0, 150);

        playerContext.PlayerController.PlayerInputController.OnDropItemPerformed += CallDropItemOnButton;
        playerContext.PlayerController.PlayerInputController.OnEquipOffhandPerformed += CallEquipOffhandOnButton;
        playerContext.PlayerController.PlayerInputController.OnCancelPerformed += OnCancel;
        VendorPanelController.PlayerContext = playerContext;
    }

    void OnEnable()
    {

    }

    void OnDestroy()
    {
        playerContext.PlayerController.PlayerInputController.OnDropItemPerformed -= CallDropItemOnButton;
        playerContext.PlayerController.PlayerInputController.OnEquipOffhandPerformed -= CallEquipOffhandOnButton;
        playerContext.PlayerController.PlayerInputController.OnCancelPerformed -= OnCancel;
    }

    private void OnCancel(CallbackContext context)
    {
        switch (playerUIState)
        {
            case PlayerUIState.Inventory_Normal:
                CloseInventory();
                break;
            case PlayerUIState.Vendor_Buy:
                CloseInventoryToVendorMenu();
                break;
            case PlayerUIState.Vendor_Sell:
                CloseInventoryToVendorMenu();
                break;
            case PlayerUIState.Vendor_Menu:
                VendorPanelController.Exit();
                break;
            case PlayerUIState.Vendor_Craft:
                CloseInventoryToVendorMenu();
                break;
        }
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

    public void TryToggleInventory()
    {
        if (playerUIState == PlayerUIState.None)
        {
            playerContext.PlayerController.PlayerInputController.EnableUIActionMap();
            OpenInventoryInMode(InventoryMode.Normal);
        }
        else if (playerUIState == PlayerUIState.Inventory_Normal)
        {
            playerContext.PlayerController.PlayerInputController.EnablePlayerActionMap();
            CloseInventory();
        }
        else if (playerUIState == PlayerUIState.Vendor_Waiting || playerUIState == PlayerUIState.Vendor_Menu)
        {
            //Do nothing
        }
        else
        {
            CloseInventoryToVendorMenu();
        }
    }

    public void OpenInventoryInMode(InventoryMode inventoryMode)
    {
        inventoryController.ChangeInventoryMode(inventoryMode);
        inventoryController.OpenInventory();

        switch (inventoryMode)
        {
            case InventoryMode.Normal:
                playerUIState = PlayerUIState.Inventory_Normal;
                playerContext.InventoryManager.RequestPause();
                break;
            case InventoryMode.Buy:
                playerUIState = PlayerUIState.Vendor_Buy;
                break;
            case InventoryMode.Sell:
                playerUIState = PlayerUIState.Vendor_Sell;
                break;
        }
    }

    public void CloseInventory()
    {
        inventoryController.ChangeInventoryMode(InventoryMode.Normal);
        inventoryController.CloseInventory();
        playerContext.InventoryManager.RequestUnpause();
        playerUIState = PlayerUIState.None;
    }

    public void CloseInventoryToVendorMenu()
    {
        CloseInventory();
        OpenVendorMenu();
    }

    public void OpenVendorMenu()
    {
        VendorPanelController.OpenVendorMenu();
        playerContext.PlayerController.PlayerInputController.EnableUIActionMap();
        playerUIState = PlayerUIState.Vendor_Menu;
    }

    public void CloseVendorMenu()
    {
        VendorPanelController.CloseVendorMenu();
        playerContext.PlayerController.PlayerInputController.EnablePlayerActionMap();
        playerUIState = PlayerUIState.None;
    }


    public void ToggleResourcePanel(bool value)
    {
        resourcePanelController.gameObject.SetActive(value);
    }

    public void TogglePlayerControlsPanel(bool value)
    {
        PlayerControlsPanel.SetActive(value);
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

public enum PlayerUIState
{
    None,
    Inventory_Normal,
    Vendor_Waiting,
    Vendor_Menu,
    Vendor_Buy,
    Vendor_Sell,
    Vendor_Craft
}
