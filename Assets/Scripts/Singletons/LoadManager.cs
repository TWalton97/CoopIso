using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class LoadManager : Singleton<LoadManager>
{
    public SceneGroup FirstGameplayScene;

    public void LoadGame(int slotIndex)
    {
        PlaySessionData.Instance.PlaySessionMetaData = ReturnSaveSlotMetaData(slotIndex);
        PlaySessionData.Instance.PlaySessionGameData = ReturnGameStateData(slotIndex);
    }

    public SaveSlotMetaData ReturnSaveSlotMetaData(int slotIndex)
    {
        string slotFolder = Path.Combine(Application.persistentDataPath, "Saves", $"Slot{slotIndex}");
        string metaPath = Path.Combine(slotFolder, "metadata.json");

        // --- If metadata exists, load it ---
        if (File.Exists(metaPath))
        {
            try
            {
                string json = File.ReadAllText(metaPath);
                SaveSlotMetaData meta = JsonUtility.FromJson<SaveSlotMetaData>(json);
                return meta;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to read metadata for slot {slotIndex}.\n{e}");
            }
        }

        // --- Otherwise, create a new metadata object ---
        SaveSlotMetaData newMeta = new SaveSlotMetaData
        {
            SaveCreatedTimestamp = DateTime.UtcNow.Ticks,
            SessionStartTimestamp = DateTime.UtcNow.Ticks,
        };

        return newMeta;
    }

    public GameStateData ReturnGameStateData(int slotIndex)
    {
        string slotFolder = Path.Combine(Application.persistentDataPath, "Saves", $"Slot{slotIndex}");
        string savePath = Path.Combine(slotFolder, "save.json");

        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                GameStateData data = JsonUtility.FromJson<GameStateData>(json);
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load save data for slot {slotIndex} at {savePath}\n{e}");
                return null;
            }
        }
        GameStateData gameStateData = new GameStateData();
        gameStateData.PlayerStateDatas = new();
        gameStateData.LastCheckpointSaveData = new();
        gameStateData.LastCheckpointSaveData.sceneGroup = FirstGameplayScene.GroupName;
        return gameStateData;
    }
}
