using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static Action OnMenuOpened;
    public static Action OnMenuClosed;
    public GameObject PlayerPreview;

    public PlayerUserInterfaceController[] playerUserInterfaceControllers = new PlayerUserInterfaceController[2];

    private bool player0MenuOpened;
    private bool player1MenuOpened;

    public void OpenInventory(int playerIndex)
    {
        if (playerUserInterfaceControllers[playerIndex].IsMenuOpened)
        {
            playerUserInterfaceControllers[playerIndex].ToggleInventory(false);
            if (playerIndex == 0) player0MenuOpened = false;
            if (playerIndex == 1) player1MenuOpened = false;

            if (!player0MenuOpened && !player1MenuOpened)
            {
                Time.timeScale = 1;

                foreach (PlayerUserInterfaceController playerUserInterfaceController in playerUserInterfaceControllers)
                {
                    playerUserInterfaceController.ToggleResourcePanel(true);
                }

                PlayerPreview.SetActive(false);
                OnMenuClosed?.Invoke();
            }
        }
        else
        {
            playerUserInterfaceControllers[playerIndex].ToggleInventory(true);

            if (playerIndex == 0) player0MenuOpened = true;
            if (playerIndex == 1) player1MenuOpened = true;
            Time.timeScale = 0;
            foreach (PlayerUserInterfaceController playerUserInterfaceController in playerUserInterfaceControllers)
            {
                playerUserInterfaceController.ToggleResourcePanel(false);
            }
            PlayerPreview.SetActive(true);
            OnMenuOpened?.Invoke();
        }
    }
}

public enum ItemType
{
    Consumable,
    Head,
    Body,
    Legs,
    OneHanded,
    TwoHanded,
    Offhand,
    Bow
};
