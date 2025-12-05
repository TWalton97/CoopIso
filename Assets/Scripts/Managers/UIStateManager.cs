using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
public class UIStateManager : Singleton<UIStateManager>
{
    public InventoryManager InventoryManager;
    public enum GlobalUIState { None, Inventory, Vendor }
    public GlobalUIState CurrentState { get; private set; } = GlobalUIState.None;

    private bool[] playerInInventory = new bool[2];
    private bool[] playerInVendor = new bool[2];

    public GameObject[] InventoryPanels;
    public GameObject[] VendorPanels;

    public static Action OnMenuOpened;
    public static Action OnMenuClosed;

    public void PlayerOpenedInventory(int playerIndex)
    {
        playerInInventory[playerIndex] = true;

        if (CurrentState == GlobalUIState.None)
        {
            CurrentState = GlobalUIState.Inventory;
            InventoryManager.RequestPause();
            OnMenuOpened?.Invoke();
            foreach (GameObject obj in InventoryPanels)
            {
                obj.SetActive(true);
            }
        }
    }

    public void PlayerClosedInventory(int playerIndex)
    {
        playerInInventory[playerIndex] = false;

        if (!playerInInventory[0] && !playerInInventory[1])
        {
            CurrentState = GlobalUIState.None;
            InventoryManager.RequestUnpause();
            OnMenuClosed?.Invoke();
            foreach (GameObject obj in InventoryPanels)
            {
                obj.SetActive(false);
            }
        }
    }

    public void PlayerOpenedVendor(int playerIndex)
    {
        playerInVendor[playerIndex] = true;

        if (CurrentState != GlobalUIState.Vendor)
        {
            CurrentState = GlobalUIState.Vendor;
            InventoryManager.RequestPause();
            OnMenuOpened?.Invoke();
            foreach (GameObject obj in VendorPanels)
            {
                obj.SetActive(true);
            }
        }
    }

    public void PlayerExitedVendor(int playerIndex)
    {
        playerInVendor[playerIndex] = false;

        if (!playerInVendor[0] && !playerInVendor[1])
        {
            CurrentState = GlobalUIState.None;
            InventoryManager.RequestUnpause();
            OnMenuClosed?.Invoke();
            foreach (GameObject obj in VendorPanels)
            {
                obj.SetActive(false);
            }
        }
    }

}