using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FeatsController : MonoBehaviour
{
    public NewPlayerController newPlayerController;
    public ExperienceController experienceController;
    public PlayerHealthController playerHealthController;


    public List<FeatSO> AllFeats;
    public List<RuntimeFeat> UnlockedFeats;

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
            return experienceController.SkillPoints >= feat.GetCostPerLevel(0);
        }

        if (runtime.IsMaxLevel)
            return false;

        return experienceController.SkillPoints >= runtime.GetNextLevelCost();
    }

    public bool UnlockFeat(FeatSO feat, Action activatedSuccess)
    {
        var runtime = GetRuntimeFeat(feat);

        if (runtime == null)
        {
            int cost = feat.GetCostPerLevel(0);
            if (!experienceController.TrySpendSkillpoints(cost))
                return false;

            runtime = new RuntimeFeat(feat);
            UnlockedFeats.Add(runtime);

            feat.OnActivate(runtime.CurrentFeatLevel, newPlayerController, activatedSuccess);

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

            feat.OnActivate(runtime.CurrentFeatLevel, newPlayerController, activatedSuccess);

            return true;
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

    public bool IsMaxLevel => CurrentFeatLevel >= BaseFeatSO.MaximumFeatLevel;

    public int GetNextLevelCost()
    {
        return BaseFeatSO.GetCostPerLevel(CurrentFeatLevel);
    }
}
