using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class MainMenuJoinManager : MonoBehaviour
{
    //When we spawn in a new player, we also spawn in a canvas and set the player
    public GameObject characterSelectionCanvas;
    public CharacterSelectUI characterSelectionUI;
    public MainMenuController mainMenuController;
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        InputSystemUIInputModule inputModule;
        CharacterSelectUI ui = Instantiate(characterSelectionUI, characterSelectionCanvas.transform);
        ui.mainMenuController = mainMenuController;
        inputModule = ui.GetComponent<InputSystemUIInputModule>();
        playerInput.uiInputModule = inputModule;
        ui.PlayerIndex = playerInput.playerIndex;
        Debug.Log($"Player input device is {playerInput.devices[0]}");
        mainMenuController.gameSetupData.Selections[playerInput.playerIndex].PlayerDevices = playerInput.devices[0];
        mainMenuController.gameSetupData.Selections[playerInput.playerIndex].PlayerControlSchemes = playerInput.currentControlScheme;
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {

    }
}
