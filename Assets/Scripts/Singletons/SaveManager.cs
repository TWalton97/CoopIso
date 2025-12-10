using System;
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

        meta.TotalSessionPlaytimeSeconds = (int)((DateTime.UtcNow.Ticks - meta.SessionStartTimestamp) / TimeSpan.TicksPerSecond);

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
