using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;

public class PlayerJoinManager : Singleton<PlayerJoinManager>
{
    public PlayerAveragePositionTracker playerAveragePositionTracker;
    public PlayerInputManager playerInputManager;
    public InteractionManager interactionManager;
    public InventoryManager inventoryManager;
    public SpawnedItemDataBase spawnedItemDatabase;
    public PlayerPreviewManager playerPreviewManager;

    public static Action<GameObject> OnPlayerJoinedEvent;
    public static Action<GameObject> OnPlayerLeftEvent;

    private Dictionary<int, NewPlayerController> playerControllers = new();

    public GameObject player1UI;
    public GameObject player2UI;

    public List<ClassPresetSO> classPresets;

    public MainMenuController mainMenuController;


    protected override void Awake()
    {
        base.Awake();

        playerInputManager = GetComponent<PlayerInputManager>();
        mainMenuController = FindObjectOfType<MainMenuController>();
        if (mainMenuController != null)
            SpawnPlayers();
    }

    public NewPlayerController GetPlayerControllerByIndex(int index)
    {
        playerControllers.TryGetValue(index, out NewPlayerController controller);
        return controller;
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        playerControllers.Add(playerInput.playerIndex, playerInput.GetComponent<NewPlayerController>());
        CreatePlayerCanvas(playerInput);
        OnPlayerJoinedEvent?.Invoke(playerInput.gameObject);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        playerControllers.Remove(playerInput.playerIndex);
        OnPlayerLeftEvent?.Invoke(playerInput.gameObject);
    }

    public void SpawnPlayers()
    {
        var data = mainMenuController.gameSetupData;
        int playerCount = data.PlayerCount;

        for (int i = 0; i < playerCount; i++)
        {
            InputDevice device = data.Selections[i].PlayerDevices;
            string scheme = data.Selections[i].PlayerControlSchemes;

            PlayerInput playerInput = playerInputManager.JoinPlayer(i, -1, scheme, device);
            playerInput.gameObject.transform.position = NavMeshUtils.ReturnRandomPointOnXZ(Vector3.zero, 4f);
        }
    }

    private void CreatePlayerCanvas(PlayerInput playerInput)
    {
        InputSystemUIInputModule inputModule;
        PlayerUserInterfaceController playerUserInterfaceController = null;
        PlayerContext playerContext;
        switch (playerInput.playerIndex)
        {
            case 0:
                //Setting up the player input module and sending it to the player's player input component
                inputModule = Instantiate(player1UI, inventoryManager.transform).GetComponent<InputSystemUIInputModule>();
                playerInput.uiInputModule = inputModule;

                playerUserInterfaceController = inputModule.GetComponent<PlayerUserInterfaceController>();
                break;
            case 1:
                inputModule = Instantiate(player2UI, inventoryManager.transform).GetComponent<InputSystemUIInputModule>();
                playerInput.uiInputModule = inputModule;

                playerUserInterfaceController = inputModule.GetComponent<PlayerUserInterfaceController>();
                break;
        }

        playerContext = new PlayerContext();
        playerContext.PlayerIndex = playerInput.playerIndex;
        playerContext.UserInterfaceController = playerUserInterfaceController;
        playerContext.PlayerController = GetPlayerControllerByIndex(playerInput.playerIndex);
        playerContext.PlayerInput = playerInput;
        playerContext.InventoryManager = inventoryManager;
        playerContext.InteractionManager = interactionManager;
        playerContext.SpawnedItemDatabase = spawnedItemDatabase;
        playerContext.PlayerPreviewManager = playerPreviewManager;

        playerContext.UserInterfaceController.inventoryController.controller = playerContext.PlayerController;

        playerContext.PlayerController.PlayerContext = playerContext;

        playerContext.PlayerController.Init();

        ClassPresetSO classPresetSO;
        if (mainMenuController != null)
        {
            classPresetSO = mainMenuController.gameSetupData.chosenClassPresets[playerInput.playerIndex];
        }
        else
        {
            classPresetSO = ChooseRandomClassPreset();
        }


        playerContext.PlayerController.FeatsController.SetupClassPreset(classPresetSO.classFeatConfig);
        playerContext.PlayerController.WeaponController.EquipStarterItems(classPresetSO.StartingMainHandWeapon, classPresetSO.StartingOffhandWeapon);
        playerContext.PlayerController.ArmorController.EquipStarterItems(classPresetSO.StartingHelmet, classPresetSO.StartingBodyArmor, classPresetSO.StartingLegArmor);
        playerContext.PlayerController.PlayerStatsBlackboard.ClassName = classPresetSO.PresetName;

        playerUserInterfaceController.Init(playerInput, playerContext);

        playerAveragePositionTracker.AddPlayer(playerInput.gameObject);
    }

    private ClassPresetSO ChooseRandomClassPreset()
    {
        if (classPresets.Count == 0) return null;

        return classPresets[UnityEngine.Random.Range(0, classPresets.Count)];
    }
}

[Serializable]
public class PlayerContext
{
    public int PlayerIndex;
    public PlayerUserInterfaceController UserInterfaceController;
    public NewPlayerController PlayerController;
    public PlayerInput PlayerInput;

    public InventoryManager InventoryManager;
    public InteractionManager InteractionManager;
    public SpawnedItemDataBase SpawnedItemDatabase;
    public PlayerPreviewManager PlayerPreviewManager;
}
