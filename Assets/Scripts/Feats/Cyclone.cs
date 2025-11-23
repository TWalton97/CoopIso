using System;
public class Cyclone : Feat
{
    public override string FeatName { get; set; } = "Cyclone";
    public override string FeatDescription { get; set; } = "Spin rapidly, hitting nearby enemies 5 times for 20% weapon damage";
    public override int StartingFeatLevel { get; set; }
    public override int CurrentFeatLevel { get; set; } = 0;
    public override int MaximumFeatLevel { get; set; } = 4;
    public override int SkillPointsCostPerLevel { get; set; } = 2;
    public override int SkillPointsCostIncreasePerLevel { get; set; } = 0;
    public AbilitySO ability;

    public Cyclone(AbilitySO ability, int startingLevel = 0)
    {
        StartingFeatLevel = startingLevel;
        this.ability = ability;
    }

    public override void OnActivate(FeatsController controller, Action activatedSuccess)
    {
        if (CurrentFeatLevel == MaximumFeatLevel) return;

        if (controller.experienceController.TrySpendSkillpoints(SkillPointsCostPerLevel))
        {
            controller.newPlayerController.AbilityController.UnlockAbility(ability);
            CurrentFeatLevel++;
            SkillPointsCostPerLevel += SkillPointsCostIncreasePerLevel;
            activatedSuccess?.Invoke();
        }
    }

    public override void OnActivateNoReqs(FeatsController controller, Action activatedSuccess)
    {
        if (CurrentFeatLevel == MaximumFeatLevel) return;

        controller.newPlayerController.AbilityController.UnlockAbility(ability);
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
        else if (CurrentFeatLevel != 0)
        {
            return "Increases weapon damage per hit by 5%";
        }
        else
        {
            return "Next Level: Unlocks ability Cyclone";
        }
    }
}
