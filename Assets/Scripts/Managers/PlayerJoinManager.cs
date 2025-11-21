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
        PlayerUserInterfaceController playerUserInterfaceController;
        switch (playerInput.playerIndex)
        {
            case 0:
                //Setting up the player input module and sending it to the player's player input component
                inputModule = Instantiate(player1UI, inventoryManager.transform).GetComponent<InputSystemUIInputModule>();
                playerInput.uiInputModule = inputModule;

                //Setting up the player inventory controller and giving it the appropriate player index
                // inventoryManager.EquipmentMenuObjects[playerInput.playerIndex].EquipmentMenuObject = inputModule.gameObject;
                // controller = inputModule.GetComponentInChildren<InventoryController>();
                // inventoryManager.EquipmentMenuObjects[playerInput.playerIndex].controller = controller;
                // controller.playerIndex = playerInput.playerIndex;

                //Setting up the player feats panel

                //We should do this setup through the PlayerUserInterfaceController
                playerUserInterfaceController = inputModule.GetComponent<PlayerUserInterfaceController>();
                playerUserInterfaceController.Init(playerInput);
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
