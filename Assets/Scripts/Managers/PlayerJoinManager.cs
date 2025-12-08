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
    public UIStateManager UIStateManager;

    public static Action<GameObject> OnPlayerJoinedEvent;
    public static Action<GameObject> OnPlayerLeftEvent;

    public Dictionary<int, NewPlayerController> playerControllers = new();

    public InputSystemUIInputModule player1UI;
    public InputSystemUIInputModule player2UI;

    public MainMenuController mainMenuController;

    public int TargetSpawnID;

    public GameStateData LoadGameStateData;
    protected override void Awake()
    {
        base.Awake();
        playerInputManager = GetComponent<PlayerInputManager>();
        mainMenuController = FindObjectOfType<MainMenuController>();
        sceneLoadingManager = SceneLoadingManager.Instance;
        sceneLoadingManager.OnUnloadingStarted += DisablePlayerGravity;
        sceneLoadingManager.OnSceneGroupLoaded += EnablePlayerGravity;

        if (mainMenuController != null)
        {
            if (mainMenuController.gameLoadMode == GameLoadMode.NewGame)
            {
                StartNewGame();
            }
            else
            {
                LoadSavedGame();
            }
        }
    }

    void OnDisable()
    {
        sceneLoadingManager.OnUnloadingStarted -= DisablePlayerGravity;
        sceneLoadingManager.OnSceneGroupLoaded -= EnablePlayerGravity;
    }

    private void StartNewGame()
    {
        SpawnPlayers();
    }

    private void LoadSavedGame()
    {
        LoadGameStateData = mainMenuController.GameStateDataToLoad;

        InitializeSpawnedItemDataBase();

        foreach (var savedPlayer in LoadGameStateData.PlayerStateDatas)
        {
            PlayerInput spawned = playerInputManager.JoinPlayer(savedPlayer.playerIndex, -1);
            var controller = spawned.GetComponent<NewPlayerController>();
            controller.gameObject.transform.position = NavMeshUtils.ReturnRandomPointOnXZ(Vector3.zero, 4f);
        }
    }

    private void InitializeSpawnedItemDataBase()
    {
        foreach (ItemDataSaveEntry itemData in LoadGameStateData.SpawnedItemData)
        {
            ItemSO itemSO = ItemDatabase.GetItemSO(itemData.ItemSO_ID);
            ItemData newItemData = itemData.itemData;
            newItemData.ItemSO = itemSO;
            spawnedItemDatabase.spawnedItemData.Add(newItemData.ItemID, newItemData);
        }

        Debug.Log("SpawnedItemBase initialized with load data");
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
        switch (playerInput.playerIndex)
        {
            case 0:
                playerInput.uiInputModule = player1UI;
                player1UI.gameObject.SetActive(true);
                playerUserInterfaceController = player1UI.GetComponent<PlayerUserInterfaceController>();
                break;
            case 1:
                playerInput.uiInputModule = player2UI;
                player2UI.gameObject.SetActive(true);
                playerUserInterfaceController = player2UI.GetComponent<PlayerUserInterfaceController>();
                break;
        }

        if (mainMenuController.gameLoadMode == GameLoadMode.NewGame)
        {
            SetupNewPlayer(playerUserInterfaceController, playerInput);
        }
        else
        {
            SetupLoadedPlayer(playerUserInterfaceController, playerInput);
        }

        //playerAveragePositionTracker.AddPlayer(playerInput.gameObject);
    }

    public void SetupLoadedPlayer(PlayerUserInterfaceController playerUserInterfaceController, PlayerInput playerInput)
    {
        PlayerContext playerContext;
        PlayerStateData playerStateData = LoadGameStateData.PlayerStateDatas[playerInput.playerIndex];
        playerContext = new PlayerContext();
        playerContext.PlayerIndex = playerInput.playerIndex;
        ClassPresetSO classPreset = ClassPresetDatabase.GetClassPreset(playerStateData.classPresetID);
        Debug.Log($"Loaded class preset is {classPreset.PresetName}");
        playerContext.PlayerClassPreset = classPreset;
        playerContext.UserInterfaceController = playerUserInterfaceController;
        playerContext.PlayerController = GetPlayerControllerByIndex(playerInput.playerIndex);
        playerContext.PlayerInput = playerInput;
        playerContext.InventoryManager = inventoryManager;
        playerContext.InteractionManager = interactionManager;
        playerContext.SpawnedItemDatabase = spawnedItemDatabase;
        playerContext.PlayerPreviewManager = playerPreviewManager;
        playerContext.InventoryController = playerUserInterfaceController.inventoryController;
        playerContext.UIStateManager = UIStateManager;

        playerUserInterfaceController.playerContext = playerContext;

        playerContext.PlayerController.PlayerContext = playerContext;

        playerContext.PlayerController.Init();

        playerUserInterfaceController.Init(playerContext);

        List<RuntimeFeat> runtimeFeats = new();
        foreach (RuntimeFeatSaveData rfsd in playerStateData.unlockedFeats)
        {
            RuntimeFeat runtimeFeat = new RuntimeFeat(FeatPresetDatabase.GetFeat(rfsd.featID));
            runtimeFeat.CurrentFeatLevel = rfsd.currentLevel;
            runtimeFeats.Add(runtimeFeat);
        }
        playerContext.PlayerController.FeatsController.SetupLoadedCharacter(runtimeFeats);

        foreach (ItemSaveData isd in playerStateData.weapons)
        {
            playerContext.InventoryController.AddItemToInventory(spawnedItemDatabase.GetSpawnedItemDataFromDataBase(isd.itemID), isd.isEquipped);
        }

        foreach (ItemSaveData isd in playerStateData.armor)
        {
            playerContext.InventoryController.AddItemToInventory(spawnedItemDatabase.GetSpawnedItemDataFromDataBase(isd.itemID), isd.isEquipped);
        }

        foreach (ConsumableSaveData csd in playerStateData.misc)
        {
            //Get the ItemSO
            ItemSO so = ItemDatabase.GetItemSO(csd.ItemSO_ID);
            playerContext.InventoryController.AddConsumableToInventory(so, csd.quantity);
        }

        playerContext.PlayerController.PlayerStatsBlackboard.ClassName = classPreset.PresetName;
        playerContext.PlayerController.EntityData = classPreset.PlayerStatsSO;
        playerContext.PlayerController.ApplyStats();
        playerContext.UserInterfaceController.inventoryController.FeatsMenu.CreateFeatButtons(playerContext);
    }

    public void SetupNewPlayer(PlayerUserInterfaceController playerUserInterfaceController, PlayerInput playerInput)
    {
        PlayerContext playerContext;
        playerContext = new PlayerContext();
        playerContext.PlayerIndex = playerInput.playerIndex;
        playerContext.PlayerClassPreset = mainMenuController.gameSetupData.chosenClassPresets[playerInput.playerIndex];
        playerContext.UserInterfaceController = playerUserInterfaceController;
        playerContext.PlayerController = GetPlayerControllerByIndex(playerInput.playerIndex);
        playerContext.PlayerInput = playerInput;
        playerContext.InventoryManager = inventoryManager;
        playerContext.InteractionManager = interactionManager;
        playerContext.SpawnedItemDatabase = spawnedItemDatabase;
        playerContext.PlayerPreviewManager = playerPreviewManager;
        playerContext.InventoryController = playerUserInterfaceController.inventoryController;
        playerContext.UIStateManager = UIStateManager;

        playerUserInterfaceController.playerContext = playerContext;

        playerContext.PlayerController.PlayerContext = playerContext;

        playerContext.PlayerController.Init();

        ClassPresetSO classPresetSO = mainMenuController.gameSetupData.chosenClassPresets[playerInput.playerIndex];

        playerUserInterfaceController.Init(playerContext);

        playerContext.PlayerController.FeatsController.SetupClassPreset(classPresetSO.classFeatConfig);
        playerContext.PlayerController.WeaponController.EquipStarterItems(classPresetSO.StartingMainHandWeapon, classPresetSO.StartingOffhandWeapon);
        playerContext.PlayerController.ArmorController.EquipStarterItems(classPresetSO.StartingHelmet, classPresetSO.StartingBodyArmor, classPresetSO.StartingLegArmor);
        playerContext.PlayerController.PlayerStatsBlackboard.ClassName = classPresetSO.PresetName;
        playerContext.PlayerController.EntityData = classPresetSO.PlayerStatsSO;
        if (classPresetSO.StartingConsumables.Count > 0)
        {
            foreach (ItemSO consumable in classPresetSO.StartingConsumables)
            {
                ItemData itemData = new ItemData();
                itemData.ItemSO = consumable;
                itemData.ItemID = spawnedItemDatabase.RegisterItemToDatabase(itemData);
                playerContext.UserInterfaceController.inventoryController.AddItemToInventory(itemData);
            }
        }
        playerContext.PlayerController.ApplyStats();

        playerContext.UserInterfaceController.inventoryController.FeatsMenu.CreateFeatButtons(playerContext);
    }
}

[Serializable]
public class PlayerContext
{
    public int PlayerIndex;
    public ClassPresetSO PlayerClassPreset;
    public PlayerUserInterfaceController UserInterfaceController;
    public NewPlayerController PlayerController;
    public PlayerInput PlayerInput;

    public InventoryManager InventoryManager;
    public InteractionManager InteractionManager;
    public SpawnedItemDataBase SpawnedItemDatabase;
    public PlayerPreviewManager PlayerPreviewManager;
    public InventoryController InventoryController;
    public UIStateManager UIStateManager;
}
