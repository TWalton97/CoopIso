using System;

public class ArmorMastery : Feat
{
    public override string FeatName { get; set; } = "Armor Mastery";
    public override string FeatDescription { get; set; } = "Allows your character to wear heavier armor, providing better defenses.";
    public override int StartingFeatLevel { get; set; }
    public override int CurrentFeatLevel { get; set; } = 0;
    public override int MaximumFeatLevel { get; set; } = 3;
    public override int SkillPointsCostPerLevel { get; set; } = 2;
    public override int SkillPointsCostIncreasePerLevel { get; set; } = 1;


    public ArmorMastery(int startingLevel = 0)
    {
        StartingFeatLevel = startingLevel;
    }

    public override void OnActivate(FeatsController controller, Action activatedSuccess)
    {
        if (CurrentFeatLevel == MaximumFeatLevel) return;

        if (controller.experienceController.TrySpendSkillpoints(SkillPointsCostPerLevel))
        {
            DetermineNextArmorLevel(controller);
            CurrentFeatLevel++;
            SkillPointsCostPerLevel += SkillPointsCostIncreasePerLevel;
            activatedSuccess?.Invoke();
        }
    }

    public override void OnActivateNoReqs(FeatsController controller, Action activatedSuccess)
    {
        if (CurrentFeatLevel == MaximumFeatLevel) return;

        DetermineNextArmorLevel(controller);
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

        switch (CurrentFeatLevel)
        {
            case 0:
                return "Next Level: Allows your character to wear light armor.";
            case 1:
                return "Next Level: Allows your character to wear medium armor.";
            case 2:
                return "Next Level: Allows your character to wear heavy armor.";
        }

        return "Maximum Level Reached";
    }

    private void DetermineNextArmorLevel(FeatsController controller)
    {
        switch (CurrentFeatLevel)
        {
            case 0:
                controller.newPlayerController.PlayerStatsBlackboard.armorType = ArmorType.Light;
                break;
            case 1:
                controller.newPlayerController.PlayerStatsBlackboard.armorType = ArmorType.Medium;
                break;
            case 2:
                controller.newPlayerController.PlayerStatsBlackboard.armorType = ArmorType.Heavy;
                break;
        }
    }
}
