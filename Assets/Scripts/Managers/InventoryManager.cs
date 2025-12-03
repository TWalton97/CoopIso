using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static Action OnMenuOpened;
    public static Action OnMenuClosed;
    public GameObject PlayerPreview;

    public PlayerUserInterfaceController[] playerUserInterfaceControllers = new PlayerUserInterfaceController[2];

    public int PauseRequests = 0;


    public void RequestPause()
    {
        PauseRequests++;
        if (PauseRequests > 0)
        {
            Time.timeScale = 0;
            foreach (PlayerUserInterfaceController playerUserInterfaceController in playerUserInterfaceControllers)
            {
                playerUserInterfaceController.ToggleResourcePanel(false);
            }

            PlayerPreview.SetActive(true);
            OnMenuOpened?.Invoke();
        }
    }

    public void RequestUnpause()
    {
        PauseRequests--;
        if (PauseRequests <= 0)
        {
            PauseRequests = 0;
            Time.timeScale = 1;
            foreach (PlayerUserInterfaceController playerUserInterfaceController in playerUserInterfaceControllers)
            {
                playerUserInterfaceController.ToggleResourcePanel(true);
            }

            PlayerPreview.SetActive(false);
            OnMenuClosed?.Invoke();
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
