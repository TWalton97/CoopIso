public class Vigor : Feat
{
    public int[] HealthIncreasePerLevel = { 5, 10, 15, 20, 25 };
    public override void OnActivate(FeatsController controller)
    {
        if (CurrentFeatLevel == FeatData.MaximumFeatLevel) return;

        if (controller.experienceController.TrySpendSkillpoints(FeatData.SkillPointsCostPerLevel))
        {
            controller.playerHealthController.IncreaseMaximumHealth(HealthIncreasePerLevel[CurrentFeatLevel]);
            CurrentFeatLevel++;
        }
    }
}
