using System;

public abstract class Feat : IFeat
{
    public abstract string FeatName { get; set; }
    public abstract string FeatDescription { get; set; }
    public abstract int StartingFeatLevel { get; set; }
    public abstract int CurrentFeatLevel { get; set; }
    public abstract int MaximumFeatLevel { get; set; }
    public abstract int SkillPointsCostPerLevel { get; set; }
    public abstract int SkillPointsCostIncreasePerLevel { get; set; }
    public virtual void OnActivate(FeatsController controller, Action activatedSuccess)
    {

    }

    public virtual void OnActivateNoReqs(FeatsController controller, Action activatedSuccess)
    {

    }

    public Feat(int startingLevel = 0)
    {
        StartingFeatLevel = startingLevel;
    }

    public virtual string GenerateStatString()
    {
        return "";
    }
}
