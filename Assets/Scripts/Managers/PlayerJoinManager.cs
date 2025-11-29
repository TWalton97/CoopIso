using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem.UI;
using Cinemachine;
using System.Collections;

public class PlayerJoinManager : Singleton<PlayerJoinManager>
{
    //public PlayerAveragePositionTracker playerAveragePositionTracker;
    public PlayerInputManager playerInputManager;
    public InteractionManager interactionManager;
    public InventoryManager inventoryManager;
    public SpawnedItemDataBase spawnedItemDatabase;
    public PlayerPreviewManager playerPreviewManager;
    public SceneLoadingManager sceneLoadingManager;
    public CullingManager cullingManager;
    public PlayerAveragePositionTracker playerAveragePositionTracker;

    public static Action<GameObject> OnPlayerJoinedEvent;
    public static Action<GameObject> OnPlayerLeftEvent;

    public Dictionary<int, NewPlayerController> playerControllers = new();

    public InputSystemUIInputModule player1UI;
    public InputSystemUIInputModule player2UI;

    public List<ClassPresetSO> classPresets;

    public MainMenuController mainMenuController;

    public int TargetSpawnID;
    protected override void Awake()
    {
        base.Awake();
        playerInputManager = GetComponent<PlayerInputManager>();
        mainMenuController = FindObjectOfType<MainMenuController>();
        sceneLoadingManager = SceneLoadingManager.Instance;
        sceneLoadingManager.OnUnloadingStarted += DisablePlayerGravity;
        sceneLoadingManager.OnSceneGroupLoaded += EnablePlayerGravity;
        if (mainMenuController != null)
            SpawnPlayers();
    }

    void OnEnable()
    {

    }

    void OnDisable()
    {
        sceneLoadingManager.OnUnloadingStarted -= DisablePlayerGravity;
        sceneLoadingManager.OnSceneGroupLoaded -= EnablePlayerGravity;
    }

    private void DisablePlayerGravity()
    {
        playerAveragePositionTracker.DisableLeashing = true;
        cullingManager.CanCull = false;
        for (int i = 0; i < playerControllers.Count; i++)
        {
            playerControllers[i].GetComponent<Rigidbody>().useGravity = false;
        }
    }

    private void EnablePlayerGravity()
    {
        SpawnPoint targetSpawnPoint = null;

        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.SpawnID == TargetSpawnID)
            {
                targetSpawnPoint = spawnPoint;
            }
        }

        for (int i = 0; i < playerControllers.Count; i++)
        {
            if (targetSpawnPoint != null)
            {
                playerControllers[i].transform.SetPositionAndRotation(targetSpawnPoint.transform.GetChild(i).position, targetSpawnPoint.transform.GetChild(i).rotation);
            }
            else
            {
                playerControllers[i].transform.position = Vector3.zero;
            }

            playerControllers[i].GetComponent<Rigidbody>().useGravity = true;
        }

        StartCoroutine(EnableCulling());
    }

    private IEnumerator EnableCulling()
    {
        yield return new WaitForSeconds(0.1f);
        cullingManager.CanCull = true;
        cullingManager.InitialStateCheck();
        playerAveragePositionTracker.DisableLeashing = false;
        yield return null;
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
        //InputSystemUIInputModule inputModule;
        PlayerUserInterfaceController playerUserInterfaceController = null;
        PlayerContext playerContext;
        switch (playerInput.playerIndex)
        {
            case 0:
                //Setting up the player input module and sending it to the player's player input component
                //inputModule = Instantiate(player1UI, inventoryManager.transform).GetComponent<InputSystemUIInputModule>();
                playerInput.uiInputModule = player1UI;
                player1UI.gameObject.SetActive(true);
                playerUserInterfaceController = player1UI.GetComponent<PlayerUserInterfaceController>();
                break;
            case 1:
                playerInput.uiInputModule = player2UI;
                player2UI.gameObject.SetActive(true);
                playerUserInterfaceController = player2UI.GetComponent<PlayerUserInterfaceController>();
                // inputModule = Instantiate(player2UI, inventoryManager.transform).GetComponent<InputSystemUIInputModule>();
                // playerInput.uiInputModule = inputModule;

                // playerUserInterfaceController = inputModule.GetComponent<PlayerUserInterfaceController>();
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
        playerContext.InventoryController = playerUserInterfaceController.inventoryController;

        playerUserInterfaceController.playerContext = playerContext;

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

        playerUserInterfaceController.Init(playerContext);

        playerContext.PlayerController.FeatsController.SetupClassPreset(classPresetSO.classFeatConfig);
        playerContext.PlayerController.WeaponController.EquipStarterItems(classPresetSO.StartingMainHandWeapon, classPresetSO.StartingOffhandWeapon);
        playerContext.PlayerController.ArmorController.EquipStarterItems(classPresetSO.StartingHelmet, classPresetSO.StartingBodyArmor, classPresetSO.StartingLegArmor);
        playerContext.PlayerController.PlayerStatsBlackboard.ClassName = classPresetSO.PresetName;
        playerContext.PlayerController.EntityData = classPresetSO.PlayerStatsSO;
        playerContext.PlayerController.ApplyStats();

        playerContext.UserInterfaceController.inventoryController.FeatsMenu.CreateFeatButtons(playerContext);

        //playerAveragePositionTracker.AddPlayer(playerInput.gameObject);
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
    public InventoryController InventoryController;
}
