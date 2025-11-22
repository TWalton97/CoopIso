using System;

[Serializable]
public class Feat : IFeat
{
    public virtual string FeatName { get; set; }
    public virtual string FeatDescription { get; set; }
    public virtual int StartingFeatLevel { get; set; }
    public virtual int CurrentFeatLevel { get; set; }
    public virtual int MaximumFeatLevel { get; set; }
    public virtual int SkillPointsCostPerLevel { get; set; }
    public virtual int SkillPointsCostIncreasePerLevel { get; set; }
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
