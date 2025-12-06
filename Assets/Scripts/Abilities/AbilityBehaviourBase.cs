using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityBehaviourBase : MonoBehaviour
{
    public abstract void Initialize(NewPlayerController player, RuntimeAbility runtime, List<StatusSO> statuses = null);
    public abstract void OnEnter();
    public abstract void OnUse();
    public abstract void OnChannelTick(float deltaTime);
    public abstract void OnExit();
    public abstract bool CanUse(ResourceController resourceController);
}
