
using UnityEngine;
using System.Collections.Generic;
public static class ClassPresetDatabase
{
    public static Dictionary<string, ClassPresetSO> Presets;

    [RuntimeInitializeOnLoadMethod]
    public static void Initialize()
    {
        Presets = new Dictionary<string, ClassPresetSO>();
        ClassPresetSO[] allPresets = Resources.LoadAll<ClassPresetSO>("ClassPresets");
        foreach (ClassPresetSO presetSO in allPresets)
        {
            Presets[NormalizeGroupName(presetSO.PresetName)] = presetSO;
        }
    }

    public static string NormalizeGroupName(string name)
    {
        return name.Replace(" ", "").ToLower();
    }

    public static ClassPresetSO GetClassPreset(string presetName)
    {
        string key = NormalizeGroupName(presetName);
        if (Presets.TryGetValue(key, out ClassPresetSO preset))
            return preset;

        return null;
    }
}

public static class FeatPresetDatabase
{
    public static Dictionary<string, FeatSO> feats;

    [RuntimeInitializeOnLoadMethod]
    public static void Initialize()
    {
        feats = new Dictionary<string, FeatSO>();
        FeatSO[] allFeats = Resources.LoadAll<FeatSO>("Feats");
        foreach (FeatSO featSO in allFeats)
        {
            feats[NormalizeGroupName(featSO.FeatName)] = featSO;
        }
    }

    public static string NormalizeGroupName(string name)
    {
        return name.Replace(" ", "").ToLower();
    }

    public static FeatSO GetFeat(string featName)
    {
        string key = NormalizeGroupName(featName);
        if (feats.TryGetValue(key, out FeatSO feat))
            return feat;

        return null;
    }
}

public static class SceneGroupDatabase
{
    public static Dictionary<string, SceneGroup> sceneGroups;

    [RuntimeInitializeOnLoadMethod]
    public static void Initialize()
    {
        sceneGroups = new Dictionary<string, SceneGroup>();
        SceneGroup[] allSceneGroups = Resources.LoadAll<SceneGroup>("SceneGroups");
        foreach (SceneGroup sceneGroup in allSceneGroups)
        {
            sceneGroups[NormalizeGroupName(sceneGroup.GroupName)] = sceneGroup;

        }
    }

    public static string NormalizeGroupName(string name)
    {
        return name.Replace(" ", "").ToLower();
    }

    public static SceneGroup GetSceneGroup(string groupName)
    {
        string key = NormalizeGroupName(groupName);
        if (sceneGroups.TryGetValue(key, out SceneGroup group))
            return group;

        Debug.LogWarning($"SceneGroup '{groupName}' not found.");
        return null;
    }
}

public class ItemDatabase
{
    public static Dictionary<string, ItemSO> ItemSOs;

    [RuntimeInitializeOnLoadMethod]
    public static void Initialize()
    {
        ItemSOs = new Dictionary<string, ItemSO>();
        ItemSO[] allItemSOs = Resources.LoadAll<ItemSO>("Items");
        foreach (ItemSO itemSO in allItemSOs)
        {
            ItemSOs[NormalizeGroupName(itemSO.ItemName)] = itemSO;
        }
    }

    public static string NormalizeGroupName(string name)
    {
        return name.Replace(" ", "").ToLower();
    }

    public static ItemSO GetItemSO(string itemSOName)
    {
        string key = NormalizeGroupName(itemSOName);
        if (ItemSOs.TryGetValue(key, out ItemSO itemSO))
            return itemSO;

        return null;
    }
}


