using UnityEngine;

public class WeaponAbilityBehaviour : AbilityBehaviour<WeaponRuntimeAbility>
{
    public WeaponRangeType weaponRangeType;
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
        if (resourceController.newPlayerController.WeaponController.instantiatedPrimaryWeapon != null && resourceController.newPlayerController.WeaponController.instantiatedPrimaryWeapon.weaponRangeType == weaponRangeType)
        {
            return resourceController.resource.resourceType == runtime.resourceType && resourceController.resource.RemoveResource(runtime.resourceAmount);
        }
        return false;
    }
}
