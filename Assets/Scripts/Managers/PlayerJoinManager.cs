using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem.UI;
using Cinemachine;
using System.Collections;
using System.Linq;
using UnityEngine.InputSystem.Controls;

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

    public LoadMenuManager LoadMenuManager;

    public InputSystemUIInputModule player1UI;
    public InputSystemUIInputModule player2UI;

    public PlaySessionData playSessionData;

    public int TargetSpawnID;

    public GameStateData LoadGameStateData;
    private bool firstLoad = false;

    public bool player2Loaded = false;
    public bool awaitingSecondPlayer;

    protected override void Awake()
    {
        base.Awake();
        playSessionData = PlaySessionData.Instance;
        playerInputManager = GetComponent<PlayerInputManager>();
        sceneLoadingManager = SceneLoadingManager.Instance;
        sceneLoadingManager.OnUnloadingStarted += DisablePlayerGravity;
        sceneLoadingManager.OnSceneGroupLoaded += EnablePlayerGravity;

        if (playSessionData != null)
        {
            if (playSessionData.PlaySessionLoadMode == GameLoadMode.NewGame)
            {
                StartNewGame();
            }
            else
            {
                firstLoad = true;
                LoadSavedGame();
            }
        }

        playerAveragePositionTracker.DisableLeashing = true;
    }

    void OnDisable()
    {
        sceneLoadingManager.OnUnloadingStarted -= DisablePlayerGravity;
        sceneLoadingManager.OnSceneGroupLoaded -= EnablePlayerGravity;
    }

    public void CheckIfAllPlayersAreDead()
    {

        int numPlayers = playerControllers.Count;
        int numDeadPlayers = 0;
        foreach (NewPlayerController controller in playerControllers.Values)
        {
            if (controller.HealthController.IsDead)
            {
                numDeadPlayers++;
            }
        }

        if (numDeadPlayers == numPlayers)
        {
            OpenLoadMenu();
        }
    }

    public void OpenLoadMenu()
    {
        inventoryManager.gameObject.SetActive(false);
        LoadMenuManager.OpenLoadMenu();
    }

    private void StartNewGame()
    {
        SpawnPlayers();
    }

    private void LoadSavedGame()
    {

        LoadGameStateData = playSessionData.PlaySessionGameData;
        playSessionData.PlaySessionMetaData.SessionStartTimestamp = DateTime.UtcNow.Ticks;

        InitializeSpawnedItemDataBase();

        GameSetupData data = playSessionData.gameSetupData;

        for (int i = 0; i < LoadGameStateData.PlayerStateDatas.Count; i++)
        {
            if (data.Selections[i].PlayerDevices != null)
            {
                InputDevice device = data.Selections[i].PlayerDevices;
                string scheme = data.Selections[i].PlayerControlSchemes;

                PlayerInput playerInput = playerInputManager.JoinPlayer(LoadGameStateData.PlayerStateDatas[i].playerIndex, -1, scheme, device);
                playerInput.gameObject.transform.position = NavMeshUtils.ReturnRandomPointOnXZ(Vector3.zero, 4f);
            }
            else
            {
                awaitingSecondPlayer = true;
            }
        }
    }

    private void Update()
    {
        if (!awaitingSecondPlayer || player2Loaded) return;

        InputDevice newDevice = DetectUnusedDevice();

        if (newDevice != null)
        {
            SpawnPlayer2UsingSavedData(newDevice);
            awaitingSecondPlayer = false;
            player2Loaded = true;
        }
    }

    private InputDevice DetectUnusedDevice()
    {
        var used = new HashSet<InputDevice>(
            playerControllers[0].PlayerContext.PlayerInput.devices
        );

        // Keyboard
        if (Keyboard.current != null &&
            !used.Contains(Keyboard.current) &&
            Keyboard.current.anyKey.wasPressedThisFrame)
        {
            return Keyboard.current;
        }

        // Gamepads
        foreach (var gp in Gamepad.all)
        {
            if (used.Contains(gp)) continue;

            if (gp.allControls.Any(c => c is ButtonControl b && b.wasPressedThisFrame))
                return gp;
        }

        return null;
    }

    private void SpawnPlayer2UsingSavedData(InputDevice device)
    {
        var player2Data = LoadGameStateData.PlayerStateDatas[1];
        string controlScheme = DetermineSchemeFor(device);

        PlayerInput input = playerInputManager.JoinPlayer(player2Data.playerIndex, -1, controlScheme, device);

        playSessionData.gameSetupData.Selections[1].PlayerDevices = device;
        playSessionData.gameSetupData.Selections[1].PlayerControlSchemes = controlScheme;

        Vector3 spawnPos = NavMeshUtils.ReturnRandomPointOnXZ(playerAveragePositionTracker.transform.position, 2f);
        spawnPos.y = 0;
        input.gameObject.transform.position = spawnPos;
    }

    private string DetermineSchemeFor(InputDevice device)
    {
        switch (device)
        {
            case Gamepad:
                return "Gamepad";
            case Keyboard:
                return "Keyboard&Mouse";
        }
        return "";
    }


    private void InitializeSpawnedItemDataBase()
    {
        foreach (ItemDataSaveEntry itemData in LoadGameStateData.SpawnedItemData)
        {
            ItemSO itemSO = ItemDatabase.GetItemSO(itemData.ItemSO_ID);
            ItemData newItemData = itemData.itemData;
            newItemData.ItemSO = itemSO;
            if (!spawnedItemDatabase.spawnedItemData.ContainsKey(newItemData.ItemID))
                spawnedItemDatabase.spawnedItemData.Add(newItemData.ItemID, newItemData);
        }
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
            if (!firstLoad)
            {
                if (targetSpawnPoint != null)
                {
                    playerControllers[i].transform.SetPositionAndRotation(targetSpawnPoint.transform.GetChild(i).position, targetSpawnPoint.transform.GetChild(i).rotation);
                }
                else
                {
                    playerControllers[i].transform.position = Vector3.zero;
                }
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
        firstLoad = false;
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
        playerControllers[playerInput.playerIndex].HealthController.OnDie += CheckIfAllPlayersAreDead;
        CreatePlayerCanvas(playerInput);
        OnPlayerJoinedEvent?.Invoke(playerInput.gameObject);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        playerControllers[playerInput.playerIndex].HealthController.OnDie -= CheckIfAllPlayersAreDead;
        playerControllers.Remove(playerInput.playerIndex);
        OnPlayerLeftEvent?.Invoke(playerInput.gameObject);
    }

    public void SpawnPlayers()
    {
        var data = playSessionData.gameSetupData;
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

        if (playSessionData.PlaySessionLoadMode == GameLoadMode.NewGame)
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
        playerContext.PlayerClassPreset = classPreset;
        playerContext.UserInterfaceController = playerUserInterfaceController;
        playerContext.PlayerController = GetPlayerControllerByIndex(playerInput.playerIndex);
        playerContext.PlayerController.ExperienceController.level = playerStateData.Level;
        playerContext.PlayerController.PlayerStatsBlackboard.GoldAmount = playerStateData.GoldAmount;
        playerContext.PlayerController.ExperienceController.SkillPoints = playerStateData.SkillPoints;
        playerContext.PlayerController.ExperienceController.experience = playerStateData.currentExp;
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
        playerContext.PlayerController.FeatsController.SetupLoadedCharacter(runtimeFeats, classPreset.classFeatConfig);

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
        playerContext.PlayerController.HealthController.CurrentHealth = playerStateData.currentHealth;
        playerContext.PlayerController.Heal(0);
        playerContext.PlayerController.ResourceController.resource.resourceCurrent = playerStateData.currentMana;
        playerContext.PlayerController.ResourceController.resource.AddResource(0);
        playerContext.UserInterfaceController.inventoryController.FeatsMenu.CreateFeatButtons(playerContext);
    }

    public void SetupNewPlayer(PlayerUserInterfaceController playerUserInterfaceController, PlayerInput playerInput)
    {
        PlayerContext playerContext;
        playerContext = new PlayerContext();
        playerContext.PlayerIndex = playerInput.playerIndex;
        playerContext.PlayerClassPreset = playSessionData.gameSetupData.chosenClassPresets[playerInput.playerIndex];
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

        ClassPresetSO classPresetSO = playSessionData.gameSetupData.chosenClassPresets[playerInput.playerIndex];

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
