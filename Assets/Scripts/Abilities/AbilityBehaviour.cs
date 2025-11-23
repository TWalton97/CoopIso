using UnityEngine;

public abstract class AbilityBehaviour<T> : AbilityBehaviourBase
    where T : RuntimeAbility
{
    protected NewPlayerController player;
    protected T runtime; // typed runtime instance

    public override void Initialize(NewPlayerController player, RuntimeAbility runtime)
    {
        this.player = player;
        this.runtime = (T)runtime;
    }
    public override bool CanUse(ResourceController resourceController)
    {
        return resourceController.resource.resourceType == runtime.resourceType && resourceController.resource.RemoveResource(runtime.resourceAmount);
    }
}
