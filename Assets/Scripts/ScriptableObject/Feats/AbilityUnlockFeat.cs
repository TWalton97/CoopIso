using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FeatData", menuName = "Data/Feat Data/Ability Unlock Feat")]
public class AbilityUnlockFeat : FeatSO
{
    public AbilitySO AbilityToUnlock;

    public override string GetCurrentLevelDescription(int level)
    {
        if (level == 0) return "Current: " + AbilityToUnlock.GetCurrentLevelDescription(level + 1);
        return "Current: " + AbilityToUnlock.GetCurrentLevelDescription(level);
    }

    public override string GetNextLevelDescription(int level)
    {
        if (level >= AbilityToUnlock.MaxLevel) return "Maximum level Reached";
        return "Next Level: " + AbilityToUnlock.GetNextLevelDescription(level);
    }

    public override void OnActivate(int CurrentFeatLevel, NewPlayerController controller)
    {
        controller.AbilityController.UnlockAbility(AbilityToUnlock);

    }
}

