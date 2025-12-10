using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    public void SaveGame(int slotIndex)
    {
        var meta = PlaySessionData.Instance.PlaySessionMetaData;
        var data = PlaySessionData.Instance.PlaySessionGameData;

        WriteZoneData(data);
        WriteSpawnedItemDatabase(data);
        WriteLastCheckpointData(data);
        foreach (var saveable in SaveRegistry.All)
            saveable.Save(data);

        UpdateMetaData(meta, data);

        WriteToDisk(slotIndex, data, meta);
    }


    private string GetSlotRoot()
    {
        return Path.Combine(Application.persistentDataPath, "Saves");
    }

    private string GetSlotFolder(int slotIndex)
    {
        return Path.Combine(GetSlotRoot(), $"Slot{slotIndex}");
    }

    private string GetSavePath(int slotIndex)
    {
        return Path.Combine(GetSlotFolder(slotIndex), "save.json");
    }

    private string GetMetaPath(int slotIndex)
    {
        return Path.Combine(GetSlotFolder(slotIndex), "metadata.json");
    }

    public void WriteZoneData(GameStateData data)
    {
        ZoneManager.Instance.GenerateZoneData(SceneLoadingManager.Instance.ReturnActiveEnvironmentalScene().name);
        data.ZoneDatas = new();
        foreach (ZoneData zd in ZoneManager.instance.ZoneDatas)
        {
            zd.PrepareForSave();
            data.ZoneDatas.Add(zd);
        }
    }

    public void WriteSpawnedItemDatabase(GameStateData data)
    {
        data.SpawnedItemData = new();

        foreach (var kvp in SpawnedItemDataBase.Instance.spawnedItemData)
        {
            ItemDataSaveEntry entry = new();
            entry.itemID = kvp.Key;
            entry.itemData = kvp.Value;
            entry.ItemSO_ID = entry.itemData.ItemSO.ItemName;

            data.SpawnedItemData.Add(entry);
        }
    }

    public void WriteLastCheckpointData(GameStateData data)
    {
        data.LastCheckpointSaveData.sceneGroup = SceneLoadingManager.Instance.activeSceneGroup.GroupName;
        data.LastCheckpointSaveData.checkpointIndex = ZoneManager.Instance.LastCheckpointIndex;
    }

    public void UpdateMetaData(SaveSlotMetaData meta, GameStateData data)
    {
        if (meta == null || data == null)
            return;

        meta.LastZoneName = data.LastCheckpointSaveData != null
            ? data.LastCheckpointSaveData.sceneGroup
            : "UnknownZone";

        meta.LastSavedTimestamp = DateTime.UtcNow.Ticks;

        if (meta.newGame)
        {
            meta.newGame = false;
        }

        meta.TotalSessionPlaytimeSeconds += (int)((DateTime.UtcNow.Ticks - meta.SessionStartTimestamp) / TimeSpan.TicksPerSecond);

        meta.SessionStartTimestamp = DateTime.UtcNow.Ticks;

        if (data.PlayerStateDatas != null)
        {
            meta.playerCount = data.PlayerStateDatas.Count;
            meta.playerClasses = data.PlayerStateDatas
                .Select(p => p.classPresetID)
                .ToList();
        }
        else
        {
            meta.playerCount = 0;
            meta.playerClasses = new();
        }

        meta.isValid = true;
    }

    private void WriteToDisk(int slotIndex, GameStateData data, SaveSlotMetaData meta)
    {
        string slotFolder = GetSlotFolder(slotIndex);
        if (!Directory.Exists(slotFolder))
            Directory.CreateDirectory(slotFolder);

        string saveJson = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(slotIndex), saveJson);

        string metaJson = JsonUtility.ToJson(meta, true);
        File.WriteAllText(GetMetaPath(slotIndex), metaJson);
    }
}

[System.Serializable]
public class GameStateData
{
    public List<ZoneData> ZoneDatas;
    public List<PlayerStateData> PlayerStateDatas;
    public LastCheckpointSaveData LastCheckpointSaveData;

    public List<ItemDataSaveEntry> SpawnedItemData;

    public PlayerStateData GetPlayerStateFor(int playerId)
    {
        // Look for an existing entry
        for (int i = 0; i < PlayerStateDatas.Count; i++)
        {
            if (PlayerStateDatas[i].playerIndex == playerId)
                return PlayerStateDatas[i];
        }

        PlayerStateData newData = new PlayerStateData();
        PlayerStateDatas.Add(newData);
        return newData;
    }
}

[System.Serializable]
public class PlayerStateData
{
    public int playerIndex;

    //General Info
    public int Level;
    public int GoldAmount;
    public int SkillPoints;
    public int currentExp;
    public int currentHealth;
    public float currentMana;

    public string classPresetID;

    public List<RuntimeFeatSaveData> unlockedFeats; //Then we level the necessary feats

    //Equipment Info
    public List<ItemSaveData> weapons;
    public List<ItemSaveData> armor;
    public List<ConsumableSaveData> misc;
}

[System.Serializable]
public class ItemSaveData
{
    public string itemID;
    public bool isEquipped;
}

[System.Serializable]
public class LastCheckpointSaveData
{
    //We just need to store what scene and what checkpoint we were at
    public string sceneGroup;
    public int checkpointIndex;
}

[System.Serializable]
public class ConsumableSaveData
{
    public int quantity;
    public string ItemSO_ID;
}

[System.Serializable]
public class RuntimeFeatSaveData
{
    public string featID;
    public int currentLevel;
}

[System.Serializable]
public class ItemDataSaveEntry
{
    public string itemID;
    public ItemData itemData;
    public string ItemSO_ID;
}

[Serializable]
public class SaveSlotMetaData
{
    public string LastZoneName;                 //Last zone players were in
    public long LastSavedTimestamp;             //When the game was last saved
    public long SaveCreatedTimestamp;           //When the save was created
    public long SessionStartTimestamp;          //When this play session was started
    public int TotalSessionPlaytimeSeconds;     //Total seconds played

    public int playerCount;
    public List<string> playerClasses;

    public bool isValid;
    public bool newGame = true;
}
