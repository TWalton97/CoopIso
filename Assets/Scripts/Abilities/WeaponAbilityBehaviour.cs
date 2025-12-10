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

    public override void OnChannelTick(float deltaTime)
    {

    }

    public override bool CanUse(ResourceController resourceController)
    {
        if (weaponRangeType == WeaponRangeType.Melee)
        {
            if (resourceController.newPlayerController.WeaponController.instantiatedPrimaryWeapon == null || resourceController.newPlayerController.WeaponController.instantiatedPrimaryWeapon.weaponRangeType != WeaponRangeType.Melee)
            {
                return false;
            }
        }
        else if (weaponRangeType == WeaponRangeType.Ranged)
        {
            if (resourceController.newPlayerController.WeaponController.instantiatedPrimaryWeapon == null || resourceController.newPlayerController.WeaponController.instantiatedPrimaryWeapon.weaponRangeType != WeaponRangeType.Ranged)
            {
                return false;
            }
        }
        else if (weaponRangeType == WeaponRangeType.Shield)
        {
            if (resourceController.newPlayerController.WeaponController.instantiatedSecondaryWeapon == null || resourceController.newPlayerController.WeaponController.instantiatedSecondaryWeapon.weaponRangeType != weaponRangeType)
            {
                return false;
            }
        }
        return resourceController.resource.resourceType == runtime.resourceType && resourceController.resource.RemoveResource(runtime.resourceAmount);
    }

    public virtual int CalculateDamagePerTick()
    {
        return Mathf.CeilToInt(player.WeaponController.CombinedWeaponDamage * runtime.WeaponDamage);
    }
}
