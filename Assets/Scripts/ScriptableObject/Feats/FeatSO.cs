using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FeatData", menuName = "Data/Feat Data")]
public class FeatSO : ScriptableObject
{
    [field: SerializeField] public virtual string FeatName { get; set; }
    [field: SerializeField] public virtual string FeatUnlockDescription { get; set; }
    [field: SerializeField] public virtual string FeatUpgradeDescription { get; set; }
    [field: SerializeField] public virtual string FeatStatDescription { get; set; }
    [field: SerializeField] public virtual int StartingFeatLevel { get; set; }
    [field: SerializeField] public virtual int MaximumFeatLevel { get; set; }
    [field: SerializeField] public int[] SkillPointCostPerLevel { get; set; }

    public virtual void OnActivate(int CurrentFeatLevel, NewPlayerController controller, Action activatedSuccess)
    {

    }

    public int GetCostPerLevel(int level)
    {
        level = Mathf.Clamp(level, 0, MaximumFeatLevel - 1);
        return SkillPointCostPerLevel[level];
    }

    public virtual string GenerateUnlockString(int currentFeatLevel)
    {
        return "";
    }

    public virtual string GenerateNextLevelString(int currentFeatLevel)
    {
        return "";
    }

    public virtual string GenerateStatDescriptionString(int currentFeatLevel)
    {
        return "";
    }


}
