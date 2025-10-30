using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem.UI;

public class PlayerJoinManager : Singleton<PlayerJoinManager>
{
    public PlayerInputManager playerInputManager;

    public static Action<GameObject> OnPlayerJoinedEvent;
    public static Action<GameObject> OnPlayerLeftEvent;

    private Dictionary<int, NewPlayerController> playerControllers = new();

    public InventoryManager inventoryManager;
    public GameObject player1UI;
    public GameObject player2UI;

    protected override void Awake()
    {
        base.Awake();

        playerInputManager = GetComponent<PlayerInputManager>();
    }

    public NewPlayerController GetPlayerControllerByIndex(int index)
    {
        playerControllers.TryGetValue(index, out NewPlayerController controller);
        return controller;
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("Player joined");
        CreatePlayerCanvas(playerInput);
        playerControllers.Add(playerInput.playerIndex, playerInput.GetComponent<NewPlayerController>());
        OnPlayerJoinedEvent?.Invoke(playerInput.gameObject);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        playerControllers.Remove(playerInput.playerIndex);
        OnPlayerLeftEvent?.Invoke(playerInput.gameObject);
    }

    private void CreatePlayerCanvas(PlayerInput playerInput)
    {
        InputSystemUIInputModule inputModule;
        InventoryController controller;
        switch (playerInput.playerIndex)
        {
            case 0:
                inputModule = Instantiate(player1UI, inventoryManager.transform).GetComponent<InputSystemUIInputModule>();
                playerInput.uiInputModule = inputModule;
                inventoryManager.EquipmentMenuObjects[playerInput.playerIndex].EquipmentMenuObject = inputModule.gameObject;
                controller = inputModule.GetComponent<InventoryController>();
                inventoryManager.EquipmentMenuObjects[playerInput.playerIndex].controller = controller;
                controller.playerIndex = playerInput.playerIndex;
                break;
            case 1:
                inputModule = Instantiate(player2UI, inventoryManager.transform).GetComponent<InputSystemUIInputModule>();
                playerInput.uiInputModule = inputModule;
                inventoryManager.EquipmentMenuObjects[playerInput.playerIndex].EquipmentMenuObject = inputModule.gameObject;
                controller = inputModule.GetComponent<InventoryController>();
                inventoryManager.EquipmentMenuObjects[playerInput.playerIndex].controller = controller;
                controller.playerIndex = playerInput.playerIndex;

                break;
        }
    }

}
