using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class SpawnedItemDataBase : Singleton<SpawnedItemDataBase>
{
    public Dictionary<string, SpawnedItemData> spawnedItemData = new Dictionary<string, SpawnedItemData>();

    public List<Item> spawnableItems;
    public List<ConsumableDrop> spawnableConsumables;

    public void SpawnItemFromDatabase(string itemID, Vector3 worldPosition, Quaternion worldRotation)
    {
        ItemData itemData = GetSpawnedItemDataFromDataBase(itemID).itemData;

        GameObject itemToDrop = new GameObject(itemData.itemName);
        Item newItem = itemToDrop.AddComponent<Item>();
        newItem.itemData = itemData;

        if (itemData.floorObjectPrefab == null)
        {
            Instantiate(itemData.objectPrefab, Vector3.zero, itemData.objectPrefab.transform.rotation, itemToDrop.transform);
        }
        else
        {
            Instantiate(itemData.floorObjectPrefab, Vector3.zero, itemData.floorObjectPrefab.transform.rotation, itemToDrop.transform);
        }

        SceneManager.MoveGameObjectToScene(itemToDrop, SceneLoadingManager.Instance.ReturnActiveEnvironmentalScene());
        itemToDrop.transform.SetPositionAndRotation(worldPosition, worldRotation);

        //Add collider
        itemToDrop.AddComponent<SphereCollider>().isTrigger = true;
    }

    public Item SpawnAndRegisterItem(ItemData itemData, Vector3 worldPosition, Quaternion worldRotation)
    {
        GameObject itemToDrop = new GameObject(itemData.itemName);
        Item newItem = itemToDrop.AddComponent<Item>();
        newItem.itemData = itemData;
        newItem.itemData.itemID = RegisterItemToDatabase(itemData);

        if (itemData.floorObjectPrefab == null)
        {
            Instantiate(itemData.objectPrefab, Vector3.zero, itemData.objectPrefab.transform.rotation, itemToDrop.transform);
        }
        else
        {
            Instantiate(itemData.floorObjectPrefab, Vector3.zero, itemData.floorObjectPrefab.transform.rotation, itemToDrop.transform);
        }

        SceneManager.MoveGameObjectToScene(itemToDrop, SceneLoadingManager.Instance.ReturnActiveEnvironmentalScene());
        itemToDrop.transform.SetPositionAndRotation(worldPosition, worldRotation);

        itemToDrop.AddComponent<SphereCollider>().isTrigger = true;
        return newItem;
    }

    public void SpawnConsumableItem(ConsumableDrop consumableDrop, Vector3 worldPosition, Quaternion worldRotation)
    {
        GameObject obj = Instantiate(consumableDrop.potionData.PotionPrefab, worldPosition, worldRotation);
        SceneManager.MoveGameObjectToScene(obj, SceneLoadingManager.Instance.ReturnActiveEnvironmentalScene());
    }

    public Item ReturnRandomItem()
    {
        int rand = UnityEngine.Random.Range(0, spawnableItems.Count);
        return spawnableItems[rand];
    }

    public string RegisterItemToDatabase(ItemData itemData)
    {
        string id = Guid.NewGuid().ToString();
        SpawnedItemData spawnedItem = new SpawnedItemData(id, itemData, itemData.itemQuality);
        spawnedItemData.Add(id, spawnedItem);
        return id;
    }

    public SpawnedItemData GetSpawnedItemDataFromDataBase(string id)
    {
        SpawnedItemData data = null;
        if (spawnedItemData.ContainsKey(id))
        {
            data = spawnedItemData[id];
        }
        return data;
    }

    public class SpawnedItemData
    {
        public string uniqueID;
        public ItemData itemData;
        public ItemQuality itemQuality;

        public SpawnedItemData(string id, ItemData data, ItemQuality quality)
        {
            uniqueID = id;
            itemData = data;
            itemQuality = quality;
        }
    }
}



public enum ItemQuality
{
    Shoddy = -1,
    Normal = 0,
    Fine = 1,
    Remarkable = 2,
    Superior = 3,
    Grand = 4,
    Imperial = 5,
    Flawless = 6
}