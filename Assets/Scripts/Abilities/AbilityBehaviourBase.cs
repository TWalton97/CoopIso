using UnityEngine;

public abstract class AbilityBehaviourBase : MonoBehaviour
{
    public abstract void Initialize(NewPlayerController player, RuntimeAbility runtime);
    public abstract void OnEnter();
    public abstract void OnUse();
    public abstract void OnExit();
    public abstract bool CanUse(ResourceController resourceController);
}
