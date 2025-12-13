using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class SpawnedItemDataBase : Singleton<SpawnedItemDataBase>
{
    public Dictionary<string, ItemData> spawnedItemData = new Dictionary<string, ItemData>();

    public List<ItemSO> spawnableItems;
    public List<ConsumableDrop> spawnableConsumables;
    public List<GemSO> spawnableGems;

    public void SpawnItemFromDatabase(string itemID, Vector3 worldPosition, Quaternion worldRotation)
    {
        ItemData itemData = GetSpawnedItemDataFromDataBase(itemID);
        Item spawnedItem;

        if (itemData.GroundPrefab == null)
        {
            spawnedItem = Instantiate(itemData.ItemPrefab, Vector3.zero, itemData.ItemPrefab.transform.rotation).GetComponent<Item>();
        }
        else
        {
            spawnedItem = Instantiate(itemData.GroundPrefab, Vector3.zero, itemData.GroundPrefab.transform.rotation).GetComponent<Item>();
            spawnedItem.Quantity = itemData.Quantity;
        }
        spawnedItem.ItemData = itemData;
        SceneManager.MoveGameObjectToScene(spawnedItem.gameObject, SceneLoadingManager.Instance.ReturnActiveEnvironmentalScene());
        spawnedItem.transform.SetPositionAndRotation(worldPosition, worldRotation);
    }

    public string RegisterItemToDatabase(ItemData itemData)
    {
        string id = Guid.NewGuid().ToString();
        // SpawnedItemData spawnedItem = new SpawnedItemData(id, itemData, itemData.Quality);
        spawnedItemData.Add(id, itemData);
        return id;
    }

    public ItemData CreateItemData(ItemSO itemSO, ItemQuality quality = ItemQuality.Normal, int quantity = 1)
    {
        ItemData itemData = new ItemData();
        itemData.ItemSO = itemSO;
        itemData.Quality = quality;
        itemData.Quantity = quantity;
        itemData.EquipmentSlotType = itemSO.EquipmentSlotType;
        itemData.ItemID = RegisterItemToDatabase(itemData);
        return itemData;
    }

    public ItemSO ReturnRandomItemOfType(Func<ItemSO, bool> filter)
    {
        var filtered = spawnableItems.Where(filter).ToList();
        if (filtered.Count == 0) return null;

        int rand = UnityEngine.Random.Range(0, filtered.Count);
        return filtered[rand];
    }

    public T ReturnRandomItemOfType<T>() where T : ItemSO
    {
        var filtered = spawnableItems.OfType<T>().ToList();
        if (filtered.Count == 0)
            return null;

        int rand = UnityEngine.Random.Range(0, filtered.Count);
        return filtered[rand];
    }

    public void SpawnWorldDropAtPosition(LootResult lootResult, Vector3 worldPosition)
    {
        Item item = Instantiate(lootResult.itemSO.GroundItemPrefab, worldPosition, lootResult.itemSO.ItemPrefab.transform.rotation).GetComponent<Item>();
        item.ItemData = CreateItemData(lootResult.itemSO, lootResult.quality, lootResult.quantity);
        foreach (GemSO gem in lootResult.socketedGems)
        {
            GemEffectHandler.SocketGem(gem, item.ItemData);
        }
        SceneManager.MoveGameObjectToScene(item.gameObject, SceneLoadingManager.Instance.ReturnActiveEnvironmentalScene());
    }

    public ItemData GetSpawnedItemDataFromDataBase(string id)
    {
        ItemData data = null;
        if (spawnedItemData.ContainsKey(id))
        {
            data = spawnedItemData[id];
            return data;
        }
        return data;
    }

    public ItemSO GetItemByID(string itemSO_ID)
    {
        foreach (ItemSO itemSO in spawnableItems)
        {
            if (itemSO.ItemName == itemSO_ID)
            {
                return itemSO;
            }
        }

        foreach (ConsumableDrop consumableDrop in spawnableConsumables)
        {
            if (consumableDrop.ItemSO.ItemName == itemSO_ID)
            {
                return consumableDrop.ItemSO;
            }
        }

        return null;
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