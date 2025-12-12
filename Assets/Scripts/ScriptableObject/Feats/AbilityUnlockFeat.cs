using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FeatData", menuName = "Data/Feat Data/Ability Unlock Feat")]
public class AbilityUnlockFeat : FeatSO
{
    public AbilitySO AbilityToUnlock;

    public override string GetCurrentEffect(int level)
    {
        if (level == 0) return AbilityToUnlock.GetLevelDescription(level + 1);
        return AbilityToUnlock.GetLevelDescription(level);
    }

    public override string GetNextLevelEffect(int level)
    {
        if (level >= AbilityToUnlock.MaxLevel) return "Maximum level Reached";
        if (level == 0) return "Next Level: Unlocks ability " + AbilityToUnlock.AbilityName;
        return "Next Level: " + AbilityToUnlock.GetUpgradeDescription(level);
    }

    public override string GetDescription()
    {
        return AbilityToUnlock.AbilityDescription;
    }

    public override void OnActivate(int CurrentFeatLevel, NewPlayerController controller)
    {
        controller.AbilityController.UnlockAbility(AbilityToUnlock);

    }
}

