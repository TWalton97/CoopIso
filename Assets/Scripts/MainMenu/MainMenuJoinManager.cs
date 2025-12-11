using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class MainMenuJoinManager : MonoBehaviour
{
    //When we spawn in a new player, we also spawn in a canvas and set the player
    public GameObject characterSelectionCanvas;
    public CharacterSelectUI[] characterSelectionUI;
    public List<PlayerInput> PlayerInputs;
    public MainMenuNavigationController mainMenuNavigationController;
    public PlaySessionData playSessionData;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        PlayerInputs.Add(playerInput);
        playerInput.transform.parent = transform;
        // InputSystemUIInputModule inputModule;
        CharacterSelectUI ui = characterSelectionUI[playerInput.playerIndex];
        ui.mainMenuNavigationController = mainMenuNavigationController;
        // inputModule = ui.GetComponent<InputSystemUIInputModule>();
        // playerInput.uiInputModule = inputModule;
        ui.PlayerIndex = playerInput.playerIndex;
        playSessionData.gameSetupData.Selections[playerInput.playerIndex].PlayerDevices = playerInput.devices[0];
        playSessionData.gameSetupData.Selections[playerInput.playerIndex].PlayerControlSchemes = playerInput.currentControlScheme;
        characterSelectionUI[playerInput.playerIndex].EnableSelectionButtons();
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {

    }
}
