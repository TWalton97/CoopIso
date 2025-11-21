using UnityEngine;

[CreateAssetMenu(fileName = "FeatData", menuName = "Data/Feat Data")]
public class FeatSO : ScriptableObject
{
    [field: SerializeField] public virtual int MaximumFeatLevel { get; set; }
    [field: SerializeField] public int SkillPointsCostPerLevel { get; set; }
}
