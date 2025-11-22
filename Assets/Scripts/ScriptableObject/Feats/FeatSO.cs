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
    [field: SerializeField] public int SkillPointsCostPerLevel { get; set; }
    [field: SerializeField] public int SkillPointsCostIncreasePerLevel { get; set; }

    public virtual void OnActivate(int CurrentFeatLevel, FeatsController controller, Action activatedSuccess)
    {

    }

    public virtual string GenerateStatString()
    {
        return FeatStatDescription;
    }
}
