using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class SpawnedItemDataBase : Singleton<SpawnedItemDataBase>
{
    public Dictionary<string, SpawnedItemData> spawnedItemData = new Dictionary<string, SpawnedItemData>();

    public List<ItemSO> spawnableItems;
    public List<ConsumableDrop> spawnableConsumables;

    public void SpawnItemFromDatabase(string itemID, Vector3 worldPosition, Quaternion worldRotation)
    {
        ItemData itemData = GetSpawnedItemDataFromDataBase(itemID).itemData;

        GameObject itemToDrop = new GameObject(itemData.Name);
        Item newItem = itemToDrop.AddComponent<Item>();
        newItem.ItemData = itemData;

        if (itemData.GroundPrefab == null)
        {
            Instantiate(itemData.ItemPrefab, Vector3.zero, itemData.ItemPrefab.transform.rotation, itemToDrop.transform);
        }
        else
        {
            Instantiate(itemData.GroundPrefab, Vector3.zero, itemData.GroundPrefab.transform.rotation, itemToDrop.transform);
        }

        SceneManager.MoveGameObjectToScene(itemToDrop, SceneLoadingManager.Instance.ReturnActiveEnvironmentalScene());
        itemToDrop.transform.SetPositionAndRotation(worldPosition, worldRotation);

        //Add collider
        itemToDrop.AddComponent<SphereCollider>().isTrigger = true;
    }

    public string RegisterItemToDatabase(ItemData itemData)
    {
        string id = Guid.NewGuid().ToString();
        SpawnedItemData spawnedItem = new SpawnedItemData(id, itemData, itemData.Quality);
        spawnedItemData.Add(id, spawnedItem);
        return id;
    }

    public ItemData CreateItemData(ItemSO itemSO, ItemQuality quality = ItemQuality.Normal, int quantity = 1)
    {
        ItemData itemData = new ItemData();
        itemData.ItemSO = itemSO;
        itemData.Quality = quality;
        itemData.Quantity = quantity;
        itemData.ItemID = RegisterItemToDatabase(itemData);
        return itemData;
    }

    public ItemSO ReturnRandomWeaponSO()
    {
        for (int i = 0; i < 20; i++)
        {
            int rand = UnityEngine.Random.Range(0, spawnableItems.Count);
            if (spawnableItems[rand] is WeaponSO)
            {
                return spawnableItems[rand];
            }
        }
        return null;
    }

    public ItemSO ReturnRandomArmorSO()
    {
        for (int i = 0; i < 20; i++)
        {
            int rand = UnityEngine.Random.Range(0, spawnableItems.Count);
            if (spawnableItems[rand] is ArmorSO || spawnableItems[rand] is ShieldSO)
            {
                return spawnableItems[rand];
            }
        }
        return null;
    }

    public void SpawnWorldDropAtPosition(LootResult lootResult, Vector3 worldPosition)
    {
        Item item = Instantiate(lootResult.itemSO.GroundItemPrefab, worldPosition, lootResult.itemSO.ItemPrefab.transform.rotation).GetComponent<Item>();
        item.ItemSO = lootResult.itemSO;
        item.Quality = lootResult.quality;
        item.Quantity = lootResult.quantity;
        SceneManager.MoveGameObjectToScene(item.gameObject, SceneLoadingManager.Instance.ReturnActiveEnvironmentalScene());
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