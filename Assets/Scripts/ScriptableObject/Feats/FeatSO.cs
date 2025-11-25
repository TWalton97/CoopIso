using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FeatData", menuName = "Data/Feat Data")]
public abstract class FeatSO : ScriptableObject
{
    public string FeatName;
    public int[] SkillPointCostPerLevel;

    public virtual int GetCost(int level)
    {
        return SkillPointCostPerLevel[Mathf.Clamp(level - 1, 0, SkillPointCostPerLevel.Length - 1)];
    }

    public abstract string GetCurrentEffect(int level);
    public abstract string GetNextLevelEffect(int level);
    public abstract string GetDescription();

    public virtual void OnActivate(int CurrentFeatLevel, NewPlayerController controller)
    {

    }

    // public int GetCostPerLevel(int level)
    // {
    //     level = Mathf.Clamp(level, 0, MaximumFeatLevel - 1);
    //     return SkillPointCostPerLevel[level];
    // }

    // public virtual string GenerateUnlockString(int currentFeatLevel)
    // {
    //     return "";
    // }

    // public virtual string GenerateNextLevelString(int currentFeatLevel)
    // {
    //     return "";
    // }

    // public virtual string GenerateStatDescriptionString(int currentFeatLevel)
    // {
    //     return "";
    // }


}
