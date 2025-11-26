using System;
public class ShieldTraining : Feat
{
    public override string FeatName { get; set; } = "Shield Training";
    public override string FeatDescription { get; set; } = "Allows your character to equip shields.";
    public override int StartingFeatLevel { get; set; }
    public override int CurrentFeatLevel { get; set; } = 0;
    public override int MaximumFeatLevel { get; set; } = 1;
    public override int SkillPointsCostPerLevel { get; set; } = 2;
    public override int SkillPointsCostIncreasePerLevel { get; set; } = 0;


    public ShieldTraining(int startingLevel = 0)
    {
        StartingFeatLevel = startingLevel;
    }

    public override void OnActivate(FeatsController controller, Action activatedSuccess)
    {
        if (CurrentFeatLevel == MaximumFeatLevel) return;

        if (controller.experienceController.TrySpendSkillpoints(SkillPointsCostPerLevel))
        {
            controller.PlayerController.PlayerStatsBlackboard.CanEquipShields = true;
            CurrentFeatLevel++;
            SkillPointsCostPerLevel += SkillPointsCostIncreasePerLevel;
            activatedSuccess?.Invoke();
        }
    }

    public override void OnActivateNoReqs(FeatsController controller, Action activatedSuccess)
    {
        if (CurrentFeatLevel == MaximumFeatLevel) return;

        controller.PlayerController.PlayerStatsBlackboard.CanEquipShields = true;
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
        return "Next Level: Allows your character to equip shields";
    }
}
