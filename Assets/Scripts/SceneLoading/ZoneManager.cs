using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZoneManager : MonoBehaviour
{
    //Every time a zone gets unloaded we store a reference to that zone along with a list of all entity
    public SceneLoadingManager sceneLoadingManager;
    public SpawnedItemDataBase SpawnedItemDataBase;

    public List<ZoneData> ZoneDatas;

    [System.Serializable]
    //Need to store all enemy data
    //ID
    //Position
    //Is Dead

    //Need to store all item data
    //ID
    //Position
    //Is Dead

    //Need to store all interactable data
    //ID
    //Interacted

    public class ZoneData
    {
        public string ZoneName;
        private Dictionary<string, EntityStatus> EntityStatusDict = new Dictionary<string, EntityStatus>();
        public List<ItemStatus> ItemStatuses = new();
        public Dictionary<string, ChestStatus> ChestStatuses = new();

        public ZoneData(string _zoneName, Dictionary<string, EntityStatus> _entityStatusDict, List<ItemStatus> _itemStatuses, Dictionary<string, ChestStatus> _chestStatuses)
        {
            ZoneName = _zoneName;
            EntityStatusDict = _entityStatusDict;
            ItemStatuses = _itemStatuses;
            ChestStatuses = _chestStatuses;
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
            ChestStatuses[newStatus.GUID] = newStatus;
        }

        public bool TryGetChestStatus(string guid, out ChestStatus status)
        {
            return ChestStatuses.TryGetValue(guid, out status);
        }

    }
    void Awake()
    {
        sceneLoadingManager = SceneLoadingManager.Instance;
    }
    void OnEnable()
    {
        sceneLoadingManager.OnSceneUnloadStarted += GenerateZoneData;
        sceneLoadingManager.OnSceneLoaded += CallWaitForScene;
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
    private void GenerateZoneData(string sceneName)
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
