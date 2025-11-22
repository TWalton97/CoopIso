using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FeatData", menuName = "Data/Feat Data/Ability Unlock Feat")]
public class AbilityUnlockFeat : FeatSO
{
    public BaseAbility AbilityToUnlock;

    public override void OnActivate(int CurrentFeatLevel, FeatsController controller, Action activatedSuccess)
    {
        if (CurrentFeatLevel == MaximumFeatLevel) return;

        if (controller.experienceController.TrySpendSkillpoints(SkillPointsCostPerLevel))
        {
            controller.newPlayerController.AbilityController.UnlockAbility(AbilityToUnlock);
            CurrentFeatLevel++;
            SkillPointsCostPerLevel += SkillPointsCostIncreasePerLevel;
            activatedSuccess?.Invoke();
        }
    }
}
