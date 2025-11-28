using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FeatsController : MonoBehaviour
{
    public NewPlayerController PlayerController;
    public ExperienceController experienceController;

    public List<FeatSO> AllFeats;
    public List<RuntimeFeat> UnlockedFeats;

    public Action<FeatSO, int> OnFeatLevelChanged;

    public void SetupClassPreset(ClassFeatConfig classFeatConfig)
    {
        SetupClassConfig(classFeatConfig);
    }

    public RuntimeFeat GetRuntimeFeat(FeatSO feat)
    {
        return UnlockedFeats.FirstOrDefault(f => f.BaseFeatSO == feat);
    }

    public bool IsUnlocked(FeatSO feat)
    {
        return GetRuntimeFeat(feat) != null;
    }

    public bool CanAfford(FeatSO feat)
    {
        var runtime = GetRuntimeFeat(feat);

        if (runtime == null)
        {
            return experienceController.SkillPoints >= feat.GetCost(0);
        }

        if (runtime.IsMaxLevel)
            return false;

        return experienceController.SkillPoints >= runtime.GetNextLevelCost();
    }

    public bool UnlockFeat(FeatSO feat)
    {
        var runtime = GetRuntimeFeat(feat);

        if (runtime == null)
        {
            int cost = feat.GetCost(0);
            if (!experienceController.TrySpendSkillpoints(cost))
                return false;

            runtime = new RuntimeFeat(feat);
            UnlockedFeats.Add(runtime);

            feat.OnActivate(runtime.CurrentFeatLevel, PlayerController);
            OnFeatLevelChanged?.Invoke(feat, runtime.CurrentFeatLevel);
            return true;
        }
        else
        {
            if (runtime.IsMaxLevel)
                return false;

            int cost = runtime.GetNextLevelCost();
            if (!experienceController.TrySpendSkillpoints(cost))
                return false;

            runtime.CurrentFeatLevel++;

            feat.OnActivate(runtime.CurrentFeatLevel, PlayerController);
            OnFeatLevelChanged?.Invoke(feat, runtime.CurrentFeatLevel);
            return true;
        }
    }

    public int GetCurrentFeatLevel(FeatSO feat)
    {
        var runtime = GetRuntimeFeat(feat);
        if (runtime == null) return 0;

        return runtime.CurrentFeatLevel;
    }

    public bool UnlockFeatBypassReqs(FeatSO feat)
    {
        var runtime = GetRuntimeFeat(feat);

        if (runtime == null)
        {
            int cost = feat.GetCost(0);

            runtime = new RuntimeFeat(feat);
            UnlockedFeats.Add(runtime);

            feat.OnActivate(runtime.CurrentFeatLevel, PlayerController);
            OnFeatLevelChanged?.Invoke(feat, runtime.CurrentFeatLevel);

            return true;
        }
        else
        {
            if (runtime.IsMaxLevel)
                return false;

            int cost = runtime.GetNextLevelCost();

            runtime.CurrentFeatLevel++;

            feat.OnActivate(runtime.CurrentFeatLevel, PlayerController);
            OnFeatLevelChanged?.Invoke(feat, runtime.CurrentFeatLevel);

            return true;
        }
    }

    private void SetupClassConfig(ClassFeatConfig classFeatConfig)
    {
        foreach (ClassFeatConfig.StartingFeat startingFeat in classFeatConfig.startingFeats)
        {
            AllFeats.Add(startingFeat.feat);
            for (int i = 0; i < startingFeat.startingLevel; i++)
            {
                UnlockFeatBypassReqs(startingFeat.feat);
            }
        }
    }
}

[Serializable]
public class RuntimeFeat
{
    public FeatSO BaseFeatSO;
    public int CurrentFeatLevel;

    public RuntimeFeat(FeatSO baseFeat)
    {
        BaseFeatSO = baseFeat;
        CurrentFeatLevel = 1;
    }

    public bool IsMaxLevel => CurrentFeatLevel >= BaseFeatSO.SkillPointCostPerLevel.Length;

    public int GetNextLevelCost()
    {
        return BaseFeatSO.GetCost(CurrentFeatLevel);
    }
}

[Serializable]
public class ClassFeatConfig
{
    [Serializable]
    public struct StartingFeat
    {
        public FeatSO feat;
        public int startingLevel;
    }

    public List<StartingFeat> startingFeats;
}
