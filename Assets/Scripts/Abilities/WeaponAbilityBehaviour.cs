using UnityEngine;

public class WeaponAbilityBehaviour : AbilityBehaviour<WeaponRuntimeAbility>
{
    public override void OnEnter()
    {

    }

    public override void OnExit()
    {
    }

    public override void OnUse()
    {
    }

    public override bool CanUse(ResourceController resourceController)
    {
        return resourceController.resource.resourceType == runtime.resourceType && resourceController.resource.RemoveResource(runtime.resourceAmount);
    }
}
