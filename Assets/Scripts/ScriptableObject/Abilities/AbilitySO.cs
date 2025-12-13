using UnityEngine;

public abstract class AbilitySO : ScriptableObject
{
    public string AbilityName;
    public Sprite AbilityIcon;
    public AnimationClip AnimationClip;
    public PlayerResource.ResourceType ResourceType;
    public float ResourceAmount;
    public AbilityType AbilityType = AbilityType.Active;
    public bool CanMoveWhileUsing = false;
    public bool CanRotateDuringCast = true;
    public bool IsChannelingAbility = false;
    public int MaxLevel;
    [TextArea] public string CurrentLevelDescription;
    [TextArea] public string NextLevelDescription;

    public abstract RuntimeAbility CreateRuntimeAbility();

    public abstract string GetLevelDescription(int currentLevel);
    public abstract string GetCalculatedLevelDescription(int currentLevel, int damage);

    public abstract string GetUpgradeDescription(int currentLevel);

}

public enum AbilityType
{
    Active,
    Passive
}
