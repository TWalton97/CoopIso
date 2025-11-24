using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityBehaviour<T> : AbilityBehaviourBase
    where T : RuntimeAbility
{
    protected NewPlayerController player;
    protected T runtime; // typed runtime instance
    protected List<StatusSO> statuses;

    public override void Initialize(NewPlayerController player, RuntimeAbility runtime, List<StatusSO> statuses = null)
    {
        this.player = player;
        this.runtime = (T)runtime;
        this.statuses = statuses;
    }
    public override bool CanUse(ResourceController resourceController)
    {
        return resourceController.resource.resourceType == runtime.resourceType && resourceController.resource.RemoveResource(runtime.resourceAmount);
    }
}
