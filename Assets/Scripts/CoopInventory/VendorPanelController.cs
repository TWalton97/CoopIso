using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class VendorPanelController : MonoBehaviour
{
    public PlayerContext PlayerContext;
    public GameObject ButtonPanel;
    public GameObject WaitingForPlayerPanel;
    public UIButton[] Buttons;
    public List<InventoryItemView> ItemsForSale;

    private float nextAllowedInputTime = 0f;

    void Awake()
    {
        PlayerContext.PlayerController.PlayerInputController.OnSubmitPerformed += OnSubmit;
    }

    void OnDestroy()
    {
        PlayerContext.PlayerController.PlayerInputController.OnSubmitPerformed -= OnSubmit;
    }

    private void OnSubmit(CallbackContext context)
    {
        if (PlayerContext.UserInterfaceController.playerUIState == PlayerUIState.Vendor_Waiting)
        {
            Enter();
        }
    }

    public void OpenVendorMenu()
    {
        gameObject.SetActive(true);
        ButtonPanel.gameObject.SetActive(true);
        WaitingForPlayerPanel.gameObject.SetActive(false);
        PlayerContext.UserInterfaceController.eventSystem.SetSelectedGameObject(Buttons[0].gameObject);
        PlayerContext.InventoryManager.RequestPause();
    }

    public void CloseVendorMenu()
    {
        gameObject.SetActive(false);
        foreach (UIButton button in Buttons)
        {
            button.ToggleHighlight(false);
        }
    }

    public void EnterBuyMode()
    {
        if (Time.unscaledTime < nextAllowedInputTime) return;
        nextAllowedInputTime = Time.unscaledTime + 0.15f;
        PlayerContext.InventoryController.SetupBuyInventory(ItemsForSale);
        PlayerContext.UserInterfaceController.OpenInventoryInMode(InventoryMode.Buy);
        CloseVendorMenu();
    }

    public void EnterSellMode()
    {
        if (Time.unscaledTime < nextAllowedInputTime) return;
        nextAllowedInputTime = Time.unscaledTime + 0.15f;
        PlayerContext.UserInterfaceController.OpenInventoryInMode(InventoryMode.Sell);
        CloseVendorMenu();
    }

    public void Exit()
    {
        if (Time.unscaledTime < nextAllowedInputTime) return;
        nextAllowedInputTime = Time.unscaledTime + 0.15f;
        PlayerContext.UserInterfaceController.playerUIState = PlayerUIState.Vendor_Waiting;
        ButtonPanel.gameObject.SetActive(false);
        WaitingForPlayerPanel.gameObject.SetActive(true);
        PlayerContext.InventoryManager.RequestUnpause();
    }

    public void Enter()
    {
        if (Time.unscaledTime < nextAllowedInputTime) return;
        nextAllowedInputTime = Time.unscaledTime + 0.15f;
        PlayerContext.UserInterfaceController.playerUIState = PlayerUIState.Vendor_Menu;
        ButtonPanel.gameObject.SetActive(true);
        WaitingForPlayerPanel.gameObject.SetActive(false);
        PlayerContext.InventoryManager.RequestPause();
    }
}
