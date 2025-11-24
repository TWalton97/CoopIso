using System.Collections.Generic;
using UnityEngine;

public class BuffAbilityBehaviour : AbilityBehaviour<BuffRuntimeAbility>
{
    public WeaponRangeType weaponRangeType;
    public LayerMask PlayerLayer;
    public float radius;
    public StatusSO Status;

    private List<StatusController> statusControllers = new();

    public override void OnEnter()
    {

    }

    public override void OnExit()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, PlayerLayer);
        foreach (Collider coll in colliders)
        {
            if (coll.TryGetComponent(out StatusController statusController))
            {
                if (!statusControllers.Contains(statusController))
                {
                    statusControllers.Add(statusController);
                }
            }
        }

        foreach (StatusController s in statusControllers)
        {
            s.ApplyStatus(Status, player);
        }

        statusControllers.Clear();
    }

    public override void OnUse()
    {
    }

    public override bool CanUse(ResourceController resourceController)
    {
        if (weaponRangeType == WeaponRangeType.Melee || weaponRangeType == WeaponRangeType.Ranged)
        {
            if (resourceController.newPlayerController.WeaponController.instantiatedPrimaryWeapon == null || resourceController.newPlayerController.WeaponController.instantiatedPrimaryWeapon.weaponRangeType != weaponRangeType)
            {
                return false;
            }
        }
        else if (weaponRangeType == WeaponRangeType.Shield)
        {
            if (!resourceController.newPlayerController.WeaponController.HasShieldEquipped)
            {
                return false;
            }
        }

        return resourceController.resource.resourceType == runtime.resourceType && resourceController.resource.RemoveResource(runtime.resourceAmount);
    }
}
