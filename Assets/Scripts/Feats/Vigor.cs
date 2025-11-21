
using System;

public class Vigor : Feat
{
    public override string FeatName { get; set; } = "Vigor";
    public override string FeatDescription { get; set; } = "Increases the amount of damage your character can take before dying.";
    public int[] HealthIncreasePerLevel = { 5, 10, 15, 20, 25 };
    public override int StartingFeatLevel { get; set; }
    public override int CurrentFeatLevel { get; set; } = 0;
    public override int MaximumFeatLevel { get; set; } = 5;
    public override int SkillPointsCostPerLevel { get; set; } = 1;
    public override int SkillPointsCostIncreasePerLevel { get; set; } = 1;


    public Vigor(int startingLevel = 0)
    {
        StartingFeatLevel = startingLevel;
    }

    public override void OnActivate(FeatsController controller, Action activatedSuccess)
    {
        if (CurrentFeatLevel == MaximumFeatLevel) return;

        if (controller.experienceController.TrySpendSkillpoints(SkillPointsCostPerLevel))
        {
            controller.playerHealthController.IncreaseMaximumHealth(HealthIncreasePerLevel[CurrentFeatLevel]);
            CurrentFeatLevel++;
            SkillPointsCostPerLevel += SkillPointsCostIncreasePerLevel;
            activatedSuccess?.Invoke();
        }
    }

    public override void OnActivateNoReqs(FeatsController controller, Action activatedSuccess)
    {
        if (CurrentFeatLevel == MaximumFeatLevel) return;

        controller.playerHealthController.IncreaseMaximumHealth(HealthIncreasePerLevel[CurrentFeatLevel]);
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
        return "Next Level: Increases maximum health by " + HealthIncreasePerLevel[CurrentFeatLevel];
    }
}
