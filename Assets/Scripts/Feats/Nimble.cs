using System;
using Unity.VisualScripting;
using UnityEngine;

public class Nimble : Feat
{
    public override string FeatName { get; set; } = "Nimble";
    public override string FeatDescription { get; set; } = "Increases your character's maximum movement speed.";
    public float[] MovementSpeedIncreasePerLevel = { 0.2f, 0.3f, 0.4f };
    public override int StartingFeatLevel { get; set; }
    public override int CurrentFeatLevel { get; set; } = 0;
    public override int MaximumFeatLevel { get; set; } = 3;
    public override int SkillPointsCostPerLevel { get; set; } = 1;
    public override int SkillPointsCostIncreasePerLevel { get; set; } = 0;

    public Nimble(int startingLevel = 0)
    {
        StartingFeatLevel = startingLevel;
    }

    public override void OnActivate(FeatsController controller, Action activatedSuccess)
    {
        if (CurrentFeatLevel == MaximumFeatLevel) return;

        if (controller.experienceController.TrySpendSkillpoints(SkillPointsCostPerLevel))
        {
            controller.newPlayerController.IncreaseMovementSpeed(MovementSpeedIncreasePerLevel[CurrentFeatLevel]);
            CurrentFeatLevel++;
            SkillPointsCostPerLevel += SkillPointsCostIncreasePerLevel;
            activatedSuccess?.Invoke();
        }
    }

    public override void OnActivateNoReqs(FeatsController controller, Action activatedSuccess)
    {
        if (CurrentFeatLevel == MaximumFeatLevel) return;

        controller.newPlayerController.IncreaseMovementSpeed(MovementSpeedIncreasePerLevel[CurrentFeatLevel]);
        CurrentFeatLevel++;
        SkillPointsCostPerLevel += SkillPointsCostIncreasePerLevel;
        activatedSuccess?.Invoke();
    }

    public override string GenerateStatString()
    {
        if (CurrentFeatLevel == MaximumFeatLevel)
        {
            return "Maximum Level Reached";
        }
        return "Next Level: Increases movement speed by " + MovementSpeedIncreasePerLevel[CurrentFeatLevel];
    }
}
