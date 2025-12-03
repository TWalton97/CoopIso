using UnityEngine;

public abstract class AbilitySO : ScriptableObject
{
    public string AbilityName;
    public Sprite AbilityIcon;
    public AnimationClip AnimationClip;
    public Resources.ResourceType ResourceType;
    public float ResourceAmount;
    public bool CanMoveWhileUsing = false;
    public bool CanRotateDuringCast = true;
    public int MaxLevel;
    [TextArea] public string AbilityDescription;

    [TextArea] public string LevelDescriptionTemplate;
    [TextArea] public string UpgradeDescriptionTemplate;

    public abstract RuntimeAbility CreateRuntimeAbility();

    public abstract string GetLevelDescription(int currentLevel);
    public abstract string GetCalculatedLevelDescription(int currentLevel, int damage);

    public abstract string GetUpgradeDescription(int currentLevel);

}
