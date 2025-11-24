using UnityEngine;

public abstract class AbilitySO : ScriptableObject
{
    public string AbilityName;
    public Sprite AbilityIcon;
    public AnimationClip AnimationClip;
    public Resources.ResourceType ResourceType;
    public float ResourceAmount;
    public float MovementSpeedMultiplierWhileUsing = 1;
    public bool CanRotateDuringCast = true;

    public abstract RuntimeAbility CreateRuntimeAbility();
}
