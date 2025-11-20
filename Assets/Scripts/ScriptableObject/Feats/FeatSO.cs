using UnityEngine;

[CreateAssetMenu(fileName = "FeatData", menuName = "Data/Feat Data")]
public class FeatSO : ScriptableObject
{
    public virtual int MaximumFeatLevel { get; set; }
    public int SkillPointsCostPerLevel { get; set; }
}
