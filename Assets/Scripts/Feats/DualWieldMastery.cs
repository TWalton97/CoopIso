using System;
public class DualWieldMastery : Feat
{
    public override string FeatName { get; set; } = "Dual Wield Mastery";
    public override string FeatDescription { get; set; } = "Adds a powerful combo attack when using two weapons at once.";
    public override int StartingFeatLevel { get; set; }
    public override int CurrentFeatLevel { get; set; } = 0;
    public override int MaximumFeatLevel { get; set; } = 1;
    public override int SkillPointsCostPerLevel { get; set; } = 3;
    public override int SkillPointsCostIncreasePerLevel { get; set; } = 0;


    public DualWieldMastery(int startingLevel = 0)
    {
        StartingFeatLevel = startingLevel;
    }

    public override void OnActivate(FeatsController controller, Action activatedSuccess)
    {
        if (CurrentFeatLevel == MaximumFeatLevel) return;

        if (controller.experienceController.TrySpendSkillpoints(SkillPointsCostPerLevel))
        {
            controller.newPlayerController.PlayerStatsBlackboard.DualWieldMastery = true;
            controller.newPlayerController.WeaponController.CheckForAppropriateMastery();
            CurrentFeatLevel++;
            SkillPointsCostPerLevel += SkillPointsCostIncreasePerLevel;
            activatedSuccess?.Invoke();
        }
    }

    public override void OnActivateNoReqs(FeatsController controller, Action activatedSuccess)
    {
        if (CurrentFeatLevel == MaximumFeatLevel) return;

        controller.newPlayerController.PlayerStatsBlackboard.DualWieldMastery = true;
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
        return "Next Level: Improves your character's ability to fight with two weapons at once.";
    }
}
