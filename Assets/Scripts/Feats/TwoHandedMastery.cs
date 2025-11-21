using System;
public class TwoHandedMastery : Feat
{
    public override string FeatName { get; set; } = "Two Handed Mastery";
    public override string FeatDescription { get; set; } = "Imbues your character with great strength, allowing them to wield two-handed weapons with one hand.";
    public override int StartingFeatLevel { get; set; }
    public override int CurrentFeatLevel { get; set; } = 0;
    public override int MaximumFeatLevel { get; set; } = 1;
    public override int SkillPointsCostPerLevel { get; set; } = 5;
    public override int SkillPointsCostIncreasePerLevel { get; set; } = 0;


    public TwoHandedMastery(int startingLevel = 0)
    {
        StartingFeatLevel = startingLevel;
    }

    public override void OnActivate(FeatsController controller, Action activatedSuccess)
    {
        if (CurrentFeatLevel == MaximumFeatLevel) return;

        if (controller.experienceController.TrySpendSkillpoints(SkillPointsCostPerLevel))
        {
            controller.newPlayerController.PlayerStatsBlackboard.TwoHandedMastery = true;
            controller.newPlayerController.WeaponController.CheckForAppropriateMastery();
            CurrentFeatLevel++;
            SkillPointsCostPerLevel += SkillPointsCostIncreasePerLevel;
            activatedSuccess?.Invoke();
        }
    }

    public override void OnActivateNoReqs(FeatsController controller, Action activatedSuccess)
    {
        if (CurrentFeatLevel == MaximumFeatLevel) return;

        controller.newPlayerController.PlayerStatsBlackboard.TwoHandedMastery = true;
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
        return "Next Level: Allows your character to wield two-handed weapons in one hand.";
    }
}
