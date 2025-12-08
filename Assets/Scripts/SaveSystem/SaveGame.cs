using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveGame : Singleton<SaveGame>
{
    public PlayerJoinManager PlayerJoinManager;
    public SceneLoadingManager SceneLoadingManager;
    public ZoneManager ZoneManager;

    public GameStateData CurrentSave = new();
    public int LastCheckpointIndex;

    protected override void Awake()
    {
        base.Awake();
        SceneLoadingManager = SceneLoadingManager.Instance;
    }

    public void Save()
    {
        if (CurrentSave.PlayerStateDatas == null)
            CurrentSave.PlayerStateDatas = new();

        GatherPlayerState();
        GatherWorldState();
        GatherLastCheckpointSaveData();
        GatherSpawnedItemData();

        string path = Path.Combine(Application.persistentDataPath, "save1.json");
        string json = JsonUtility.ToJson(CurrentSave, true);
        File.WriteAllText(path, json);
        Debug.Log($"Saved game to {path}");
    }

    public void GatherPlayerState()
    {
        foreach (NewPlayerController player in PlayerJoinManager.playerControllers.Values)
        {
            PlayerStateData playerStateData = CurrentSave.PlayerStateDatas.Find(p => p.playerIndex == player.PlayerContext.PlayerInput.playerIndex);
            if (playerStateData == null)
            {
                playerStateData = new PlayerStateData();
                playerStateData.playerIndex = player.PlayerContext.PlayerInput.playerIndex;
                CurrentSave.PlayerStateDatas.Add(playerStateData);
            }

            playerStateData.Level = player.ExperienceController.level;
            playerStateData.GoldAmount = player.PlayerStatsBlackboard.GoldAmount;
            playerStateData.SkillPoints = player.ExperienceController.SkillPoints;
            playerStateData.currentExp = player.ExperienceController.experience;
            playerStateData.currentHealth = player.HealthController.CurrentHealth;
            playerStateData.currentMana = player.ResourceController.resource.resourceCurrent;
            playerStateData.classPresetID = player.PlayerContext.PlayerClassPreset.PresetName;

            playerStateData.unlockedFeats = new();
            foreach (RuntimeFeat runtimeFeat in player.FeatsController.UnlockedFeats)
            {
                RuntimeFeatSaveData s = new RuntimeFeatSaveData();
                s.featID = runtimeFeat.BaseFeatSO.FeatName;
                s.currentLevel = runtimeFeat.CurrentFeatLevel;
                playerStateData.unlockedFeats.Add(s);
            }

            playerStateData.weapons = new();
            foreach (ItemButton button in player.PlayerContext.InventoryController.WeaponInventory.instantiatedItemButtons.Values)
            {
                ItemSaveData weapon = new ItemSaveData();
                weapon.itemID = button.ItemData.ItemID;
                weapon.itemData = button.ItemData;
                weapon.isEquipped = false;
                if (button.buttonState == ItemButton.ButtonState.Activated)
                {
                    weapon.isEquipped = true;
                }
                playerStateData.weapons.Add(weapon);
            }

            playerStateData.armor = new();
            foreach (ItemButton button in player.PlayerContext.InventoryController.ArmorInventory.instantiatedItemButtons.Values)
            {
                ItemSaveData armor = new ItemSaveData();
                armor.itemID = button.ItemData.ItemID;
                armor.itemData = button.ItemData;
                armor.isEquipped = false;
                if (button.buttonState == ItemButton.ButtonState.Activated)
                {
                    armor.isEquipped = true;
                }
                playerStateData.armor.Add(armor);
            }

            playerStateData.misc = new();
            foreach (ItemButton button in player.PlayerContext.InventoryController.ConsumablesInventory.instantiatedItemButtons.Values)
            {
                ConsumableSaveData consumable = new ConsumableSaveData();
                consumable.consumableSO = button.ItemSO;
                consumable.quantity = (button as ConsumableButton).Quantity;
                consumable.ItemSO_ID = button.ItemSO.ItemName;
                playerStateData.misc.Add(consumable);
            }
        }
    }

    public void GatherWorldState()
    {
        //Generate zone data for the current zone, then save it
        ZoneManager.GenerateZoneData(SceneLoadingManager.ReturnActiveEnvironmentalScene().name);
        foreach (var zone in ZoneManager.ZoneDatas)
        {
            zone.PrepareForSave();
        }
        CurrentSave.ZoneDatas = ZoneManager.ZoneDatas;
    }

    public void GatherLastCheckpointSaveData()
    {
        CurrentSave.LastCheckpointSaveData.sceneGroup = SceneLoadingManager.activeSceneGroup.GroupName;
        CurrentSave.LastCheckpointSaveData.checkpointIndex = LastCheckpointIndex;
    }

    public void GatherSpawnedItemData()
    {
        CurrentSave.SpawnedItemData = new();

        foreach (var kvp in SpawnedItemDataBase.Instance.spawnedItemData)
        {
            ItemDataSaveEntry entry = new();
            entry.itemID = kvp.Key;
            entry.itemData = kvp.Value;
            entry.ItemSO_ID = entry.itemData.ItemSO.ItemName;

            CurrentSave.SpawnedItemData.Add(entry);
        }
    }

    [ContextMenu("Save Game")]
    public void SaveGameTest()
    {
        Save();
    }
}

[System.Serializable]
public class GameStateData
{
    public List<ZoneData> ZoneDatas;
    public List<PlayerStateData> PlayerStateDatas;
    public LastCheckpointSaveData LastCheckpointSaveData;

    public List<ItemDataSaveEntry> SpawnedItemData;
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
    public ItemData itemData;
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
    public ItemSO consumableSO;
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
