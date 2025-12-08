using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZoneManager : Singleton<ZoneManager>
{
    //Every time a zone gets unloaded we store a reference to that zone along with a list of all entity
    public SceneLoadingManager sceneLoadingManager;
    public SpawnedItemDataBase SpawnedItemDataBase;
    private MainMenuController mainMenuController;

    public List<ZoneData> ZoneDatas;

    public int LastCheckpointIndex;
    public bool ShouldApplySavedCheckpoint { get; set; } = false;

    protected override void Awake()
    {
        base.Awake();
        sceneLoadingManager = SceneLoadingManager.Instance;

        sceneLoadingManager.OnSceneUnloadStarted += GenerateZoneData;
        sceneLoadingManager.OnSceneLoaded += CallWaitForScene;

        mainMenuController = FindObjectOfType<MainMenuController>();
        if (mainMenuController.gameLoadMode == GameLoadMode.LoadedGame)
        {
            ZoneDatas = mainMenuController.GameStateDataToLoad.ZoneDatas;
            foreach (ZoneData zd in ZoneDatas)
            {
                zd.RebuildDictionaries();
            }

            LastCheckpointIndex = mainMenuController.GameStateDataToLoad.LastCheckpointSaveData.checkpointIndex;
            ShouldApplySavedCheckpoint = true;
        }
    }

    void Start()
    {
        CallWaitForScene(sceneLoadingManager.ReturnActiveEnvironmentalScene().name);
    }

    void OnDisable()
    {
        sceneLoadingManager.OnSceneUnloadStarted -= GenerateZoneData;
        sceneLoadingManager.OnSceneLoaded -= CallWaitForScene;
    }
    public bool HasZoneDataForScene(string sceneName)
    {
        if (ZoneDatas.Count == 0) return false;

        foreach (ZoneData zoneData in ZoneDatas)
        {
            if (zoneData.ZoneName == sceneName) return true;
        }

        return false;
    }
    private ZoneData ReturnZoneDataByName(string sceneName)
    {
        if (ZoneDatas.Count == 0) return null;

        foreach (ZoneData zoneData in ZoneDatas)
        {
            if (zoneData.ZoneName == sceneName) return zoneData;
        }

        return null;
    }
    public void GenerateZoneData(string sceneName)
    {
        if (HasZoneDataForScene(sceneName))
        {
            UpdateZoneData(sceneName);
            return;
        }

        List<Enemy> enemies = GetObjectsOfTypeInScene<Enemy>(sceneName);
        Dictionary<string, EntityStatus> tempDict = new();
        if (enemies.Count == 0)
        {

        }
        else
        {
            foreach (Enemy enemy in enemies)
            {
                EntityStatus status = enemy.ReturnEntityStatus();
                tempDict.Add(status.GUID, status);
            }
        }

        List<Item> items = GetObjectsOfTypeInScene<Item>(sceneName);
        List<ItemStatus> tempList = new();
        if (items.Count == 0)
        {

        }
        else
        {
            foreach (Item item in items)
            {
                tempList.Add(item.ReturnItemStatus());
            }
        }

        List<Chest> chests = GetObjectsOfTypeInScene<Chest>(sceneName);
        Dictionary<string, ChestStatus> chestStatusDict = new();
        if (chests.Count == 0)
        {

        }
        else
        {
            foreach (Chest chest in chests)
            {
                ChestStatus status = chest.ReturnChestStatus();
                chestStatusDict.Add(status.GUID, status);
            }
        }

        ZoneDatas.Add(new ZoneData(sceneName, tempDict, tempList, chestStatusDict));
    }
    private void UpdateZoneData(string sceneName)
    {
        ZoneData zoneData = ReturnZoneDataByName(sceneName);

        List<Enemy> enemies = GetObjectsOfTypeInScene<Enemy>(sceneName);

        List<EntityStatus> tempEntityStatuses = new();

        foreach (Enemy enemy in enemies)
        {
            tempEntityStatuses.Add(enemy.ReturnEntityStatus());
        }

        foreach (EntityStatus status in tempEntityStatuses)
        {
            zoneData.UpdateEntityStatus(status);
        }

        List<Item> items = GetObjectsOfTypeInScene<Item>(sceneName);
        List<ItemStatus> tempList = new();
        if (items.Count == 0)
        {

        }
        else
        {
            foreach (Item item in items)
            {
                tempList.Add(item.ReturnItemStatus());
            }
        }

        zoneData.ReplaceItemStatuses(tempList);

        List<Chest> chests = GetObjectsOfTypeInScene<Chest>(sceneName);

        List<ChestStatus> tempChestStatuses = new();

        foreach (Chest chest in chests)
        {
            tempChestStatuses.Add(chest.ReturnChestStatus());
        }

        foreach (ChestStatus status in tempChestStatuses)
        {
            zoneData.UpdateChestStatus(status);
        }
    }

    private void CallWaitForScene(string sceneName)
    {
        StartCoroutine(WaitForSceneToBeLoaded(sceneName));
    }
    private IEnumerator WaitForSceneToBeLoaded(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        while (!scene.IsValid() || !scene.isLoaded)
        {
            yield return null;
        }
        DistributeEntityStatuses(sceneName);
        if (ShouldApplySavedCheckpoint)
        {
            ZoneController zoneController = FindObjectOfType<ZoneController>();
            Checkpoint checkpoint = zoneController.FindCheckpointByIndex(LastCheckpointIndex);
            foreach (NewPlayerController controller in PlayerJoinManager.Instance.playerControllers.Values)
            {
                checkpoint.RespawnPlayer(controller);
            }
            ShouldApplySavedCheckpoint = false;
        }
        yield return null;
    }
    public void DistributeEntityStatuses(string sceneName)
    {
        if (!HasZoneDataForScene(sceneName))
        {
            return;
        }

        ZoneData zoneData = ReturnZoneDataByName(sceneName);

        List<Enemy> enemies = GetObjectsOfTypeInScene<Enemy>(sceneName);

        foreach (Enemy enemy in enemies)
        {
            string guid = enemy.EntityStatus.GUID;
            if (zoneData.TryGetEnemyStatus(guid, out EntityStatus status))
            {
                enemy.EntityStatus = status;
                enemy.transform.position = status.WorldPosition;
                enemy.IsDead = status.IsDead;
                if (status.IsDead)
                    enemy.HasSpawnedItems = true;
            }
        }

        foreach (ItemStatus itemStatus in zoneData.ItemStatuses)
        {
            SpawnedItemDataBase.SpawnItemFromDatabase(itemStatus.GUID, itemStatus.WorldPosition, itemStatus.WorldRotation);
        }

        List<Chest> chests = GetObjectsOfTypeInScene<Chest>(sceneName);

        foreach (Chest chest in chests)
        {
            string guid = chest.ChestStatus.GUID;
            if (zoneData.TryGetChestStatus(guid, out ChestStatus status))
            {
                chest.ChestStatus = status;
                chest.isInteractable = !status.IsOpened;
                if (status.IsOpened)
                {
                    chest.animator.Play("ChestOpen", 0, 1f);
                    chest.animator.Update(0f);
                }
            }
        }
    }
    public List<T> GetObjectsOfTypeInScene<T>(string sceneName) where T : Component
    {
        List<T> foundComponents = new List<T>();
        Scene scene = SceneManager.GetSceneByName(sceneName);

        // Check if the scene is valid and loaded
        if (!scene.IsValid() || !scene.isLoaded)
        {
            return foundComponents;
        }

        // Get all root GameObjects in the specified scene
        GameObject[] rootObjects = scene.GetRootGameObjects();

        // Iterate through all root objects and their children
        foreach (GameObject rootObject in rootObjects)
        {
            T[] components = rootObject.GetComponentsInChildren<T>(true);
            foundComponents.AddRange(components);
        }

        return foundComponents;
    }
}

[System.Serializable]
public class ZoneData
{
    public string ZoneName;

    [NonSerialized] public Dictionary<string, EntityStatus> EntityStatusDict = new Dictionary<string, EntityStatus>();
    public List<SerializableKeyValuePair<string, EntityStatus>> EntityStatusList;

    public List<ItemStatus> ItemStatuses = new();

    [NonSerialized] public Dictionary<string, ChestStatus> ChestStatusDict = new();
    public List<SerializableKeyValuePair<string, ChestStatus>> ChestStatusList;

    public ZoneData(string _zoneName, Dictionary<string, EntityStatus> _entityStatusDict, List<ItemStatus> _itemStatuses, Dictionary<string, ChestStatus> _chestStatuses)
    {
        ZoneName = _zoneName;
        EntityStatusDict = _entityStatusDict;
        ItemStatuses = _itemStatuses;
        ChestStatusDict = _chestStatuses;
    }

    public void UpdateEntityStatus(EntityStatus newStatus)
    {
        EntityStatusDict[newStatus.GUID] = newStatus;
    }

    public bool TryGetEnemyStatus(string guid, out EntityStatus status)
    {
        return EntityStatusDict.TryGetValue(guid, out status);
    }

    public void ReplaceItemStatuses(List<ItemStatus> itemStatuses)
    {
        ItemStatuses.Clear();
        ItemStatuses = itemStatuses;
    }

    public void UpdateChestStatus(ChestStatus newStatus)
    {
        ChestStatusDict[newStatus.GUID] = newStatus;
    }

    public bool TryGetChestStatus(string guid, out ChestStatus status)
    {
        return ChestStatusDict.TryGetValue(guid, out status);
    }

    public void PrepareForSave()
    {
        EntityStatusList = new List<SerializableKeyValuePair<string, EntityStatus>>();
        foreach (var kv in EntityStatusDict)
            EntityStatusList.Add(new SerializableKeyValuePair<string, EntityStatus> { Key = kv.Key, Value = kv.Value });

        ChestStatusList = new List<SerializableKeyValuePair<string, ChestStatus>>();
        foreach (var kv in ChestStatusDict)
            ChestStatusList.Add(new SerializableKeyValuePair<string, ChestStatus> { Key = kv.Key, Value = kv.Value });
    }

    public void RebuildDictionaries()
    {
        EntityStatusDict = new Dictionary<string, EntityStatus>();
        foreach (var kv in EntityStatusList)
            EntityStatusDict[kv.Key] = kv.Value;

        ChestStatusDict = new Dictionary<string, ChestStatus>();
        foreach (var kv in ChestStatusList)
            ChestStatusDict[kv.Key] = kv.Value;
    }
}

[System.Serializable]
public class SerializableKeyValuePair<TKey, TValue>
{
    public TKey Key;
    public TValue Value;
}

