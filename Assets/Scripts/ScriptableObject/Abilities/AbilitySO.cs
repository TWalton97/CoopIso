using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data")]
public class AbilitySO : ScriptableObject
{
    public string AbilityName;
    public Sprite AbilityIcon;
    public AnimationClip AnimationClip;
    public Resources.ResourceType ResourceType;
    public float ResourceAmount;
}
