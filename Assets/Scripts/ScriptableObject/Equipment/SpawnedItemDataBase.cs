using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class SpawnedItemDataBase : Singleton<SpawnedItemDataBase>
{
    public Dictionary<string, ItemData> spawnedItemData = new Dictionary<string, ItemData>();

    public List<ItemSO> spawnableItems;
    public List<ConsumableDrop> spawnableConsumables;

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
        itemData.ItemID = RegisterItemToDatabase(itemData);
        return itemData;
    }

    public ItemSO ReturnRandomWeaponSO()
    {
        for (int i = 0; i < 50; i++)
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
        for (int i = 0; i < 50; i++)
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

    public ItemData GetSpawnedItemDataFromDataBase(string id)
    {
        ItemData data = null;
        if (spawnedItemData.ContainsKey(id))
        {
            data = spawnedItemData[id];
            return data;
        }
        Debug.Log("No ItemData found at id");
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